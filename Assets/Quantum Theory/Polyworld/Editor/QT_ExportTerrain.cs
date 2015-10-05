/*Quantum Theory Entertainment - Polyworld Terrain Conversion Tool 4/8/2014 v1.0
 * http://www.qt-ent.com
 *  
 * v1.0 - known issues:
 *         * relies on strings for naming of files and folders. Need to investigate GUIDs for future releases.
 *         * fails if duplicate polyworld terrains exist.
 *         * Mesh chop works but might leave small meshes behind due to arbitrary "cut box" scale set from face size.
 *         * only supports square unity terrains. non-square may be possible in the future
 *         * With large terrains, the mesh can be very dense even with the setting at "eighth," so a strong understanding of how to generate Unity Terrains is strongly recommended.
 *         * Bands of color from the opposite edge of a terrain mesh may appear after calculating vertex colors. Set the game view aspect to 1:1 and resize a bit, then recalculate.
 *         
 * Export from Unity Terrain to OBJ code:
 * Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
 * C # manual conversion work by Yun Kyu Choi
 * 
 * */

using UnityEngine;
using UnityEngineInternal;
using UnityEditor;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

enum SaveFormat { Triangles, Quads }
enum SaveResolution { Full=0, Half, Quarter, Eighth, Sixteenth }

class QT_ExportTerrain : EditorWindow
{
	SaveFormat saveFormat = SaveFormat.Triangles;
	SaveResolution saveResolution = SaveResolution.Half;
	
	 TerrainData terrain;
     Terrain terrainObject;
	 Vector3 terrainPos;

	 GameObject targetGO; //gameobject with all the meshes in it we'll create/adjust

	int tCount;
	int counter;
	int totalCount;
	int progressUpdateInterval = 10000;

    bool shownWarning = false; //turns off the full res warning message after it's shown once.
	bool usePrefab=true;
	bool smoothVertColor=false;
    bool autoUpdateVC = false; //auto update the vertex colors flag
    bool meshHidden = false;
    bool terrainHidden = false;
    bool setTerrainStatic = false;

	string MeshName="New Terrain Mesh";

    Vector3[] tGO_PreviousTransform = new Vector3[3]; //stores the transform of the targetGO prior to generating it again. Useful if the targetGO is a custom mesh.
    
    //UI icons
    Texture GenerateIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Quantum Theory/Polyworld/Editor/QT_Generate-icon.png", typeof(Texture));
	
    //Texture DivideIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Editor/QT_Divide-icon.png", typeof(Texture));
	Texture WorldIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Quantum Theory/Polyworld/Editor/QT_PolyWorld-icon.png", typeof(Texture));
	
   // Texture DividerIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Editor/QT_Divider-icon.png", typeof(Texture));

    int[] chunkVal = new int[] { 30,20, 15, 10, 5 }; //the chunk multiplier. Take the size of a face and multiply it by this to get the bound size. done in dividemesh()
    int chunkValUI = 2; //the int value in the gui interface. Rather have it int so it's easier for the user.

 
	[MenuItem("Window/Quantum Theory/PolyWorld Terrain")]
	static void Init()
	{
        GUIContent GC = new GUIContent("PolyWorld");
        QT_ExportTerrain window = (QT_ExportTerrain)EditorWindow.GetWindow(typeof(QT_ExportTerrain));
        window.Show();
        window.titleContent = GC;    
        window.maxSize = new Vector2(300, 370);
        window.minSize = window.maxSize;
        window.Show();
	}

    

	void OnGUI()
	{
        EditorGUI.DrawPreviewTexture(new Rect(10, 10, 280, 60), WorldIcon);
       

        if (Selection.gameObjects.Length > 0 && Selection.gameObjects[0].GetComponent(typeof(Terrain)))
        {    
            terrainObject = (Terrain)Selection.gameObjects[0].GetComponent(typeof(Terrain));
            terrain = terrainObject.terrainData;
            terrainPos = terrainObject.transform.position;
            if (terrainObject.gameObject.isStatic)
                Debug.LogWarning("Setting Unity Terrain to non-static so lighting isn't baked into the PolyWorld Mesh.");
            terrainObject.gameObject.isStatic = setTerrainStatic;

            GUILayout.BeginArea(new Rect(10, 70, 140, 100));
            GUILayout.Label("Mesh Type:");
            saveFormat = (SaveFormat)EditorGUILayout.EnumPopup("", saveFormat);
            GUILayout.Label("Resolution:");
            saveResolution = (SaveResolution)EditorGUILayout.EnumPopup("", saveResolution);
            TerrainCollider[] tcs = UnityEngine.Object.FindObjectsOfType<TerrainCollider>();
            
            //check to see if the terrain is using the standard shader. if so, revert it to legacy diffuse so we render VC properly.
            if (terrainObject.materialType != Terrain.MaterialType.BuiltInLegacyDiffuse)
            {
                terrainObject.materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
                Debug.Log("Changing terrain material type to Legacy Diffuse so the vertex colors will render properly for PolyWorld Terrains.");
            }

            foreach (TerrainCollider t in tcs)
            {
                //if the two terrain datas match but we aren't looking at the same gameobject..
                if (t.terrainData.name.Equals(terrain.name) && t.gameObject != terrainObject.gameObject)
                {
                    targetGO = t.gameObject;
                    break;
                }
                else
                    targetGO = null;
            }
            //if there is a target gameobject in the scene
            if (targetGO)
            {
                usePrefab = GUILayout.Toggle(usePrefab, "Use Associated Mesh");//EditorGUILayout.Toggle("Use Associated Mesh", usePrefab);                
            }
            else
                usePrefab = false;
            GUILayout.EndArea();


            //Generate Terrain
            if (GUI.Button(new Rect(160, 72, 130, 90), GenerateIcon))//GUILayout.Button(exportText))
            {
                bool badTerrain = false;
                if (terrain.size.x % terrain.size.z > 0)
                    badTerrain = true;
                if (badTerrain)
                    EditorUtility.DisplayDialog("Incompatible Terrain", "The terrain conversion has one requirement:\n\n1. " + terrainObject.name + " needs to have its length and width to be equal.", "Cancel Export");
                else if (targetGO == null && usePrefab)
                    EditorUtility.DisplayDialog("No Associated Mesh", "This terrain does not have an associated mesh generated yet. Uncheck 'Use Associated Mesh' and regenerate.", "OK");
                else if (terrainObject.name.Equals("Terrain"))
                    EditorUtility.DisplayDialog("Change Terrain Name", "Please give the Unity Terrain object a unique name other than 'Terrain.' This will make asset management easier for you.", "OK");
                else if (saveResolution == SaveResolution.Full && shownWarning==false)
                {
                    if (EditorUtility.DisplayDialog("Warning", "Warning: Exporting a full resolution terrain carries some significant risks:\n\n1. It will take some significant time to generate depending on the speed of your cpu.\n\n2. It will be more expensive to render it on your target platform as there is much more geometry to render.\n\n3. It will take longer to lightmap.\n\n Only choose 'Full Resolution' if you know what you are doing.\n\nAre you sure you want to render at Full Resolution?", "Yes", "Cancel"))
                    {
                        shownWarning = true;
                       // AssetDatabase.Refresh();
                       
                        Export();
                    }
                }
                else
                {
                    //AssetDatabase.Refresh();
                    Export();
                }
            }
            if (usePrefab)
            {  
                if (targetGO!=null)
                {
                    MeshFilter[] mf = targetGO.GetComponentsInChildren<MeshFilter>(true);

                    GUILayout.BeginArea(new Rect(10, 170, 280, 30));                   
                    GUILayout.Label("Editing: " + targetGO.name);
                    GUILayout.EndArea();
                    GUILayout.BeginArea(new Rect(10, 190, 140, 80));
                    meshHidden = GUILayout.Toggle(meshHidden,"Hide Associated Mesh");
                    terrainHidden = GUILayout.Toggle(terrainHidden, "Hide Terrain");
                    autoUpdateVC = GUILayout.Toggle(autoUpdateVC,"Auto Bake VC");
                    smoothVertColor = GUILayout.Toggle(smoothVertColor,"Smooth Vertex Color");
                    GUILayout.EndArea();
                    GUILayout.BeginArea(new Rect(160, 187, 130, 60));
                    if (GUILayout.Button("Use Custom Shaders"))
                    {
                        Shader s = Shader.Find("QuantumTheory/VertexColors/Unity5/Diffuse");
                        if (s != null)
                        {
                            MeshRenderer[] meshRenderers = targetGO.GetComponentsInChildren<MeshRenderer>();

                            foreach (MeshRenderer m in meshRenderers)
                                m.sharedMaterial.shader = s;
                            Debug.Log("Shaders applied successfully.");
                        }
                        else
                            Debug.LogWarning("Custom shaders not found! Please manually assign a shader that supports vertex colors.");

                    }
                  
                    if (GUILayout.Button("Bake Vertex Colors"))                        
                            RenderVertexColors(targetGO);
                    GUILayout.EndArea();
                   // EditorGUI.DrawPreviewTexture(new Rect(10, 265, 280, DividerIcon.height), DividerIcon);

                    

                    //if there are no chunk meshes in the prefab
                    if (!mf[0].sharedMesh.name.Contains("-Chunk"))
                    {
                        GUILayout.BeginArea(new Rect(10, 275, 280, 30));
                        chunkValUI = EditorGUILayout.IntSlider("Chunk Density:", chunkValUI, 1, 5, null);
                        GUILayout.EndArea();
                        float faceSize = GetFaceSize(mf[0].sharedMesh);
                        float newBoundSize = (float)(faceSize * chunkVal[chunkValUI - 1]);
                        int numMeshes = Mathf.CeilToInt(terrain.size.x / newBoundSize);
                        EditorGUI.HelpBox(new Rect(10, 295, 280, 40), "Chunk Size:  " + newBoundSize + "m\n" + "Terrain Size:  " + terrain.size.x + "m x " + terrain.size.z + "m\n" + "Approximate Mesh Count: " + numMeshes * numMeshes, MessageType.None);                       
                        GUILayout.BeginArea(new Rect(10, 340, 280, 30));
                        //DIVIDE MESH
                        if (GUILayout.Button("Divide Mesh into Chunks"))
                        {
                            //add a check here to see if there is a folder with chunks inside. Delete them since we'll regenerate them.
                            string basePath = Application.dataPath;
                            string pathB = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(targetGO));
                            pathB = pathB.Replace(targetGO.name + ".prefab", "");
                            basePath = basePath.Replace("Assets", pathB + terrainObject.name + "-Chunks");
                            if (Directory.Exists(basePath))
                                FileUtil.DeleteFileOrDirectory(basePath);
                            AssetDatabase.Refresh();
                            DivideMesh();
                            mf = null;
                        }
                        GUILayout.EndArea();
                    }


                   
                    else
                        EditorGUI.HelpBox(new Rect(10, 295, 280, 40), "Regenerate the terrain to use different Chunk sizes.", MessageType.None);
                    if (mf!=null)
                    {
                        if (meshHidden == false && mf[0].GetComponent<Renderer>().enabled != true)
                            ShowTerrainMesh(mf);
                        else if (meshHidden == true && mf[0].GetComponent<Renderer>().enabled != false)
                        {
                            HideTerrainMesh(mf);
                            if (terrainHidden)
                            {
                                terrainObject.enabled = true;
                                terrainHidden = false;
                            }
                        }
                    }
                    if (terrainHidden == false && terrainObject.enabled!=true)
                    {
                        terrainObject.enabled = true;
                    }
                    else if (terrainHidden == true && terrainObject.enabled != false)
                    {
                        terrainObject.enabled = false;
                        if (meshHidden == true)
                        {
                            ShowTerrainMesh(mf);
                            meshHidden = false;
                        }
                    }
                    

                }
            }
            //else we are generating a new mesh          
            else
            {
                usePrefab = false;
                targetGO = null;
                EditorGUI.HelpBox(new Rect(10, 295, 280, 60), "Select your mesh type, then pick the resolution. Be careful though, if you have a large terrain, some resolutions might be too much.\nTurn on 'use associated mesh' to apply the changes to the mesh if one is available.", MessageType.Warning);
            }           

        }
        else //nothing is selected
        {
            targetGO = null;
            EditorGUI.HelpBox(new Rect(10, 295, 280, 40), "Select a Unity Terrain to get started!", MessageType.Warning);
        }
	

	}

    

    private void HideTerrainMesh(MeshFilter[] mf)
    {
        foreach (MeshFilter m in mf)
            m.GetComponent<Renderer>().enabled = false;
    }

    private void ShowTerrainMesh(MeshFilter[] mf)
    {
        foreach (MeshFilter m in mf)
            m.GetComponent<Renderer>().enabled = true;
    }

    //starts rendering vertex colors
    public void RenderVertexColors(GameObject go)    
    {
        terrainObject.gameObject.SetActive(true);
        GameObject RT = new GameObject();
        RT.name = "TopDown Renderer - EraseMe!";
        RT.AddComponent<Camera>();
        QT_RenderBaseMap RBM = (QT_RenderBaseMap)RT.AddComponent<QT_RenderBaseMap>();
        RBM.RenderMap(terrainObject, terrainObject.terrainData, go, smoothVertColor);
   
    }
        
	private void OnInspectorUpdate()
	{
		Repaint();		
	}

    private Vector3[] StoreTerrainMeshTransforms()
    {
        Vector3[] t = new Vector3[3];
        t[0] = targetGO.transform.position;
        t[1] = targetGO.transform.localEulerAngles;
        t[2] = targetGO.transform.localScale;
        return t;
    }

    private Vector3[] GetTerrainTransform()
    {
        Vector3[] t = new Vector3[3];
        t[0] = terrainPos;
        t[1] = new Vector3(0, 90, 0);
        t[2] = new Vector3(1, 1, 1);
        return t;
    }


	void Export()
	{
        MeshName = terrainObject.name;

        string fileName;

        if (!usePrefab) //if this is a new file, export it to a location specified by the user. Get the transforms of the terrain and use it for later.
        {            
            fileName = EditorUtility.SaveFolderPanel("Choose Folder in which to Export to OBJ", "", "");
            if (fileName != "")            
                fileName = fileName + "/" + MeshName + "-Source.obj";

            
            //newPrefabInScene.transform.Rotate(new Vector3(0, 90, 0));
            // newPrefabInScene.transform.position = terrainPos
            tGO_PreviousTransform = GetTerrainTransform();
        }
        else //get the path of the prefab's mesh. This gets the OBJ and we should get the prefab meshes we made. 
            //OBJ should never be touched.
        {
             //store the transform of the targetGO for later.
            tGO_PreviousTransform = StoreTerrainMeshTransforms();

           //convert to windows path
            fileName = Application.dataPath;
            
            //choosing the object from the scene in the object field..
            string assetPath = AssetDatabase.GetAssetPath((GameObject)PrefabUtility.GetPrefabParent(targetGO));
            fileName = fileName.Replace("Assets", assetPath);
            fileName = fileName.Replace(".prefab", ".obj");

          //  AssetDatabase.DeleteAsset(assetPath);
          
        }
        
        fileName = fileName.Replace("-Scene", "-Source");

        #region exporting..
        if (fileName!="") //if we chose a folder and didn't hit cancel
        {
            
            int w = terrain.heightmapWidth;
            int h = terrain.heightmapHeight;
            Vector3 meshScale = terrain.size;
            int tRes = (int)Mathf.Pow(2, (int)saveResolution);
            meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
            Vector2 uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
            float[,] tData = terrain.GetHeights(0, 0, w, h);

            w = (w - 1) / tRes + 1;
            h = (h - 1) / tRes + 1;
            Vector3[] tVertices = new Vector3[w * h];
            Vector2[] tUV = new Vector2[w * h];

            int[] tPolys;

            if (saveFormat == SaveFormat.Triangles)
            {
                tPolys = new int[(w - 1) * (h - 1) * 6];
            }
            else
            {
                tPolys = new int[(w - 1) * (h - 1) * 4];
            }

            // Build vertices and UVs
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(x, tData[x * tRes, y * tRes], y));// +terrainPos;
                    tUV[y * w + x] = Vector2.Scale(new Vector2(x * tRes, y * tRes), uvScale);
                }
            }

            int index = 0;
            if (saveFormat == SaveFormat.Triangles)
            {
                // Build triangle indices: 3 indices into vertex array for each triangle
                for (int y = 0; y < h - 1; y++)
                {
                    for (int x = 0; x < w - 1; x++)
                    {
                        // For each grid cell output two triangles
                        tPolys[index++] = (y * w) + x;
                        tPolys[index++] = ((y + 1) * w) + x;
                        tPolys[index++] = (y * w) + x + 1;

                        tPolys[index++] = ((y + 1) * w) + x;
                        tPolys[index++] = ((y + 1) * w) + x + 1;
                        tPolys[index++] = (y * w) + x + 1;
                    }
                }
            }
            else
            {
                // Build quad indices: 4 indices into vertex array for each quad
                for (int y = 0; y < h - 1; y++)
                {
                    for (int x = 0; x < w - 1; x++)
                    {
                        // For each grid cell output one quad
                        tPolys[index++] = (y * w) + x;
                        tPolys[index++] = ((y + 1) * w) + x;
                        tPolys[index++] = ((y + 1) * w) + x + 1;
                        tPolys[index++] = (y * w) + x + 1;
                    }
                }
            }

            // Export to .obj
            StreamWriter sw = new StreamWriter(fileName);
            try
            {

                sw.WriteLine("# Unity terrain OBJ File");

                // Write vertices
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                counter = tCount = 0;
                totalCount = (tVertices.Length * 2 + (saveFormat == SaveFormat.Triangles ? tPolys.Length / 3 : tPolys.Length / 4)) / progressUpdateInterval;
                for (int i = 0; i < tVertices.Length; i++)
                {
                    UpdateProgress();
                    StringBuilder sb = new StringBuilder("v ", 20);
                    // StringBuilder stuff is done this way because it's faster than using the "{0} {1} {2}"etc. format
                    // Which is important when you're exporting huge terrains.
                    sb.Append(tVertices[i].x.ToString()).Append(" ").
                    Append(tVertices[i].y.ToString()).Append(" ").
                    Append(tVertices[i].z.ToString());
                    sw.WriteLine(sb);
                }
                // Write UVs
                for (int i = 0; i < tUV.Length; i++)
                {
                    UpdateProgress();
                    StringBuilder sb = new StringBuilder("vt ", 22);
                    sb.Append(tUV[i].x.ToString()).Append(" ").
                    Append(tUV[i].y.ToString());
                    sw.WriteLine(sb);
                }

               
                    StringBuilder q = new StringBuilder("g " + MeshName + "-BaseMesh");
                    
                    sw.WriteLine(q);

               
                if (saveFormat == SaveFormat.Triangles)
                {
                    // Write triangles
                    for (int i = 0; i < tPolys.Length; i += 3)
                    {
                        UpdateProgress();
                        StringBuilder sb = new StringBuilder("f ", 43);
                        sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                        Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                        Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1);
                        sw.WriteLine(sb);
                    }
                }
                else
                {
                    // Write quads
                    for (int i = 0; i < tPolys.Length; i += 4)
                    {
                        UpdateProgress();
                        StringBuilder sb = new StringBuilder("f ", 57);
                        sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                        Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                        Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1).Append(" ").
                        Append(tPolys[i + 3] + 1).Append("/").Append(tPolys[i + 3] + 1);
                        sw.WriteLine(sb);
                    }
                }
            }
            catch (Exception err)
            {
                Debug.Log("Error saving file: " + err.Message);
            }
            sw.Close();
        #endregion

            EditorUtility.ClearProgressBar();            
           QT_ModelProcessor.isFacetedMesh = true; 
            AssetDatabase.Refresh();
         QT_ModelProcessor.isFacetedMesh = false;
            SetupTerrainPrefab(fileName,terrain);
            terrain = null;
        }
				
	}
    //have to use .sharedmesh in edit mode, but i can't write them to disk..
    private Mesh[] GetUniqueMeshes(MeshFilter[] mf)
    {
        List<Mesh> newMesh = new List<Mesh>();

        for (int x = 0; x < mf.Length; x++)
        {
            Mesh nm = new Mesh();
            nm.vertices = mf[x].sharedMesh.vertices;
            nm.normals = mf[x].sharedMesh.normals;
            nm.tangents = mf[x].sharedMesh.tangents;
            nm.uv = mf[x].sharedMesh.uv;
            nm.uv2 = mf[x].sharedMesh.uv2;
            nm.uv2 = mf[x].sharedMesh.uv2;
            nm.name = mf[x].sharedMesh.name;
            Color32[] vcWhite = new Color32[mf[x].sharedMesh.vertexCount];
            for (int c = 0; c < vcWhite.Length; c++)
                vcWhite[c] = new Color32(1, 1, 1, 1);

            nm.colors32 = vcWhite;//mf[x].sharedMesh.colors32;
           // nm.colors = mf[x].sharedMesh.colors;
            nm.triangles = mf[x].sharedMesh.triangles;
            newMesh.Add(nm);
        }
        return newMesh.ToArray();
    }

    private bool CheckMeshCount(MeshFilter[] obj, MeshFilter[] prefabinScene)
    {
        bool isDifferent = false;
        if (obj.Length != prefabinScene.Length)
            isDifferent = true;
        return isDifferent;
    }

    void SetupTerrainPrefab(string assetPath, TerrainData terrain)
    {       
        //assetpath is the full windows path and the obj file.        
        string fullPath = assetPath;
        fullPath = fullPath.Replace(".obj",""); //fullpath is the WINDOWS path to the export folder. we chopped off the extention for ease of use 
        assetPath = assetPath.Replace(".obj", ""); //do it for thiso ne
        //get the assetpath to the obj we just exported. The obj has a -Source suffix.
        assetPath = assetPath.Replace(Application.dataPath, "Assets"); //assetPath is the Unity path to the export folder used in prefab functions        
        //store it as a go
        GameObject g = (GameObject)AssetDatabase.LoadAssetAtPath(assetPath+".obj", typeof(GameObject));
        
        //grab all the meshes in obj GO.
        MeshFilter[] mf = g.GetComponentsInChildren<MeshFilter>(true);
        //we will store the new ones we write to disk in this list, or we store the ones we found.
        List<Mesh> newMeshes = new List<Mesh>();
        //copy the source data meshes into new ones.
        Mesh[] oldMeshes = GetUniqueMeshes(mf);


        #region mesh count check
        //if the prefab instance exists in the scene.
        if(targetGO)
        {            
            //grab the meshfilters from the instance
            MeshFilter[] prefabMF = targetGO.GetComponentsInChildren<MeshFilter>(true);               
                //delete all the created meshes in the export folder
                for (int x = 0; x < prefabMF.Length; x++)
                {
                    AssetDatabase.DeleteAsset(assetPath +x.ToString() + ".asset");                   
                }
                //delete the prefab
                string prefabPath = assetPath;
                prefabPath=prefabPath.Replace("-Source", "-Scene");             
                AssetDatabase.DeleteAsset(prefabPath + ".prefab");
                DestroyImmediate(targetGO);
                AssetDatabase.Refresh();           
        }
        #endregion

       
        assetPath = assetPath.Replace("-Source", "-Scene");
        fullPath = fullPath.Replace("-Source", "-Scene");

        //writes the created meshes to disk, then loads them into a seperate array.
        for(int x=0;x<mf.Length;x++)
        {             
            AssetDatabase.CreateAsset(oldMeshes[x], assetPath + x.ToString() + ".asset");
            AssetDatabase.Refresh();
            //load it, then plug it into the list.  
            Mesh temp = (Mesh)AssetDatabase.LoadAssetAtPath(assetPath + x.ToString() + ".asset", typeof(Mesh));
     
            newMeshes.Add(temp);  
        }

        bool isHighPoly = false; //if a mesh is over 65536 verts, a warning from unity pops up. I'll pop up my own to let the user know this is ok.
        //bool isTerrainCollidable = true; 
        GameObject newPrefab,newPrefabInScene;
        //if there is no prefab file found in the export folder.. shouldn't be because we've just deleted it
        if (!File.Exists(fullPath + ".prefab"))
        {           
            //creat it          
            string newPrefabName = assetPath;
            newPrefabName = newPrefabName.Replace("-Source", "-Scene");

            newPrefab = PrefabUtility.CreatePrefab(newPrefabName+".prefab", g, ReplacePrefabOptions.Default);
            AssetDatabase.Refresh();
            //instantiate it
            newPrefabInScene = (GameObject)PrefabUtility.InstantiatePrefab(newPrefab);

            newPrefabInScene.transform.position = tGO_PreviousTransform[0];
            newPrefabInScene.transform.localEulerAngles = tGO_PreviousTransform[1];
            newPrefabInScene.transform.localScale = tGO_PreviousTransform[2];
            
            //rotate it to align with the unity terrain
            //newPrefabInScene.transform.Rotate(new Vector3(0, 90, 0));
           // newPrefabInScene.transform.position = terrainPos;

            //add the terrain collision information so we can walk and generate navmesh
            TerrainCollider tc = newPrefabInScene.AddComponent(typeof(TerrainCollider)) as TerrainCollider;
            tc.terrainData = terrain;
            //get the meshes referenced in the prefab
            MeshFilter[] mfInScene = newPrefabInScene.GetComponentsInChildren<MeshFilter>(true);
            //replace all of them with the new meshes we wrote to disk.
            for (int z = 0; z < mfInScene.Length; z++)
            {
                mfInScene[z].mesh = newMeshes[z];
                if (mfInScene[z].sharedMesh.vertexCount > 65530)
                    isHighPoly = true;
            }
            //replace the prefab
            PrefabUtility.ReplacePrefab(newPrefabInScene, newPrefab, ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.Refresh();
            if (autoUpdateVC && newPrefabInScene)
                RenderVertexColors(newPrefabInScene);
        }
        if(isHighPoly)
            Debug.LogWarning("Some meshes have a high vertex count which may be expensive to render on low-end hardware. Use the 'chunk mesh' function to divide it up.");
        if (!CheckValidTerrainCollision())
        {
            Debug.LogWarning("The faceted mesh prefab has rotation or scale values that will create invalid collision. Remove the terrain collider component from it and add a mesh collider instead.");
        }
        #region deprecated
        //otherwise, edit the instantiated prefab "targetgo." we'll need to overwrite the meshes too since exporting updates the OBJ.
        //this liekly never runs as we delete the prefab prior to getting here.
        /*else
        {
            Debug.Log("prefab found..");
            //if there is no targetGO in the scene, but we're here because there is a prefab in the export folder. Associate the targetgo with that prefab.
           if(!targetGO)
                targetGO = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(assetPath + ".prefab", typeof(GameObject)));
            
            newPrefabInScene = (GameObject)PrefabUtility.InstantiatePrefab((GameObject)PrefabUtility.GetPrefabParent(targetGO));

            MeshFilter[] mfInScene = newPrefabInScene.GetComponentsInChildren<MeshFilter>(true);
            //replace all of them with the new meshes we wrote to disk.
            for (int z = 0; z < mfInScene.Length; z++)
            {
                mfInScene[z].mesh = newMeshes[z];
            }
            //replace the prefab
            PrefabUtility.ReplacePrefab(newPrefabInScene, (GameObject)PrefabUtility.GetPrefabParent(targetGO),ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.Refresh();
            DestroyImmediate(newPrefabInScene);
        }
        */

        #endregion
    }

    //if the mesh has a X and Z rotation > 0 and/or a scale of > or < 1, you won't have true terrain collision.
    private bool CheckValidTerrainCollision()
    {
        bool c = true;
        if (tGO_PreviousTransform[1].x != 0 || tGO_PreviousTransform[1].z != 0)
            c = false;
        else if (tGO_PreviousTransform[2] != new Vector3(1, 1, 1))
            c = false;
        return c;
    }
	void UpdateProgress()
	{
		if (counter++ == progressUpdateInterval)
		{
			counter = 0;
			EditorUtility.DisplayProgressBar("Saving...", "", Mathf.InverseLerp(0, totalCount, ++tCount));
		}
	}
    
    //chops up the mesh into chunks. useful for occclusion.
    private void DivideMesh()
    {

        //get the meshfilters from the targetgo in the scene.
        MeshFilter[] meshFilters = targetGO.GetComponentsInChildren<MeshFilter>();
        ShowTerrainMesh(meshFilters);
        meshHidden = false;

        //stores references to the new meshes we're making.
        List<Mesh> tempMeshes = new List<Mesh>();        
        //the parent of the new mesh
        GameObject MainParent = (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.GetPrefabParent(targetGO));
       
        //set the position to 0 since the new meshes will be placed there... stupid exporter, then put it back after the meshes are associated
        MainParent.transform.position = Vector3.zero;
        //holds references to all the child gameobjects within the MainParent prefab, which is just an instance of targetGO
        List<Transform> children = new List<Transform>();
        //we're going to delete these 
        foreach (Transform child in MainParent.transform)        
            children.Add(child);
       
        //delete them to make room for the new game objects we're making in the big loop
         foreach (Transform child in children)
          DestroyImmediate(child.gameObject);

         //for every meshfilter we got from the targetGO initially in the scene..
         
         float tOffset = 0; //The cumulative terrain widths used to determine the locations of bound centers at the start of each mesh chop.
         //store the transforms
         tGO_PreviousTransform = StoreTerrainMeshTransforms();

        //reset transforms to identity so we chop correctly
         MainParent.transform.localScale = new Vector3(1, 1, 1);
         MainParent.transform.localEulerAngles = new Vector3(0, 90, 0);
         

         for (int a = 0; a < meshFilters.Length; a++) 
        {

           
            //store the mesh
            Mesh tMesh = new Mesh();
            tMesh = meshFilters[a].sharedMesh;
            Vector3[] verts = tMesh.vertices;           
             
           
            int[] tris = tMesh.triangles;   
            Vector3 ttExtents = tMesh.bounds.extents; //get the bounding size of the meshterrain
            float faceSize = GetFaceSize(tMesh);
            
            float tWidth = ttExtents.z * 2;      //extents are half the size of the terrain	
            float tLength = ttExtents.x * 2;
            float tHeight = terrain.heightmapHeight*2;
        
           
            float newBoundSize = (float)(faceSize * chunkVal[chunkValUI - 1]);
            float adjBoundSize = (float)(faceSize * chunkVal[chunkValUI - 1]) + (faceSize / 2);//adding half a facesize for slop
            
   
             //store the previous mesh's z extent to apply it to the current
         
             if (a > 0)             
                tOffset += ((meshFilters[a - 1].sharedMesh.bounds.extents.z) * 2)-faceSize;

            //create a bound at 0,0,0 with the right size and height for chopping.
            Bounds bound = new Bounds(Vector3.zero, new Vector3(adjBoundSize, tHeight, adjBoundSize));
         
            //positioin it at the correct first chopping position.             
             //if A>0, the bound center has to have an X offset of the SUM OF ALL the previous mesh's twidth
            if (a > 0)
            {
                //offset it by the extents of previous meshes.
                bound.center += new Vector3(0, 0, tOffset);  
                //now align it to start.
                bound.center += new Vector3((newBoundSize * -1) / 2, terrain.heightmapHeight/2, (newBoundSize) / 2);
            }
            else
                bound.center = new Vector3((newBoundSize * -1) / 2, terrain.heightmapHeight / 2, (newBoundSize) / 2);

            //determine hwo many times this bound should move across the terrain           
            int TimestoMoveX = (int)(tLength / newBoundSize);
            int TimestoMoveZ = (int)(tWidth / newBoundSize);

            Vector3 initialPos = bound.center;

            //store all the temp meshes created	            
            List<int> triIndices = new List<int>();

            //store the vertex indices of what we need to capture
            List<Vector3> targetVertices = new List<Vector3>();
            List<int> originalIndices = new List<int>();

            #region chopping
            for (int z = 0; z <= TimestoMoveZ; z++)
            {
                for (int x = 0; x <= TimestoMoveX; x++)
                {
                    
                    targetVertices.Clear(); //holds the vertices found within the bounds
                    triIndices.Clear(); //refernces in the indices to those vertices.
                    originalIndices.Clear(); //holds references to the original indices, but who indices are relative to the ones we need.

                    Vector2[] newUVs;
                    Vector4[] newTangents;
                    Color32[] newColors;

                    for (int t = 0; t < tris.Length; t += 3) //iterate over tris because vert numbers can be all over the place.
                    {
                        Vector3 v1 = verts[tris[t]];
                        Vector3 v2 = verts[tris[t + 1]];
                        Vector3 v3 = verts[tris[t + 2]];

                        if (bound.Contains(v1))		 // if the vert's location is within the bounding area		
                        {
                            if (bound.Contains(v2))
                            {
                                if (bound.Contains(v3))
                                {
                                    
                                    targetVertices.Add(v1);
                                    originalIndices.Add(tris[t]);
                                    triIndices.Add(targetVertices.Count - 1);
                                    targetVertices.Add(v2);
                                    originalIndices.Add(tris[t + 1]);
                                    triIndices.Add(targetVertices.Count - 1);
                                    targetVertices.Add(v3);
                                    originalIndices.Add(tris[t + 2]);
                                    triIndices.Add(targetVertices.Count - 1);
                                }
                            }
                        }
                    }

                    newUVs = SetupUVs(originalIndices, tMesh.uv);
                    newTangents = SetupTangents(originalIndices, tMesh.tangents);
                    newColors = SetupVColors(originalIndices, tMesh.colors32);

                    Mesh newMesh = new Mesh();                    
                    newMesh.vertices = targetVertices.ToArray();
                    newMesh.uv = newUVs;
                    newMesh.tangents = newTangents;
                    newMesh.colors32 = newColors;
                    newMesh.RecalculateBounds();
                    newMesh.triangles = triIndices.ToArray();
					Unwrapping.GenerateSecondaryUVSet(newMesh);
                    newMesh.RecalculateBounds();
                    newMesh.RecalculateNormals();
                    tempMeshes.Add(newMesh);
                    //lastly, move the bound to the next cell                    
                    bound.center -= new Vector3(newBoundSize, 0, 0);
                   
                }
                //since we're done on the x axis, move up one on the z and continue..               
                bound.center = new Vector3(initialPos.x, 0, bound.center.z + newBoundSize);
            }

           
         }
#endregion

        
            string rootName = targetGO.name.Replace("-Scene", "");
           
            MeshRenderer[] OldMRs = targetGO.GetComponentsInChildren<MeshRenderer>();
            List<GameObject> meshGOs = new List<GameObject>();

            //get assetpath with base name           
            string assetPath = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(targetGO));
            string baseassetPath = assetPath.Replace("/" + targetGO.name + ".prefab", "");
            string baseName = targetGO.name.Replace("-Scene", "");

            string windowsPath = Application.dataPath;
            windowsPath = windowsPath.Replace("Assets", assetPath);
            windowsPath = windowsPath.Replace(targetGO.name + ".prefab", "");

            //final path is the final windows path of where the chunks go
            string finalPath = windowsPath + baseName + "-Chunks";           

            if (!Directory.Exists(finalPath))
            {
                Debug.LogWarning("Chunk directory not found. Making.");
                AssetDatabase.CreateFolder(baseassetPath, baseName + "-Chunks");
                AssetDatabase.Refresh();
            }
            else
                Debug.LogWarning("Chunk directory found.");


            //assetpath is now the final unity-based path of where the chunks go
            assetPath = baseassetPath + "/" + baseName + "-Chunks";
            
            //filter out any false meshes created by non-square terrains. They happen when unity divides up a mesh that's greater than 65536 tris, so we have to handle it.
    
        List<Mesh> badMeshes = new List<Mesh>();

            for (int b = 0; b < tempMeshes.Count; b++)
            {
                if (tempMeshes[b].vertexCount == 0)                
                    badMeshes.Add(tempMeshes[b]);                
            }

            for (int n = 0; n < badMeshes.Count; n++)            
                tempMeshes.RemoveAt(tempMeshes.IndexOf(badMeshes[n]));
            
            //by the tiem we get here, the meshes are setup in the tempMeshes array to be written to disk
            //create a gameobject per mesh. 
            //add the basic components
            //add the correct mesh, name the mesh correctly
            //save it to a folder of targetgo.name+chunks
            //plug in mesh to meshfilter
           
            //setup a gameobject per mesh with the right components, naming, parent, etc.
            for (int q = 0; q < tempMeshes.Count; q++)
            {
                //write the mesh to disk
                AssetDatabase.CreateAsset(tempMeshes[q], assetPath + "/" + rootName + "-Chunk" + q + ".asset");
                //add a new go per mesh..
                meshGOs.Add(new GameObject());
                meshGOs[q].name = rootName + "-Chunk" + q;
                MeshFilter mf = meshGOs[q].AddComponent<MeshFilter>();
                MeshRenderer mr = meshGOs[q].AddComponent<MeshRenderer>();
                mr.GetComponent<Renderer>().material = OldMRs[0].GetComponent<Renderer>().sharedMaterial;
                //apply the mesh
                mf.mesh = tempMeshes[q];
                //change the name
                meshGOs[q].name = targetGO.name + "-Chunk" + q;
                //parent it to the mainparent
                meshGOs[q].transform.parent = MainParent.transform;

            }

            foreach (GameObject g in meshGOs)
                g.transform.localRotation = Quaternion.identity;
           
            MainParent.transform.position = tGO_PreviousTransform[0];
            MainParent.transform.localEulerAngles = tGO_PreviousTransform[1];
            MainParent.transform.localScale = tGO_PreviousTransform[2];

            PrefabUtility.ReplacePrefab(MainParent, PrefabUtility.GetPrefabParent(targetGO), ReplacePrefabOptions.ConnectToPrefab);
            DestroyImmediate(targetGO);
        }

     
    private float GetFaceSize(Mesh m)
    {

        //grab all important mesh data
        Vector3[] verts = m.vertices;
        int[] tris = m.triangles;
        Vector3[] newverts = new Vector3[3];
        int[] newInd = new int[3];
        float[] vertDistances = new float[3];
        float size = 0;
        //setup the index for a single triangle, flatten it on the y.
        for (int x = 0; x < newverts.Length; x++)
        {
            newverts[x] = verts[tris[x]];
            newInd[x] = x;
            newverts[x].y = 0f;
        }
        vertDistances[0] = Vector3.Distance(newverts[0], newverts[1]);  //2
        vertDistances[1] = Vector3.Distance(newverts[0], newverts[2]);  //2
        vertDistances[2] = Vector3.Distance(newverts[1], newverts[2]);  //3

        //smallest distance = length. Longest = hypoteneuse
        size = Mathf.Min(vertDistances[0], Mathf.Min(vertDistances[1], vertDistances[2]));
        /*
        for (int y = 0; y < 2;y++)//vertDistances.Length; y++)
        {
            if (vertDistances[y] <= vertDistances[y + 1])
            {
                size = vertDistances[y];
                break;
            }
        }
         * */

        return size;
    }

    private Color32[] SetupVColors(List<int> oI, Color32[] vcolors)
    {
        List<Color32> TempC = new List<Color32>();
        for (int x = 0; x < oI.Count; x++)
        {
            Color32 vc = vcolors[oI[x]];
            TempC.Add(vc);
        }

        return TempC.ToArray();
    }
    private Vector4[] SetupTangents(List<int> oI, Vector4[] tangents)
    {
        List<Vector4> TempTs = new List<Vector4>();
        for (int x = 0; x < oI.Count; x++)
        {
            Vector4 t = tangents[oI[x]];
            TempTs.Add(t);
        }

        return TempTs.ToArray();
    }

    private Vector2[] SetupUVs(List<int> oI, Vector2[] uvs)
    {
        List<Vector2> TempUVs = new List<Vector2>();
        for (int x = 0; x < oI.Count; x++)
        {
            Vector2 u = uvs[oI[x]];
            TempUVs.Add(u);
        }

        return TempUVs.ToArray();
    }

	private static string GetAssetPath()
	{
		string basePath = AssetDatabase.GetAllAssetPaths().First(p => p.EndsWith("QT_ConvertMesh.cs"));
		int lastDelimiter = basePath.LastIndexOf('/') + 1;
		basePath = basePath.Remove(lastDelimiter, basePath.Length - lastDelimiter);
		return basePath;
    }

}