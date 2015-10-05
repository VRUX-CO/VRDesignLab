using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngineInternal;
using System.Collections;
using System.Collections.Generic;

// 6/24/2014: Sometimes the skybox will show through onto the terrainmap causing subtle vertex-colored lines to show up on an edge of the terrain. Nudge the gameview window in or out and recalculate. This is due to
// the terrain extents being in a sub-pixel location. Unity has to round that off, so sometimes it grabs an extra row or column of pixels. If anyone knows how to alleviate that, send me an email at pete@quantumtheoryentertainment.com

[ExecuteInEditMode]
public class QT_RenderBaseMap : MonoBehaviour {
    #if UNITY_EDITOR
	private EditorWindow gameWindow = null;
    
 
	public void RenderMap(Terrain terrainObject, TerrainData terrain, GameObject targetGO,bool smoothVertColor)
	{
        
        StartCoroutine(YieldSaveScreenshot(terrainObject, terrain, targetGO, smoothVertColor));
    }

    
	public void OnEnable()
	{
		// save the gamewindow
		gameWindow = GetWindowByName("UnityEditor.GameView");

	}

    //The workhorse. Renders the terrain top down in the game view, stores it off, takes the terrain mesh and samples each vertex's uvs for a vertex color then optionally averages them into one.
    //Also does all the necessary scene stuff to get a correct topdown texture from the terrain (turn lights off, ambient white, ortho cam, etc).
    //We use coroutine to get a correct screenshot via Waitforendofframe().
	IEnumerator YieldSaveScreenshot(Terrain terrainObject, TerrainData terrain, GameObject targetGO,bool smoothVertColor) 
	{       
        //get the mesh filters for the target gameobject
        MeshFilter[] meshFilters = targetGO.GetComponentsInChildren<MeshFilter>(true);      
        //get the prefabPath to the targetgo's parent prefab
        string prefabPath = AssetDatabase.GetAssetPath((GameObject)PrefabUtility.GetPrefabParent(targetGO));
        //remove the prefab extention so we can use this to overwrite the meshes with a .asset extention
        prefabPath = prefabPath.Replace(".prefab", "");

        #region stuff irrelevant to rendering..
        if (meshFilters.Length==0)	
		{
			Debug.LogError("No Meshes Found. Rendering aborted!");
			DestroyImmediate(this.gameObject);
			yield break;
		}
		else
		{
            
            //set everything up
			float terrainSize = terrain.size.x;///2; //assuming it's square.
			Camera renderCam = this.GetComponent<Camera>();
            renderCam.depth = 100; //depth has to be greater than any other cameras.
			UnityEngine.Rendering.AmbientMode previousAmbientMode = RenderSettings.ambientMode;
			float ambIntensity = RenderSettings.ambientIntensity;
			float terrainOffset = 500; //offset the terrain on the Y axis so no other meshes or objects get rendered into the basemap. Yea, it's simple..
            renderCam.farClipPlane = 500 + terrainOffset + terrain.size.y;
			terrainObject.transform.position += new Vector3 (0, terrainOffset, 0); //offset it.
			renderCam.transform.position = new Vector3(terrainObject.transform.position.x+terrainSize/2,500+terrainOffset+terrain.size.y,terrainObject.transform.position.z+terrainSize/2);//terrainObject.transform.position.x+terrainSize;
			renderCam.transform.localEulerAngles = new Vector3(90,0,0); //aim it down, rotate it to match the uvs and rotation of target mesh
			renderCam.orthographic = true;
            renderCam.orthographicSize = (terrainSize/2); //dividing by 2 ensures the square terrain fits perfectly within the height of the frustum
            Color ambient = RenderSettings.ambientLight;
            RenderSettings.ambientLight = Color.white;
			RenderSettings.ambientIntensity = 1;
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

            bool fog = RenderSettings.fog;
            RenderSettings.fog = false;

            //turn off the directional lights.
            Light[] Lights = (Light[])GameObject.FindObjectsOfType(typeof(Light));
            foreach (Light l in Lights)
            {
                if (l.type == LightType.Directional)
                    l.enabled = false;
            }
           
			yield return new WaitForEndOfFrame();
           
            //get all the screen and world coordinates of the terrain extents
            Vector3 TerrainLL = terrainObject.transform.position;
            Vector3 TerrainUR = terrainObject.transform.position + new Vector3(terrainSize, 0, terrainSize);
            Vector3 LL = renderCam.WorldToScreenPoint(TerrainLL);
			Vector3 UR = renderCam.WorldToScreenPoint(TerrainUR);
            //setup the rect so we can crop out the texture in case the gameview is not square.
            //Debug.Log("LL: " + LL + "    UR:" + UR);           
            float texStart = LL.x;         
           // Debug.Log("Texstart: " + texStart);          
            float texEnd = UR.x;           
            //Debug.Log("texend: " + texEnd);
            float texWidth = texEnd - texStart;
            //Debug.Log("texwidth: " + texWidth);
         
			gameWindow.Repaint();
			
			// wait for the end of frame
			yield return new WaitForEndOfFrame();
            //if the gameview window is shaped weird or if ti doesn't encompass the terrain well enough..
            if(texStart<0)
            {
            EditorUtility.DisplayDialog("Bad Game View Size", "Rendering the terrain vertex colors relies heavily on a game view with a proper size. The current size of your game view window is too small or oddly shaped.\n\nPlease set the aspect ratio to 1:1 to ensure a proper render. Aborting render.", "OK");
            RevertScene(fog, ambient, terrainObject, terrainOffset, Lights,previousAmbientMode,ambIntensity);
            yield break;
            }
           
			//the next two lines will capture the screen.
            Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);         
			tex = SaveAsFlippedTexture2D (tex, false, true);
            tex.Apply();
           // var bytes = tex.EncodeToPNG();
           // File.WriteAllBytes(Application.dataPath + "/UnCropped.png", bytes);
            Color[] newTex = tex.GetPixels((int)texStart, 0,(int)texWidth, (int)UR.y);
            tex = new Texture2D((int)texWidth, (int)UR.y, TextureFormat.RGB24, false);
            tex.SetPixels(newTex);
      
            //save the terrainmap to the same folder as the mesh
           GameObject pfp = (GameObject)PrefabUtility.GetPrefabParent(targetGO);
            string texPath = AssetDatabase.GetAssetPath(pfp);
            texPath = texPath.Replace(pfp.name + ".prefab", "TerrainMap-OktoDelete.asset");
            tex.name = "TerrainMap-OktoDelete";
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            //texPath = texPath.Replace(pfp.name+".prefab","");
            //write the texture to disk so we can reimport it with specific settings. It's the only way we can get rid of the edge filtering issue.
            //AssetDatabase.CreateAsset(tex, texPath);
           
            //var bytes = tex.EncodeToPNG();
           //File.WriteAllBytes(texPath, bytes);
           
     
           AssetDatabase.Refresh();
          /*
           TextureImporter A = (TextureImporter)AssetImporter.GetAtPath(texPath);
           yield return new WaitForEndOfFrame();
           A.isReadable = true;
           A.filterMode = FilterMode.Point;
           A.mipmapEnabled = false;
           A.wrapMode = TextureWrapMode.Clamp;
           AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
           tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture2D));
	        */
        #endregion
           
            string chunkFolderPath = "";
            string chunkMeshPath ="";
            if (meshFilters[0].sharedMesh.name.Contains("-Chunk"))
            {
                chunkFolderPath = prefabPath;
                chunkFolderPath = chunkFolderPath.Replace("-Scene", "-Chunks");
                chunkMeshPath = chunkFolderPath;
                chunkMeshPath = chunkMeshPath.Replace("-Chunks", "-Chunks/" + meshFilters[0].sharedMesh.name);
                chunkMeshPath = chunkMeshPath.Replace("-Chunk0", "-Chunk");
            }
           
            //for every meshfilter we find, render vertex color
			for(int p=0;p<meshFilters.Length;p++)
			{
                Mesh targetMesh;
                string mfName = meshFilters[p].sharedMesh.name;
                //if the first mesh isn't a chunk mesh, load the scene mesh
                if (!mfName.Contains("-Chunk"))
                    targetMesh = (Mesh)AssetDatabase.LoadAssetAtPath(prefabPath + p.ToString() + ".asset", typeof(Mesh));
                else
                {//load the chunk mesh                   
                    targetMesh = (Mesh)AssetDatabase.LoadAssetAtPath(chunkMeshPath + p.ToString() + ".asset", typeof(Mesh));
                }
               
				Vector3[] vertices = targetMesh.vertices; //just the positions in local space  
				Vector2[] uvs = targetMesh.uv;                
				Color32[] vertcolor = new Color32[vertices.Length];
				int[] tris = targetMesh.triangles;	
				//rotate the uvs so we match with the terrain
			
                for(int c=0;c<uvs.Length;c++)
				{                    
					uvs[c] = Quaternion.Euler(0,0,90) * uvs[c];
					uvs[c] += new Vector2(1,0);
				}
          
				//get the pixel color of the texel and store it off.
				for(int x=0;x<vertices.Length;x++)
				{				
					int UVx = (int)(uvs[x].x * tex.width);
					int UVy = (int)(uvs[x].y * tex.width);
					vertcolor[x] = tex.GetPixel(UVx,UVy);                   
                   //vertcolor[x] = tex.GetPixelBilinear(uvs[x].x, uvs[x].y);
				}
				//now average them
				//Every three elements in the tris array represents the pointers to the three vertices in each tri.
				//since we import at smoothing angle 0, angles at 0 degrees will be smoothed unfortunately. I'd have to rewrite the exporter to do it 100% correct.
                if (!smoothVertColor)
                {
                    for (int z = 0; z < tris.Length; z += 3)
                    {
                        int v1 = tris[z];
                        int v2 = tris[z + 1];
                        int v3 = tris[z + 2];
                        Color32 c1 = Color32.Lerp(vertcolor[v1], vertcolor[v2], 0.5f);
                        Color32 c2 = Color32.Lerp(vertcolor[v1], vertcolor[v3], 0.5f);
                        Color32 avg = Color32.Lerp(c1, c2, 0.5f);
                        vertcolor[v1] = avg;
                        vertcolor[v2] = avg;
                        vertcolor[v3] = avg;
                    }
                 }
                //create a new mesh with vertex colors, save it off, and apply it back to the meshfilter in the prefab
				Mesh newVCMesh = new Mesh();               
                newVCMesh.vertices = targetMesh.vertices;
                newVCMesh.normals = targetMesh.normals;
                newVCMesh.tangents = targetMesh.tangents;
                newVCMesh.uv = targetMesh.uv;
				newVCMesh.uv2 = targetMesh.uv2;
                newVCMesh.triangles = targetMesh.triangles;
                newVCMesh.colors32=vertcolor;
                newVCMesh.name = targetMesh.name;

                //apply the color to the current loaded mesh
                //overwrite the mesh with the one we made with vertex colors
                if (!mfName.Contains("-Chunk"))
                    AssetDatabase.CreateAsset(newVCMesh, prefabPath + p.ToString() + ".asset");
                else //if it's a chunk, overwrite it in the subfolder
                {
                    AssetDatabase.CreateAsset(newVCMesh, chunkMeshPath + p.ToString() + ".asset");
                }
                //assign the newmesh to the scene's gameobject's current meshfilter.
               meshFilters[p].mesh = newVCMesh;             
              
            }

            PrefabUtility.ReplacePrefab(targetGO, (GameObject)PrefabUtility.GetPrefabParent(targetGO), ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.Refresh();

            RevertScene(fog, ambient, terrainObject, terrainOffset, Lights,previousAmbientMode,ambIntensity);
        
          
		}

   
	}

    //reverts the scene in the event of a bad game view window size.
	private void RevertScene(bool fog, Color ambient,Terrain terrainObject,float terrainOffset, Light[] Lights, UnityEngine.Rendering.AmbientMode previousAmbientMode, float ambIntensity)
    {
        EditorUtility.SetDirty(this);
		RenderSettings.ambientMode = previousAmbientMode;
		RenderSettings.ambientIntensity = ambIntensity;
        RenderSettings.fog = fog; //put fog back to where it was.
        RenderSettings.ambientLight = ambient;
        terrainObject.transform.position -= new Vector3(0, terrainOffset, 0); //put the terrain back
        //turn dir lights back on
        foreach (Light l in Lights)
        {
            if (l.type == LightType.Directional)
                l.enabled = true;
        }
        gameWindow.Repaint();

        if (SceneView.sceneViews.Count > 0)
        {
            SceneView sceneView = (SceneView)SceneView.sceneViews[0];
            sceneView.Focus();
            sceneView.Repaint();
        }
        DestroyImmediate(this.gameObject);
    }

    /// <summary>
    /// Gives you the asset path of the prefab parent of the specified gameobject.
    /// </summary>
    /// <param name="GO"></param>
    /// <returns></returns>
    private string GetPathOnly(GameObject GO)
    {
        string path = "";
        path = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(GO));//AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabType(GO));
        string combined = "/" + GO.name + ".obj";        
        path = path.Replace(combined, "");
        combined = "/" + GO.name + ".prefab";
        path = path.Replace(combined, "");       
        return path;
    }


	
	public Texture2D SaveAsFlippedTexture2D(Texture2D input, bool vertical, bool horizontal)
	{
		Texture2D flipped = new Texture2D (input.width, input.height);
		Color32[] data = new Color32[input.width * input.height];
		Color32[] flipped_data = new Color32[data.Length];

		data = input.GetPixels32 ();
		
		for (int x = 0; x < input.width; x++)
		{
			for (int y = 0; y < input.height; y++)
			{
				int index = 0;
				if (horizontal && vertical)
					index = input.width - 1 - x + (input.height - 1 - y) * input.width;
				else if (horizontal && !vertical)
					index = input.width - 1 - x + y * input.width;
				else if (!horizontal && vertical)
					index = x + (input.height - 1 - y) * input.width;
				else if (!horizontal && !vertical)
					index = x + y * input.width;
				
				flipped_data[x + y * input.width] = data[index];
			}
		}
		
		flipped.SetPixels32 (flipped_data);
		
		return flipped;
	}

    
    private EditorWindow GetWindowByName(string name)
	{
		System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
		Type type = assembly.GetType(name);
		
		return EditorWindow.GetWindow(type);		
	}
    #endif

    public void Update()
    {
    }
}
