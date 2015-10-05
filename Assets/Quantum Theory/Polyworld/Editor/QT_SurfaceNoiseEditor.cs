using UnityEngine;
using System.Collections;
using UnityEditor;

enum Plane2m { MediumOpposite, Medium, MediumOppositeChoppy, ChoppyStationary }
enum Plane10m { MediumOpposite, Medium, MediumOppositeChoppy, ChoppyStationary, Large, VeryLarge, Huge }

[CustomEditor(typeof(QT_SurfaceNoise))]
public class QT_SurfaceNoiseEditor : Editor {

    static Plane2m P2;
    static Plane10m P10;
    
	private GameObject debugSphere;

    //hardcoded wavescale values for all modular planes. WaveScaleA is for 2.5m planes. B uses A + some more.. 
    //Terrain could use whatever as it's multimesh.
    float[] WaveScaleA = new float[] {6.0315f, 10.0525f,14.0743f,22.1148f};
    float[] WaveScaleB = new float[] {6.0315f, 10.0525f,14.0743f,22.1148f,6.5345f,7.037f,7.5398f};
   
    //Check to see if the plane is 10m. If so, change the enum choices.
    private bool is2mPlane=false;

    public override void OnInspectorGUI()
    {        
        
        //put check to make sure you're putting surfacenoise on the parent GO.
        if (isValidHierarchy(Selection.gameObjects[0]))
        {
            QT_SurfaceNoise SN = (QT_SurfaceNoise)target;
            bool isMM = isMultiMesh(Selection.gameObjects[0]);

            if (isMM && SN.enableMultiMesh == false)
                EditorGUILayout.HelpBox("Varied Meshes or PolyWorld Terrain Detected.\n\nMultiMesh flag should be enabled to prevent errors. This will be more costly to calculate. See Help for important information.", MessageType.Warning);
            else if (isMM == false && SN.enableMultiMesh == true)
                EditorGUILayout.HelpBox("No Varied Meshes or PolyWorld Terrain detected.\n\nMutiMesh flag could be disabled for performance increase.", MessageType.Info);
          
            SN.useOverride = EditorGUILayout.Toggle("Preset Override", SN.useOverride);
            if (SN.useOverride == false)
            {
                is2mPlane = CheckPlaneSize(Selection.gameObjects[0]);

                if (is2mPlane)
                    P2 = (Plane2m)EditorGUILayout.EnumPopup("Wave Preset:", P2);
                else
                    P10 = (Plane10m)EditorGUILayout.EnumPopup("Wave Type:", P10);
                SN.waveScale = SetWaveScale();
            }
            else            
                SN.waveScale = EditorGUILayout.Slider("Wave Shape", SN.waveScale, 0.1f, 50);
           
           // SN.enableDebug = EditorGUILayout.Toggle("enable debug", SN.enableDebug);
           // SN.scaleMultiplier = EditorGUILayout.IntSlider(SN.scaleMultiplier, 1, 20);
           // SN.waveOffset = EditorGUILayout.Slider(SN.waveOffset, 0f, 6f);
            SN.speedMultiplier = EditorGUILayout.Slider("Wave Speed", SN.speedMultiplier, 1, 400);
            SN.strengthMultiplier = EditorGUILayout.Slider("Wave Height", SN.strengthMultiplier, 0, 10);
            SN.RecalculateNormals = EditorGUILayout.Toggle("Recalculate Normals", SN.RecalculateNormals);            
            SN.useVertexAlpha = EditorGUILayout.Toggle("Use Vertex Alpha", SN.useVertexAlpha);
            SN.XAxis = EditorGUILayout.Toggle("X Axis", SN.XAxis);
            SN.YAxis = EditorGUILayout.Toggle("Y Axis", SN.YAxis);
            SN.ZAxis = EditorGUILayout.Toggle("Z Axis", SN.ZAxis);
            SN.enableMultiMesh = EditorGUILayout.Toggle("Enable Multimesh", SN.enableMultiMesh);
            if (SN.enableMultiMesh)
                SN.enableOffset = EditorGUILayout.Toggle("Unique Offset", SN.enableOffset);
            SN.enableLOD = EditorGUILayout.Toggle("Enable LOD", SN.enableLOD);
            if (SN.enableLOD)
			{
				if(!Camera.main)
					EditorGUILayout.HelpBox("No Main Camera Found! LOD can't run.",MessageType.Error);
                SN.LODDistance = EditorGUILayout.IntSlider("Distance:", SN.LODDistance,5,100);
				SN.showDebugSphere = EditorGUILayout.Toggle("Visualize Distance:",SN.showDebugSphere);
				if(SN.showDebugSphere)
				{
					if(debugSphere==null)	
					{
						debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
						debugSphere.name = "#DebugSphere - Delete Me";
					}
					else
					{
						debugSphere.transform.localPosition = Selection.activeGameObject.transform.localPosition;
						//debugSphere.transform.position = Selection.activeGameObject.transform.position;
						debugSphere.transform.localScale = new Vector3(SN.LODDistance,SN.LODDistance,SN.LODDistance);
					}
				}
				else
				{
					if(debugSphere)
						DestroyImmediate(debugSphere);
				}

			}
			if (GUILayout.Button("Help"))
			{
				Application.OpenURL("http://qt-ent.com/PolyWorld/scripts/");
			}
        }
        else
            EditorGUILayout.HelpBox("No valid children found. Noise is only applied to children of a parent Game Object.\n\nLink some gameobjects with MeshFilter components to an empty Game Object. SkinnedMeshes are not supported yet.", MessageType.Error);
    }

	private void OnDisable()
	{
		if(debugSphere)
			DestroyImmediate(debugSphere);
	}
    //searches through the hierarchy for meshfilters. If there is one found, it's valid. If there is no hierarchy, or if there is a skinnedmesh, it's invalid.
    private bool isValidHierarchy(GameObject parent)
    {
        bool isValid = false;

        if (parent.transform.childCount > 0)
        {
            SkinnedMeshRenderer[] SMRs = parent.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            if (SMRs.Length > 0)
                isValid = false;
            MeshFilter[] MFs = parent.GetComponentsInChildren<MeshFilter>(true);
            if (MFs.Length > 0)
                isValid = true;
        }
        return isValid;

    }

    //enum Plane10m { Medium,Large,VeryLarge,CalmStationary,Choppy }
    // enum Plane2m { MediumOpposite, Medium, LargeOppositeChoppy, ChoppyStationary }
    private float SetWaveScale()
    {
        float scale = 0f;
        if (is2mPlane)
        {
            if (P2 == Plane2m.MediumOpposite)
                scale = WaveScaleA[0];
            else if (P2 == Plane2m.Medium)
                scale = WaveScaleA[1];
            else if (P2 == Plane2m.MediumOppositeChoppy)
                scale = WaveScaleA[2];
            else if (P2 == Plane2m.ChoppyStationary)
                scale = WaveScaleA[3];
        }
        else
        {
            if (P10 == Plane10m.MediumOpposite)
                scale = WaveScaleB[0];
            else if (P10 == Plane10m.Medium)
                scale = WaveScaleB[1];
            else if (P10 == Plane10m.MediumOppositeChoppy)
                scale = WaveScaleB[2];
            else if (P10 == Plane10m.ChoppyStationary)
                scale = WaveScaleB[3];
            else if (P10 == Plane10m.Large)
                scale = WaveScaleB[4];
            else if (P10 == Plane10m.VeryLarge)
                scale = WaveScaleB[5];
            else if (P10 == Plane10m.Huge)
                scale = WaveScaleB[6];

        }
        return scale;
    }
   

    private bool CheckPlaneSize(GameObject target)
    {
        bool is2m = true;

        MeshFilter[] mfs = target.GetComponentsInChildren<MeshFilter>(true);

        Mesh m = mfs[0].sharedMesh;
   
        if (m.bounds.size.x >2.5f)
            is2m = false;
        return is2m;
    }
    //checks to see if you're using multiple different meshes or the same one.
    private bool isMultiMesh(GameObject go)
    {
        bool isMultiMesh = false;
        MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>(true);

        for (int x = 0; x < mfs.Length-1; x++)
        {
            Mesh m = mfs[x].sharedMesh;

            for (int z = x + 1; z < mfs.Length; z++)
            {
                Mesh compare = mfs[z].sharedMesh;
                if (m.vertexCount != compare.vertexCount)
                {
                    isMultiMesh = true;
                    break;
                }
            }
            if (isMultiMesh)
                break;
        }

        return isMultiMesh;
    }
}
