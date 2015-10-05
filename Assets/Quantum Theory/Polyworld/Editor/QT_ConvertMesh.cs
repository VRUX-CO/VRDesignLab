/*
 * QT_ConvertMesh v1.71
 * 
 * Known Issues:
 * - Unity doesn't give access to blendshape mesh data. Blendshapes can not be converted until support is added: http://feedback.unity3d.com/suggestions/expose-blend-shape-vertex-data
 * - as a result, gameobjects with skinnesmeshrender components that have blendshapes but no boneweights get converted to simple props (meshrenderer and meshfilter component-based). 
 * - Prefab hierarchies which contain meshes of the same name will create problems when "Overwrite Exported Data" is checked. 

*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
public class QT_ConvertMesh : EditorWindow
{
    [MenuItem("Window/Quantum Theory/PolyWorld Mesh Converter")]

    static void Init()
    {
        GUIContent gcw = new GUIContent("PolyWorld Mesher");
        QT_ConvertMesh window = (QT_ConvertMesh)EditorWindow.GetWindow(typeof(QT_ConvertMesh));
        window.titleContent = gcw;
        window.maxSize = new Vector2(300, 305);
        window.minSize = window.maxSize;
        window.Show();
        
    }


    private GameObject[] SourceGOs;
    private Color brightenColor = Color.black;
    private string HelpMessage;
    
    Texture WorldIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Quantum Theory/Polyworld/Editor/QT_PolyWorld-icon.png", typeof(Texture));


    private bool filterBilinear = true;
    enum BlurAmount { None, Some, Full }
    BlurAmount blurAmount = BlurAmount.None;
    int mipLevel = 0;
    private string exportAssetFolder = "Assets"; //contains the folder where we'll export all the assets
    private string exportWindowsFolder = Application.dataPath;//contains complete path to the exportAssetFolder
    private string tempExportFolder = "Assets";
    private bool overwriteMeshes = true; //if meshes of the same name are found, overwrite them.
    private bool recalcLMUVs = false; //useful for when multiple meshes get combined.
    private bool combineSubMeshes = true; //combines the submeshes of single gameobjects into one. 
    private string meshSuffix = "-Faceted"; //adds this suffix to the end of the data names


    void UpdateProgress(int totalCount,int currentGOIndex,string currentGO)
    {
        EditorUtility.DisplayProgressBar("Processing...", "Converting "+currentGO, Mathf.InverseLerp(0, totalCount, currentGOIndex));
    }


    public void OnGUI()
    {
        EditorGUI.DrawPreviewTexture(new Rect(10, 10, 280, 60), WorldIcon);
        GUILayout.BeginArea(new Rect(10, 70, 280, 220));
        if (Selection.gameObjects.Length > 0)
        {
            SourceGOs = Selection.gameObjects;

            if (isRootSelected() == false)
                HelpMessage = "Mesh conversion will only work properly if the root of all selected prefabs is selected. Do not select children.";
            else
            {
                HelpMessage = "Brighten by Color adds the color to the final vertex colors of all selected GameObjects. Use the Color swatch in the material change the color as well. Combine Materials will apply one new material to the each new prefab. Alternate Coloring applies Bilinear Filtering. Blur Amount selects a mipmap within the diffuse texture chain.";

                if (SourceGOs.Length == 1)
                    GUILayout.Label("GameObject Selected: " + SourceGOs[0].name);
                else
                    GUILayout.Label("GameObject Selected: Multiple");
                brightenColor = EditorGUILayout.ColorField("Brighten by Color: ", brightenColor, null);
                blurAmount = (BlurAmount)EditorGUILayout.EnumPopup("Blur: ", blurAmount);
                filterBilinear = EditorGUILayout.Toggle("Alternate Coloring", filterBilinear);
                
                EditorGUILayout.Separator();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Export Folder:");

                //make the string fit..     

                if (exportAssetFolder.Length >= 13)
                    tempExportFolder = exportAssetFolder.Remove(11, exportAssetFolder.Length - 11) + "..";
                else
                    tempExportFolder = exportAssetFolder;
                GUILayout.TextArea(tempExportFolder, 13);
                if (GUILayout.Button("..."))
                {
                    exportWindowsFolder = EditorUtility.SaveFolderPanel("Export Data", exportAssetFolder, "");
                    exportAssetFolder = exportWindowsFolder.Replace(Application.dataPath, "Assets");
                    if (exportAssetFolder.Equals(""))
                        exportAssetFolder = "Assets";
                    else if (!exportAssetFolder.Contains("Assets"))
                        exportAssetFolder = "Assets";
                    AssetDatabase.Refresh();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Suffix:");
                meshSuffix = GUILayout.TextField(meshSuffix, 10);


                EditorGUILayout.EndHorizontal();
                overwriteMeshes = EditorGUILayout.Toggle("Overwrite Exported Data", overwriteMeshes);
                recalcLMUVs = EditorGUILayout.Toggle("Recalculate Lightmap UVs", recalcLMUVs);
                combineSubMeshes = EditorGUILayout.Toggle("Combine SubMeshes", combineSubMeshes);

                if (GUILayout.Button("Convert to PolyWorld Mesh"))
                {

                    List<GameObject> badGOs = new List<GameObject>(); //stores a list of selected gameobjects that have no mesh. Just for error checking.
                    List<string> badMeshNames = CheckTriLimit(SourceGOs);
                    List<string> badMats = CheckMaterials(SourceGOs);
                    List<string> badTerrains = CheckforTerrain(SourceGOs);
                    List<string> notPrefab = CheckforPrefab(SourceGOs);

                    if (exportAssetFolder.Equals("Assets"))
                        EditorUtility.DisplayDialog("Invalid Folder", "Please do not export data into the root assets folder. Create a subfolder and export into that.", "OK");
                    else if (badMats.Count > 0)
                        EditorUtility.DisplayDialog("Materials Error", "Material verification failed. Please check the debug console window for details.\n\nAborting.", "OK");
                    else if (badTerrains.Count > 0)
                        EditorUtility.DisplayDialog("Terrains in Selection", "There is a terrain in the selection. This script will not convert terrains. Use the PolyWorld Terrain script located in Window->Quantum Theory->PolyWorld Terrain.", "OK");
                    else if (notPrefab.Count > 0)
                    {
                        EditorUtility.DisplayDialog("Not a Prefab", "Some GameObjects in the selection are not a prefab. Create a prefab first, then run the conversion script.\n\nCheck the Console for the Gameobjects that are not prefabs.", "OK");
                        foreach (string s in notPrefab)
                            Debug.LogError("GameObject named " + s + " is not a prefab.");
                    }
                    else
                    {
                        if (badMeshNames.Count == 0)
                        {

                            foreach (GameObject g in SourceGOs) //for every GO you have selected
                            {
                                GameObject WorkingGO = (GameObject)(PrefabUtility.InstantiatePrefab((GameObject)PrefabUtility.GetPrefabParent(g)));//instantiate it and work on it.                                
                                WorkingGO.name = WorkingGO.name.Replace("(Clone)", "");
                                MeshRenderer[] MRs = WorkingGO.GetComponentsInChildren<MeshRenderer>(true); //get all the MR components in the children.
                                MeshFilter[] MFs = WorkingGO.GetComponentsInChildren<MeshFilter>(true);
                                SkinnedMeshRenderer[] SMRs = WorkingGO.GetComponentsInChildren<SkinnedMeshRenderer>(true);//get all the SMR components in the children.
                                //now get all the gameobjects with meshes in it                          
                                GameObject[] MeshGOs = GetMeshGOs(MRs, SMRs);
                                if (MeshGOs.Length > 0)
                                    ConvertMesh(WorkingGO, MeshGOs, MFs, MRs, SMRs);
                                else
                                    badGOs.Add(WorkingGO);
                                GameObject.DestroyImmediate(WorkingGO);
                            }
                            EditorUtility.ClearProgressBar();
                            EditorUtility.DisplayDialog("Conversion Complete", "Success! All new content is located in:\n" + exportAssetFolder, "OK");

                       }
                        else
                        {
                            EditorUtility.DisplayDialog("Vertex Count Exceeded", "After conversion, the vertex count for certain meshes in the selection will exceed the amount allowed by Unity (65536). Please either reduce the triangle count on the model, or break it apart into seperate meshes.\n\nThe debug console will now output which meshes are too high poly.\n\nConversion aborted.", "OK");
                            foreach (string s in badMeshNames)
                                Debug.LogWarning(s + " has too many vertices\t. Please reduce the triangle count or break it apart into seperate meshes.");

                        }
                    }
                    if (badGOs.Count > 0)
                    {
                        EditorUtility.DisplayDialog("Meshes Not Found", "Some GameObjects in the selection had no meshes. Check the debug log. ", "OK");
                        foreach (GameObject g in badGOs)
                            Debug.LogWarning(g.name + " does not have a Mesh Filter or Skinned Mesh Filter, nor does it have children with these components, and did not get converted.");
                    }
                }
                if (GUILayout.Button("Help"))
					Application.OpenURL("http://qt-ent.com/PolyWorld/scripts/");
            }
            GUILayout.EndArea(); 
        }
        else
        {
            GUILayout.EndArea();        
            HelpMessage = "In the Scene View, select the character or prop prefab that has a diffuse texture assigned to it. This script converts the prefab parent of the selected objects. Multiple objects are supported.";
            EditorGUI.HelpBox(new Rect(10, 115, 280, 60), HelpMessage, MessageType.Info);
        }


 
       
    }
    private List<string> CheckforPrefab(GameObject[] SourceGOs)
    {
        List<string> badGOs = new List<string>();

        foreach (GameObject g in SourceGOs)
        {
            GameObject p = (GameObject)PrefabUtility.GetPrefabParent(g);
            if (p == null)
                badGOs.Add(g.name);
        }
        return badGOs;
    }
    //checks to see if any of the materials are using _Maintex and _Color.
    private List<string> CheckMaterials(GameObject[] SourceGOs)
    {
        List<string> badMats = new List<string>();
        //these properties exist in the material behind the scenes. Maybe check the shader if it's using them.

        foreach (GameObject g in SourceGOs) //for every GO you have selected
        {
            MeshRenderer[] mrs = g.GetComponentsInChildren<MeshRenderer>(true);
            foreach (MeshRenderer mr in mrs)
            {               
                    Material[] m = mr.sharedMaterials;
                    foreach (Material mat in m)
                    {
                        if (!mat.HasProperty("_Color") || !mat.HasProperty("_MainTex"))
                        {
                            badMats.Add(mat.name);
                            Debug.LogError("Shader assigned to material: "+mat.name+"    GameObject: "+mr.gameObject.name+"    does not have a _Color or _MainTex property. \nUse a standard diffuse shader as a workaround. Aborting.");
                        }
                        else if (!mat.GetTexture("_MainTex"))
                        {
                            badMats.Add(mat.name);
                            Debug.LogError("No diffuse texture specified in material: " + mat.name + "    GameObject: "+mr.gameObject.name+". \nAdd the texture named Swatch_White to the diffuse slot as a workaround. Aborting.");
                        }
                        
                    }
                
            }

            SkinnedMeshRenderer[] smrs = g.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                Material[] m = smr.sharedMaterials;
                foreach (Material mat in m)
                {
                    if (!mat.HasProperty("_Color") || !mat.HasProperty("_MainTex"))
                    {
                        badMats.Add(mat.name);
                        Debug.LogError("Shader assigned to material: " + mat.name + "    GameObject: " + smr.gameObject.name + "    does not have a _Color or _MainTex property. \nUse a standard diffuse shader as a workaround. Aborting.");
                    }
                    else if (!mat.GetTexture("_MainTex"))
                    {
                        badMats.Add(mat.name);
                        Debug.LogError("No diffuse texture specified in material: " + mat.name + "    GameObject: " + smr.gameObject.name + ". \nAdd the texture named Swatch_White to the diffuse slot as a workaround. Aborting.");
                    }
                }

            }
        }
        return badMats;
    }
    
    private string CheckBadCharacters(string sourceString)
    {
        string revisedName = sourceString;
        string[] badChars = { "//", "?", "<", ">", "\\", ":", "*", "|", "\"" };

        foreach (string c in badChars)
        {
            if (sourceString.Contains(c))
            {
                revisedName = revisedName.Replace(c, "");
                Debug.LogWarning("Illegal characters in a gameobject or mesh name detected during conversion. Removing them..");
            }
        }
        return revisedName;
    }

    //checks to see if you actually selected the root of all prefabs
    private bool isRootSelected()
    {
        bool isRoot = true;
        foreach (GameObject g in SourceGOs)
        {           
            if (g.transform.parent != null)            
                isRoot = false;            
        }
        return isRoot;
    }
    //gets all the gameobjects with meshes in it
    private GameObject[] GetMeshGOs(MeshRenderer[] MRs, SkinnedMeshRenderer[] SMRs)
    {
        List<GameObject> GOs = new List<GameObject>();
        foreach (MeshRenderer m in MRs)
            GOs.Add(m.gameObject);
        foreach (SkinnedMeshRenderer s in SMRs)
            GOs.Add(s.gameObject);
        return GOs.ToArray();
    }

    //checks all the meshes to make sure the result of conversion will be within the vertex limit in unity, currently 65536 verts.
    private List<string> CheckTriLimit(GameObject[] SourceGOs)
    {
        List<string> meshNames = new List<string>();

        foreach (GameObject g in SourceGOs)
        {
            MeshFilter[] m = g.GetComponentsInChildren<MeshFilter>();
            if (m.Length > 0)
            {
			
                foreach (MeshFilter mf in m)
				{	

                    if (mf.sharedMesh.triangles.Length > 64999)
                    {
                        meshNames.Add(mf.sharedMesh.name);
                        break;
                    }
                }
            }
            SkinnedMeshRenderer[] s = g.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (s.Length > 0)
            {
                foreach (SkinnedMeshRenderer smr in s)
                {
					if (smr.sharedMesh.triangles.Length > 64999)
                    {
                        meshNames.Add(smr.sharedMesh.name);
                        break;
                    }
                }
            }            
        }
        return meshNames;
    }

    private List<string> CheckforTerrain(GameObject[] SourceGOs)
    {
        List<string> GONames = new List<string>();
        foreach (GameObject g in SourceGOs)
        {
            Terrain[] terrains = g.GetComponentsInChildren<Terrain>(true);
            if (terrains.Length > 0)
                GONames.Add(g.name);
        }
        return GONames;
    }

    //global is just easier.
    private List<Vector3> FinalVerts = new List<Vector3>();
    private List<Vector2> FinalUVS = new List<Vector2>();
    private List<Color32> FinalVCs = new List<Color32>();
    private List<int> FinalTris = new List<int>();
    //the next two are for skinned meshes. Bone weights and bind poses.
    private List<BoneWeight> FinalBWs = new List<BoneWeight>();
    private List<Matrix4x4> FinalBPs = new List<Matrix4x4>();

    private Mesh newVCMesh;
    private int filenameCounter = 0; // counter used when scanning export folders for the same files. It increments when a file is found and cannot be overwritten as flagged in the options.

    //here is where the work is done.    
    private void ConvertMesh(GameObject WorkingGO, GameObject[] MeshGOs, MeshFilter[] MFs, MeshRenderer[] MRs, SkinnedMeshRenderer[] SMRs)
    {
       
        //just a set of flags for which go's are skinnedmeshes. it may be possible to have a mix i think
        bool[] isSkinnedMesh = new bool[MeshGOs.Length];

        //string folderPath = GetFolderPath();
        Material finalMat = CreateMasterMaterial(exportAssetFolder, WorkingGO.name + meshSuffix+"_Mat"); //create one material for the entire GO. Really, anything vertex colored can use one material..
        
        int totalCount = MeshGOs.Length;

        int SMRIndex = 0; //stores the current index to the SMR GOs
        //for every gameobject child with a mesh in it
        for (int x = 0; x < MeshGOs.Length; x++)       
        {
            AssetDatabase.Refresh();
            FinalVerts.Clear();
            FinalUVS.Clear();
            FinalVCs.Clear();
            FinalTris.Clear();
            FinalBWs.Clear();
            FinalBPs.Clear();
        
            isSkinnedMesh[x] = isGOSkinnedMesh(MeshGOs[x]);
            
            newVCMesh = new Mesh();
            
            string currentMeshName = "";

            if (isSkinnedMesh[x])
            {
                currentMeshName = SMRs[SMRIndex].sharedMesh.name;
                if (SMRs[SMRIndex].sharedMesh.blendShapeCount > 0 && SMRs[SMRIndex].sharedMesh.boneWeights.Length == 0) //can't access blendshape mesh data in Unity 4.5, so we have to convert it to a simpleprop.
                {
                    Debug.LogWarning("Warning: Gameobject" + MeshGOs[x].name + " has a mesh with blend shapes. Blend Shapes cannot be duplicated to the faceted version until there is support for accessing that data in Unity. This gameobject will use MeshFilter and MeshRenderer components.");
                   newVCMesh=RenderSimpleProp(SMRs[SMRIndex], MeshGOs[x]);
                }
                else
                {
                    if (SMRs[SMRIndex].sharedMesh.blendShapeCount > 0)
                        Debug.LogWarning("Warning: Gameobject" + MeshGOs[x].name + " has a mesh with blend shapes. Blend Shapes cannot be duplicated to the faceted version until there is support for accessing that data in Unity.");
                    newVCMesh = RenderSkinnedMesh(SMRs[SMRIndex], MeshGOs[x]);
                }
                SMRIndex++;
            }
            else
            {
                currentMeshName = MFs[x].sharedMesh.name;
                newVCMesh = RenderSimpleProp(MFs[x], MRs[x], MeshGOs[x]);
            }   
         
           /* newVCMesh.vertices = FinalVerts.ToArray();
            newVCMesh.uv = FinalUVS.ToArray();
            newVCMesh.colors32 = FinalVCs.ToArray();
            newVCMesh.triangles = FinalTris.ToArray();
            if (isSkinnedMesh[x])
            {
                newVCMesh.boneWeights = FinalBWs.ToArray();
                newVCMesh.bindposes = FinalBPs.ToArray();
            }
            * */
            if(recalcLMUVs)
                Unwrapping.GenerateSecondaryUVSet(newVCMesh);
            newVCMesh.Optimize();
            newVCMesh.RecalculateNormals(); //recalc normals breaks normals along uv seams.. 

            //remove bad mesh characters here and add suffix
            newVCMesh.name = SetupMeshNaming(currentMeshName);
            newVCMesh.RecalculateBounds();           
            //write meshes to disk.
            
            CreateMesh(newVCMesh, exportAssetFolder);
            AssetDatabase.Refresh();
            //plug in the new mesh to the current GO in the prefab hierarchy.
            MeshGOs[x] = UpdateMeshGO(MeshGOs[x], newVCMesh, finalMat, isSkinnedMesh[x]);
            UpdateProgress(totalCount,x,MeshGOs[x].name);
        }   
        //All done making meshes. Update the prefab or create a new one.
        UpdatePrefab(WorkingGO, MeshGOs, isSkinnedMesh, exportAssetFolder);
    }

    private string SetupMeshNaming(string currentName)
    {
        //first remove any illegal characters from the mesh
        string tempName = CheckBadCharacters(currentName) + meshSuffix;
        currentName = tempName;

        string[] files = Directory.GetFiles(exportWindowsFolder);

        //if overwrite meshes is false (we will add new data)
        if (!overwriteMeshes)
        {
            filenameCounter = 0;
            //first find if any similar names exist in the folder


            for (int x = 0; x < files.Length; x++)
            {

                if (!files[x].Contains("_Mat") && !files[x].Contains(".meta") && !files[x].Contains("prefab"))
                {
                    //must be a mesh we're looking at in files[x] then. 
                    if (files[x].Contains(tempName))
                    {
                        tempName = currentName + filenameCounter;
                        filenameCounter++;
                        x = 0;
                    }
                }
            }

        }
       
        filenameCounter--; //we decrease this to keep in synch with the prefab that will be made.
        string newName = CheckBadCharacters(tempName);
        return newName;

    }

    private GameObject UpdateMeshGO(GameObject MeshGO, Mesh newVCMesh, Material finalMat, bool isSkinnedMesh)
    {
        SkinnedMeshRenderer SMR;
        MeshRenderer MR;
        MeshFilter MF;

        Material[] fMat = new Material[newVCMesh.subMeshCount];
        for(int x=0;x<fMat.Length;x++)
            fMat[x] = finalMat;

        if (isSkinnedMesh)
        {
            SMR = MeshGO.GetComponent<SkinnedMeshRenderer>();
            if (SMR.sharedMesh.blendShapeCount > 0 && SMR.sharedMesh.boneWeights.Length == 0) //sloppiness ahead. If Unity gave access to blendshapes I wouldn't have to do this.
            {
                //delete the skinnedmeshrenderer component and just make it a standard mesh.
                DestroyImmediate(SMR);
                MR = MeshGO.AddComponent<MeshRenderer>();
                MF = MeshGO.AddComponent<MeshFilter>();               
                MR.sharedMaterials = fMat;
                MF.sharedMesh = newVCMesh;
            }
            else
            {
                SMR.sharedMesh = newVCMesh;
                SMR.sharedMaterials = fMat;
            }
        }
        else
        {
            MR = MeshGO.GetComponent<MeshRenderer>();
            MF = MeshGO.GetComponent<MeshFilter>();
            MR.sharedMaterials = fMat;
            MF.sharedMesh = newVCMesh;
        }
        return MeshGO;

    }
    //checks to see if the GO is a skinned mesh.
    private bool isGOSkinnedMesh(GameObject MeshGO)
    {
        bool isSkinnedMesh = false;

        SkinnedMeshRenderer SMR = MeshGO.GetComponent<SkinnedMeshRenderer>();
        if (SMR != null)        
            isSkinnedMesh = true;        
        return isSkinnedMesh;
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
    private string GetFolderPath()
    {
        string guid, newFolderPath;
        // List<GameObject> newGOs = new List<GameObject>(); //holds refs to the new gameobjects we're making

        //setup the folder. check if it exists. if it doesn't, make it.
        string windowsAssetsPath = Application.dataPath;
        string windowsFacetedPath = windowsAssetsPath + "/Faceted Meshes";

        if (!Directory.Exists(windowsFacetedPath))
        {
            guid = AssetDatabase.CreateFolder("Assets", "Faceted Meshes");
            newFolderPath = AssetDatabase.GUIDToAssetPath(guid);
        }
        else
            newFolderPath = "Assets/Faceted Meshes";

        return newFolderPath;
    }

    private Mesh RenderSimpleProp(MeshFilter MF, MeshRenderer MR, GameObject currentGO)
    {
        //make a copy of the source mesh
        Mesh sourceMesh = MF.sharedMesh;
        Vector3[][] finalSubMeshVerts = new Vector3[sourceMesh.subMeshCount][];
        Vector2[][] finalSubMeshUVs = new Vector2[sourceMesh.subMeshCount][];
        Color32[][] finalSubMeshVCs = new Color32[sourceMesh.subMeshCount][];
        
        //for every submesh, do all the work.
        for (int x = 0; x < sourceMesh.subMeshCount; x++)
        {

            List<Vector3> SMVerts = new List<Vector3>();
            List<Vector2> SMUVs = new List<Vector2>();
            List<Color32> SMVCs = new List<Color32>();
          
            //triangle arrays point to the array index of the vertices in the vertex array
            int[] triList = sourceMesh.GetTriangles(x); //submesh's index number corresponds to the material index of the meshrendere component
            //get the diffuse texture. Hopefully the shader obeys the standard naming convention..

            //doesn't work with substances or probably square textures..
            string path = AssetDatabase.GetAssetPath(MR.sharedMaterials[x].GetTexture("_MainTex"));            
            TextureImporter A = (TextureImporter)AssetImporter.GetAtPath(path);
            A.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Texture2D tex = (Texture2D)MR.sharedMaterials[x].GetTexture("_MainTex");
            Color matColor = MR.sharedMaterials[x].GetColor("_Color");

            //Coloring will not work with nonsquare textures.

            //we only want to go as far as 32x32
            int mipAmount = tex.mipmapCount - 4;
            mipLevel = PickMipLevel(tex, mipAmount);

            if (mipLevel > 0) //if we chose to do blur amount..
            {
                int mipSize = (int)Mathf.Pow(2f, (mipAmount - (mipLevel - 1)) + 4); //+4 since we are raising 2 to the x power.
                Color[] mcs = tex.GetPixels(0, 0, mipSize, mipSize, mipLevel - 2);      
                tex = new Texture2D(mipSize, mipSize, TextureFormat.ARGB32, false);
                tex.SetPixels(mcs);
            }
        

            //to facet the submesh, go through each triangle. get each vertex, add it to an new vertex array. do same with uvs and boneweights

            for (int t = 0; t < triList.Length; t++)
            {
                //add the vertex to the new array.
                SMVerts.Add(sourceMesh.vertices[triList[t]]);
                SMUVs.Add(sourceMesh.uv[triList[t]]);
            }

            

            //make vertex colors from the diffuse but using the uvs and verts of the mesh we're making   
            //second time around, vert length is already b
            for (int z = 0; z < SMVerts.Count; z++)
            {
                
                if (filterBilinear)
                {
                    float UVx = SMUVs[z].x;
                    float UVy = SMUVs[z].y;
                    SMVCs.Add(((tex.GetPixelBilinear(UVx, UVy) + (Color32)brightenColor)) * (Color32)matColor);
                }
                else
                {
                    int UVx = (int)(SMUVs[z].x * (tex.width));
                    int UVy = (int)(SMUVs[z].y * (tex.width));
                    SMVCs.Add(((tex.GetPixel(UVx, UVy) + (Color32)brightenColor)) * (Color32)matColor);
                }
              
            }

            //faceted submesh uv, verts, vert colors ready for the submesh. Add them to the master lists only if combine submesh is enabled.
            if (combineSubMeshes)
            {
                foreach (Vector3 v in SMVerts)                
                    FinalVerts.Add(v); 
                foreach (Vector2 v in SMUVs)
                    FinalUVS.Add(v);
                foreach (Color32 c in SMVCs)
                    FinalVCs.Add(c);
            }
            else //keep track of the submesh info with 2d arrays.
            {
                finalSubMeshVerts[x] = SMVerts.ToArray();//FinalVerts.ToArray();
                finalSubMeshVCs[x] = SMVCs.ToArray();//FinalVCs.ToArray();
                finalSubMeshUVs[x] = SMUVs.ToArray();//FinalUVS.ToArray();
            }
        }

        int[][] finalSubMeshTris = new int[sourceMesh.subMeshCount][];

        //all done, now recreate the triangle index for the new mesh, then average the vertex colors.
        if (combineSubMeshes)
        {
            for (int l = 0; l < FinalVerts.Count; l++)
                FinalTris.Add(l);
        }
        else
        { //setup the correct triangle lists for each submesh.
            int counter=0;

            for (int x = 0; x < sourceMesh.subMeshCount; x++)
            {
                int[] tempSMTris = new int[finalSubMeshVerts[x].Length];

                for (int l = 0; l < finalSubMeshVerts[x].Length; l++)
                {
                    tempSMTris[l] = counter;
                    counter++;
                }
                finalSubMeshTris[x] = tempSMTris;
            }
        }

       

        //average the color to get faceted color
        if (combineSubMeshes)
        {
            for (int v = 0; v < FinalTris.Count; v += 3)
            {
                Color32 avg;
                int v1 = FinalTris[v];
                int v2 = FinalTris[v + 1];
                int v3 = FinalTris[v + 2];

                Vector3 c1 = new Vector3(FinalVCs[v1].r, FinalVCs[v1].g, FinalVCs[v1].b);
                Vector3 c2 = new Vector3(FinalVCs[v2].r, FinalVCs[v2].g, FinalVCs[v2].b);
                Vector3 c3 = new Vector3(FinalVCs[v3].r, FinalVCs[v3].g, FinalVCs[v3].b);
                Vector3 avgC = (c1 + c2 + c3) / 3;
                avg = new Color32((byte)avgC.x, (byte)avgC.y, (byte)avgC.z, 1);

                FinalVCs[v1] = avg;
                FinalVCs[v2] = avg;
                FinalVCs[v3] = avg;
            }
        }
        else
        {
            for (int x = 0; x < sourceMesh.subMeshCount; x++)
            {
                for (int l = 0; l < finalSubMeshTris[x].Length; l += 3)
                {
                    Color32 avg;                   
                    Vector3 c1 = new Vector3(finalSubMeshVCs[x][l].r, finalSubMeshVCs[x][l].g, finalSubMeshVCs[x][l].b);
                    Vector3 c2 = new Vector3(finalSubMeshVCs[x][l + 1].r, finalSubMeshVCs[x][l + 1].g, finalSubMeshVCs[x][l + 1].b);
                    Vector3 c3 = new Vector3(finalSubMeshVCs[x][l + 2].r, finalSubMeshVCs[x][l + 2].g, finalSubMeshVCs[x][l + 2].b);
                    Vector3 avgC = (c1 + c2 + c3) / 3;
                    avg = new Color32((byte)avgC.x, (byte)avgC.y, (byte)avgC.z, 1);

                    finalSubMeshVCs[x][l] = avg;
                    finalSubMeshVCs[x][l+1] = avg;
                    finalSubMeshVCs[x][l+2] = avg;
                }
            }
        }
     

        Mesh newVCMesh = new Mesh();


        if (combineSubMeshes)
        {
            newVCMesh.vertices = FinalVerts.ToArray();
            newVCMesh.uv = FinalUVS.ToArray();
            newVCMesh.colors32 = FinalVCs.ToArray();
            newVCMesh.triangles = FinalTris.ToArray();
        }
        else
        {   
            newVCMesh.subMeshCount = sourceMesh.subMeshCount;           
            for (int x = 0; x < newVCMesh.subMeshCount; x++)
            {
                for (int z = 0; z < finalSubMeshVerts[x].Length; z++)
                {
                    FinalVerts.Add(finalSubMeshVerts[x][z]);
                    FinalVCs.Add(finalSubMeshVCs[x][z]);
                    FinalUVS.Add(finalSubMeshUVs[x][z]);
                }
               
            }

            newVCMesh.vertices = FinalVerts.ToArray();
            newVCMesh.uv = FinalUVS.ToArray();
            newVCMesh.colors32 = FinalVCs.ToArray();
            for (int x = 0; x < newVCMesh.subMeshCount; x++)            
                newVCMesh.SetTriangles(finalSubMeshTris[x], x);
        }
            
        return newVCMesh;
    }
    private Mesh RenderSimpleProp(SkinnedMeshRenderer SMR,GameObject currentGO) //for meshes with blendshapes. access is unsupported in unity 4.5
    {
        //make a copy of the source mesh
        Mesh sourceMesh = SMR.sharedMesh;
        Vector3[][] finalSubMeshVerts = new Vector3[sourceMesh.subMeshCount][];
        Vector2[][] finalSubMeshUVs = new Vector2[sourceMesh.subMeshCount][];
        Color32[][] finalSubMeshVCs = new Color32[sourceMesh.subMeshCount][];

        //for every submesh, do all the work.
        for (int x = 0; x < sourceMesh.subMeshCount; x++)
        {

            List<Vector3> SMVerts = new List<Vector3>();
            List<Vector2> SMUVs = new List<Vector2>();
            List<Color32> SMVCs = new List<Color32>();
          
            //triangle arrays point to the array index of the vertices in the vertex array
            int[] triList = sourceMesh.GetTriangles(x); //submesh's index number corresponds to the material index of the meshrendere component
            //get the diffuse texture. Hopefully the shader obeys the standard naming convention..

            //doesn't work with substances or probably square textures..
            string path = AssetDatabase.GetAssetPath(SMR.sharedMaterials[x].GetTexture("_MainTex"));
            TextureImporter A = (TextureImporter)AssetImporter.GetAtPath(path);
            A.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Texture2D tex = (Texture2D)SMR.sharedMaterials[x].GetTexture("_MainTex");
            Color matColor = SMR.sharedMaterials[x].GetColor("_Color");

            //Coloring will not work with nonsquare textures.

            //we only want to go as far as 32x32
            int mipAmount = tex.mipmapCount - 4;//(int)Mathf.Log((int)tex.width)+1;            
            mipLevel = PickMipLevel(tex, mipAmount);


            if (mipLevel > 0) //if we chose to do blur amount..
            {
                int mipSize = (int)Mathf.Pow(2f, (mipAmount - (mipLevel - 1)) + 4); //+4 since we are raising 2 to the x power.
                Color[] mcs = tex.GetPixels(0, 0, mipSize, mipSize, mipLevel - 2);      
                tex = new Texture2D(mipSize, mipSize, TextureFormat.ARGB32, false);
                tex.SetPixels(mcs);
            }
        

            //to facet the submesh, go through each triangle. get each vertex, add it to an new vertex array. do same with uvs and boneweights

            for (int t = 0; t < triList.Length; t++)
            {
                //add the vertex to the new array.
                SMVerts.Add(sourceMesh.vertices[triList[t]]);
                SMUVs.Add(sourceMesh.uv[triList[t]]);
            }

            

            //make vertex colors from the diffuse but using the uvs and verts of the mesh we're making   
            //second time around, vert length is already b
            for (int z = 0; z < SMVerts.Count; z++)
            {
                
                if (filterBilinear)
                {
                    float UVx = SMUVs[z].x;
                    float UVy = SMUVs[z].y;
                    SMVCs.Add(((tex.GetPixelBilinear(UVx, UVy) + (Color32)brightenColor)) * (Color32)matColor);
                }
                else
                {
                    int UVx = (int)(SMUVs[z].x * (tex.width));
                    int UVy = (int)(SMUVs[z].y * (tex.width));
                    SMVCs.Add(((tex.GetPixel(UVx, UVy) + (Color32)brightenColor)) * (Color32)matColor);
                }
              
            }

            //faceted submesh uv, verts, vert colors ready for the submesh. Add them to the master lists only if combine submesh is enabled.
            if (combineSubMeshes)
            {
                foreach (Vector3 v in SMVerts)
                    FinalVerts.Add(v);
                foreach (Vector2 v in SMUVs)
                    FinalUVS.Add(v);
                foreach (Color32 c in SMVCs)
                    FinalVCs.Add(c);
            }
            else //keep track of the submesh info with 2d arrays.
            {
                finalSubMeshVerts[x] = SMVerts.ToArray();//FinalVerts.ToArray();
                finalSubMeshVCs[x] = SMVCs.ToArray();//FinalVCs.ToArray();
                finalSubMeshUVs[x] = SMUVs.ToArray();//FinalUVS.ToArray();
            }

           
        }
       

        //all done, now recreate the triangle index for the new mesh, then average the vertex colors.
        int[][] finalSubMeshTris = new int[sourceMesh.subMeshCount][];

        //all done, now recreate the triangle index for the new mesh, then average the vertex colors.
        if (combineSubMeshes)
        {
            for (int l = 0; l < FinalVerts.Count; l++)
                FinalTris.Add(l);
        }
        else
        { //setup the correct triangle lists for each submesh.
            int counter = 0;

            for (int x = 0; x < sourceMesh.subMeshCount; x++)
            {
                int[] tempSMTris = new int[finalSubMeshVerts[x].Length];

                for (int l = 0; l < finalSubMeshVerts[x].Length; l++)
                {
                    tempSMTris[l] = counter;
                    counter++;
                }
                finalSubMeshTris[x] = tempSMTris;
            }
        }



        if (combineSubMeshes)
        {
            for (int v = 0; v < FinalTris.Count; v += 3)
            {
                Color32 avg;
                int v1 = FinalTris[v];
                int v2 = FinalTris[v + 1];
                int v3 = FinalTris[v + 2];

                Vector3 c1 = new Vector3(FinalVCs[v1].r, FinalVCs[v1].g, FinalVCs[v1].b);
                Vector3 c2 = new Vector3(FinalVCs[v2].r, FinalVCs[v2].g, FinalVCs[v2].b);
                Vector3 c3 = new Vector3(FinalVCs[v3].r, FinalVCs[v3].g, FinalVCs[v3].b);
                Vector3 avgC = (c1 + c2 + c3) / 3;
                avg = new Color32((byte)avgC.x, (byte)avgC.y, (byte)avgC.z, 1);

                FinalVCs[v1] = avg;
                FinalVCs[v2] = avg;
                FinalVCs[v3] = avg;
            }
        }
        else
        {
            for (int x = 0; x < sourceMesh.subMeshCount; x++)
            {
                for (int l = 0; l < finalSubMeshTris[x].Length; l += 3)
                {
                    Color32 avg;
                    Vector3 c1 = new Vector3(finalSubMeshVCs[x][l].r, finalSubMeshVCs[x][l].g, finalSubMeshVCs[x][l].b);
                    Vector3 c2 = new Vector3(finalSubMeshVCs[x][l + 1].r, finalSubMeshVCs[x][l + 1].g, finalSubMeshVCs[x][l + 1].b);
                    Vector3 c3 = new Vector3(finalSubMeshVCs[x][l + 2].r, finalSubMeshVCs[x][l + 2].g, finalSubMeshVCs[x][l + 2].b);
                    Vector3 avgC = (c1 + c2 + c3) / 3;
                    avg = new Color32((byte)avgC.x, (byte)avgC.y, (byte)avgC.z, 1);

                    finalSubMeshVCs[x][l] = avg;
                    finalSubMeshVCs[x][l + 1] = avg;
                    finalSubMeshVCs[x][l + 2] = avg;
                }
            }
        }


        Mesh newVCMesh = new Mesh();


        if (combineSubMeshes)
        {
            newVCMesh.vertices = FinalVerts.ToArray();
            newVCMesh.uv = FinalUVS.ToArray();
            newVCMesh.colors32 = FinalVCs.ToArray();
            newVCMesh.triangles = FinalTris.ToArray();
        }
        else
        {
            newVCMesh.subMeshCount = sourceMesh.subMeshCount;
            for (int x = 0; x < newVCMesh.subMeshCount; x++)
            {
                for (int z = 0; z < finalSubMeshVerts[x].Length; z++)
                {
                    FinalVerts.Add(finalSubMeshVerts[x][z]);
                    FinalVCs.Add(finalSubMeshVCs[x][z]);
                    FinalUVS.Add(finalSubMeshUVs[x][z]);
                }

            }

            newVCMesh.vertices = FinalVerts.ToArray();
            newVCMesh.uv = FinalUVS.ToArray();
            newVCMesh.colors32 = FinalVCs.ToArray();
            for (int x = 0; x < newVCMesh.subMeshCount; x++)
                newVCMesh.SetTriangles(finalSubMeshTris[x], x);
        }
 
        return newVCMesh;

    }
    private Mesh RenderSkinnedMesh(SkinnedMeshRenderer SMR, GameObject currentGO)
    {

        
        //make a copy of the source mesh
        Mesh sourceMesh = SMR.sharedMesh;
        Vector3[][] finalSubMeshVerts = new Vector3[sourceMesh.subMeshCount][];
        Vector2[][] finalSubMeshUVs = new Vector2[sourceMesh.subMeshCount][];
        Color32[][] finalSubMeshVCs = new Color32[sourceMesh.subMeshCount][];
        

        foreach (Matrix4x4 matrix in sourceMesh.bindposes)
            FinalBPs.Add(matrix);
        
        //for every submesh, do all the work.
        for (int x = 0; x < sourceMesh.subMeshCount; x++)
        {

            List<Vector3> SMVerts = new List<Vector3>();
            List<Vector2> SMUVs = new List<Vector2>();
            List<Color32> SMVCs = new List<Color32>();
          //  List<int> SMTris = new List<int>();
            List<BoneWeight> SMBWs = new List<BoneWeight>();

            //triangle arrays point to the array index of the vertices in the vertex array
            int[] triList = sourceMesh.GetTriangles(x); //submesh's index number corresponds to the material index of the meshrendere component
            //get the diffuse texture. Hopefully the shader obeys the standard naming convention..

            string path = AssetDatabase.GetAssetPath(SMR.sharedMaterials[x].GetTexture("_MainTex"));
            
            TextureImporter A = (TextureImporter)AssetImporter.GetAtPath(path);
            
            A.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Texture2D tex = (Texture2D)SMR.sharedMaterials[x].GetTexture("_MainTex");
            Color matColor = SMR.sharedMaterials[x].GetColor("_Color");


            //Coloring will not work with nonsquare textures.

            //we only want to go as far as 32x32
            int mipAmount = tex.mipmapCount - 4;//(int)Mathf.Log((int)tex.width)+1;            
            mipLevel = PickMipLevel(tex, mipAmount);


            if (mipLevel > 0) //if we chose to do blur amount..
            {
                int mipSize = (int)Mathf.Pow(2f, (mipAmount - (mipLevel - 1)) + 4); //+4 since we are raising 2 to the x power.
                Color[] mcs = tex.GetPixels(0, 0, mipSize, mipSize, mipLevel - 2);                
                tex = new Texture2D(mipSize, mipSize, TextureFormat.ARGB32, false);
                tex.SetPixels(mcs);
            }
      
            //to facet the submesh, go through each triangle. get each vertex, add it to an new vertex array. do same with uvs and boneweights

            for (int t = 0; t < triList.Length; t++)
            {
                //add the vertex to the new array.
                SMVerts.Add(sourceMesh.vertices[triList[t]]);
                SMUVs.Add(sourceMesh.uv[triList[t]]);
                //Add the bone weights
                if(sourceMesh.boneWeights.Length>0)
                    SMBWs.Add(sourceMesh.boneWeights[triList[t]]);
            }
            //make vertex colors from the diffuse but using the uvs and verts of the mesh we're making   
            //second time around, vert length is already b
            for (int z = 0; z < SMVerts.Count; z++)
            {
                if (filterBilinear)
                {
                    float UVx = SMUVs[z].x;
                    float UVy = SMUVs[z].y;
                    SMVCs.Add(((tex.GetPixelBilinear(UVx, UVy) + (Color32)brightenColor)) * (Color32)matColor);
                }
                else
                {
                    int UVx = (int)(SMUVs[z].x * (tex.width));
                    int UVy = (int)(SMUVs[z].y * (tex.width));
                    SMVCs.Add(((tex.GetPixel(UVx, UVy) + (Color32)brightenColor)) * (Color32)matColor);
                }
            }

            //faceted submesh uv, verts, vert colors ready for the submesh. Add them to the master lists.           
            if (combineSubMeshes)
            {
                foreach (Vector3 v in SMVerts)
                    FinalVerts.Add(v);
                foreach (Vector2 v in SMUVs)
                    FinalUVS.Add(v);
                foreach (Color32 c in SMVCs)
                    FinalVCs.Add(c);
                
            }
            else //keep track of the submesh info with 2d arrays.
            {
                finalSubMeshVerts[x] = SMVerts.ToArray();//FinalVerts.ToArray();
                finalSubMeshVCs[x] = SMVCs.ToArray();//FinalVCs.ToArray();
                finalSubMeshUVs[x] = SMUVs.ToArray();//FinalUVS.ToArray();
            }
            foreach (BoneWeight b in SMBWs)
                FinalBWs.Add(b);
        }
        //all done, now recreate the triangle index for the new mesh, then average the vertex colors.

        int[][] finalSubMeshTris = new int[sourceMesh.subMeshCount][];

        //all done, now recreate the triangle index for the new mesh, then average the vertex colors.
        if (combineSubMeshes)
        {
            for (int l = 0; l < FinalVerts.Count; l++)
                FinalTris.Add(l);
        }
        else
        { //setup the correct triangle lists for each submesh.
            int counter = 0;

            for (int x = 0; x < sourceMesh.subMeshCount; x++)
            {
                int[] tempSMTris = new int[finalSubMeshVerts[x].Length];

                for (int l = 0; l < finalSubMeshVerts[x].Length; l++)
                {
                    tempSMTris[l] = counter;
                    counter++;
                }
                finalSubMeshTris[x] = tempSMTris;
            }
        }

        //average the color to get faceted color
        if (combineSubMeshes)
        {
            for (int v = 0; v < FinalTris.Count; v += 3)
            {
                Color32 avg;
                int v1 = FinalTris[v];
                int v2 = FinalTris[v + 1];
                int v3 = FinalTris[v + 2];

                Vector3 c1 = new Vector3(FinalVCs[v1].r, FinalVCs[v1].g, FinalVCs[v1].b);
                Vector3 c2 = new Vector3(FinalVCs[v2].r, FinalVCs[v2].g, FinalVCs[v2].b);
                Vector3 c3 = new Vector3(FinalVCs[v3].r, FinalVCs[v3].g, FinalVCs[v3].b);
                Vector3 avgC = (c1 + c2 + c3) / 3;
                avg = new Color32((byte)avgC.x, (byte)avgC.y, (byte)avgC.z, 1);

                FinalVCs[v1] = avg;
                FinalVCs[v2] = avg;
                FinalVCs[v3] = avg;
            }
        }
        else
        {
            for (int x = 0; x < sourceMesh.subMeshCount; x++)
            {
                for (int l = 0; l < finalSubMeshTris[x].Length; l += 3)
                {
                    Color32 avg;
                    Vector3 c1 = new Vector3(finalSubMeshVCs[x][l].r, finalSubMeshVCs[x][l].g, finalSubMeshVCs[x][l].b);
                    Vector3 c2 = new Vector3(finalSubMeshVCs[x][l + 1].r, finalSubMeshVCs[x][l + 1].g, finalSubMeshVCs[x][l + 1].b);
                    Vector3 c3 = new Vector3(finalSubMeshVCs[x][l + 2].r, finalSubMeshVCs[x][l + 2].g, finalSubMeshVCs[x][l + 2].b);
                    Vector3 avgC = (c1 + c2 + c3) / 3;
                    avg = new Color32((byte)avgC.x, (byte)avgC.y, (byte)avgC.z, 1);

                    finalSubMeshVCs[x][l] = avg;
                    finalSubMeshVCs[x][l + 1] = avg;
                    finalSubMeshVCs[x][l + 2] = avg;
                }
            }
        }
        //feed the new stuff into a new mesh
        Mesh newVCMesh = new Mesh();


        if (combineSubMeshes)
        {
            newVCMesh.vertices = FinalVerts.ToArray();
            newVCMesh.uv = FinalUVS.ToArray();
            newVCMesh.colors32 = FinalVCs.ToArray();
            newVCMesh.triangles = FinalTris.ToArray();
        }
        else
        {
            newVCMesh.subMeshCount = sourceMesh.subMeshCount;
            for (int x = 0; x < newVCMesh.subMeshCount; x++)
            {
                for (int z = 0; z < finalSubMeshVerts[x].Length; z++)
                {
                    FinalVerts.Add(finalSubMeshVerts[x][z]);
                    FinalVCs.Add(finalSubMeshVCs[x][z]);
                    FinalUVS.Add(finalSubMeshUVs[x][z]);
                }

            }

            newVCMesh.vertices = FinalVerts.ToArray();
            newVCMesh.uv = FinalUVS.ToArray();
            newVCMesh.colors32 = FinalVCs.ToArray();
            for (int x = 0; x < newVCMesh.subMeshCount; x++)
                newVCMesh.SetTriangles(finalSubMeshTris[x], x);
        }

        newVCMesh.boneWeights = FinalBWs.ToArray();
        newVCMesh.bindposes = FinalBPs.ToArray();
       
        return newVCMesh;

    }

    private int PickMipLevel(Texture2D tex, int mipAmount)
    {
        int ml = 0;

        if (blurAmount == BlurAmount.None)
            return ml;
        else
        {
            if (blurAmount == BlurAmount.Some)
                ml = mipAmount / 2;
            else
                ml = mipAmount;
        }
        return ml;
    }
    private Material CreateMasterMaterial(string folderPath, string matName)
    {
        //write the material. There will be only one since all the GO's will use it.
        Material newMat = null; 

        //first check the export folder for an existing material based on the given matName
        string[] files = Directory.GetFiles(exportWindowsFolder);

        for (int x = 0; x < files.Length; x++)
        {
            if (files[x].Contains("_Mat"))
            {
                //must be a mesh we're looking at in files[x] then. 
                if (files[x].Contains(matName) && !files[x].Contains(".meta"))
                {
                    string assetPath = files[x].Replace(Application.dataPath, "Assets");
                    assetPath = assetPath.Replace("\\", "/");                    
                    newMat = (Material)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Material));
                }
            }
        }

        //if there is no material in the folder, make a new one.
        if(newMat==null)
        {           
			newMat = new Material(Shader.Find("QuantumTheory/VertexColors/Unity5/Diffuse"));
            newMat.name = matName;
         AssetDatabase.CreateAsset(newMat, folderPath + "/" + newMat.name + ".asset");
        }
        
        return newMat;
    }
    private void CreateMesh(Mesh vcmesh, string folderPath)
    {
        //write the new meshes to the project. Will overwrite if one is found.
        vcmesh.name = vcmesh.name.Replace("(Clone)", "");
        AssetDatabase.CreateAsset(vcmesh, folderPath + "/" + vcmesh.name + ".asset");
        AssetDatabase.Refresh();
    }
    private void UpdatePrefab(GameObject WorkingGO, GameObject[] MeshGOs, bool[] isSkinnedMesh, string folderPath)
    {
        string pfPath = folderPath + "/" + WorkingGO.name + meshSuffix+".prefab";
       // string pfPath = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(WorkingGO));
        GameObject sourcePF = (GameObject)AssetDatabase.LoadAssetAtPath(pfPath, typeof(GameObject));

        if (sourcePF == null)
            PrefabUtility.CreatePrefab(pfPath, WorkingGO);
        else
        {
            if (overwriteMeshes)
                PrefabUtility.ReplacePrefab(WorkingGO, sourcePF, ReplacePrefabOptions.ReplaceNameBased);
            else
            {
                pfPath = folderPath + "/" + WorkingGO.name + meshSuffix + filenameCounter + ".prefab";
                PrefabUtility.CreatePrefab(pfPath, WorkingGO);
            }
            
        }
        AssetDatabase.Refresh();
    }

	
    
}
