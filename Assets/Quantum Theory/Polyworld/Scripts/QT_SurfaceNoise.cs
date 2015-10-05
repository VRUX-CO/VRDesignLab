using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QT_SurfaceNoise : MonoBehaviour
{
   

    public float baseWaveSpeed = .02f;
    public float waveScale = 2.0105f;//3.7698 for 10m plane
    public float speedMultiplier=15;
    public float strengthMultiplier = 1;
    public float baseNoiseStrength = 0.03f;	
    public bool RecalculateNormals = false;
    public bool useVertexAlpha = false;
    public bool enableMultiMesh = false;
    public bool useOverride = false;

    public bool enableDebug = true;
    public bool enableOffset = false;
    public bool enableLOD = true;
    public int LODDistance = 50;

    public int scaleMultiplier = 1; 
    Vector3[] baseHeight;
    Vector3[] baseHeightWorld;
    Vector3[] newVerts;
	Vector3 vertex;
    public bool ZAxis = true;
    public bool XAxis = false;
    public bool YAxis = false;
	public bool showDebugSphere = false;
  

    Mesh[] meshes;
    List<Vector3[]> allBaseVertices; //a list of vector3 arrays. each element is the base vertex set of each mesh. Used for Polyworld Terrains and unique meshes
    List<Color[]> allBaseVertexAlphas; //all the colors of the meshes, but we'll only use alpha. ***Should be optimized later.
    float[] randomOffsets; //holds the random offsets for each mesh if Multimesh + random offset is used.

	bool isStatic=false;
    
	//go through each mesh and add all vertices to the temp vertex array
    void Start()
    {  

        //get the meshes in the children
        MeshFilter[] mfs = this.gameObject.GetComponentsInChildren<MeshFilter>(false);
        meshes = new Mesh[mfs.Length];
        for (int x = 0; x < meshes.Length; x++)
            meshes[x] = mfs[x].mesh;
        //cache all the meshes. you need this to animate everything properly.
        if (enableMultiMesh) //if it's a polyworld terrain, we need all the meshes to be animated uniquely.
        {
            //init the size of the array based on the number of meshes            
            allBaseVertices = new List<Vector3[]>(meshes.Length);
            allBaseVertexAlphas = new List<Color[]>(meshes.Length);
            if (enableOffset)
                randomOffsets = new float[meshes.Length];

            //put all the vertices of the meshes into a temp array
            for (int x = 0; x < meshes.Length; x++)
            {
                allBaseVertices.Add(meshes[x].vertices);
                allBaseVertexAlphas.Add(meshes[x].colors);
                if (enableOffset)              
                    randomOffsets[x] = Random.Range(2, 6);
            }
           
        }
        else //we assume you have one mesh duplicated as children. We'll animate one and distribute the result.
        {
            allBaseVertexAlphas = new List<Color[]>();
            baseHeight = meshes[0].vertices;
            allBaseVertexAlphas.Add(meshes[0].colors);
            newVerts = new Vector3[baseHeight.Length];
        }

		if(!Camera.main)		
			Debug.LogWarning("No MainCamera found. SurfaceNoise LOD will not run on GameObject: "+this.gameObject.name);
		for (int x = 0; x < mfs.Length; x++)
		{
			if(mfs[x].gameObject.isStatic)
			{
				isStatic=true;
				break;
			}
		}
    }

    void OnRenderObject()//FixedUpdate()
	{
		if(isStatic==false)
		    {
		    if(Camera.main)
		        {
                    if (enableLOD)
                    {
                        //be nice to have a lerp to enabled/disabled instead of a pop.
                        if (Vector3.Distance(this.transform.position, Camera.main.transform.position) < LODDistance)
                            RunWave();
                    }
                    else
                        RunWave();
		        }
		    else		
			    Debug.LogError("No camera tagged MainCamera is found for SurfaceNoise.");		
		    }
		else		
			Debug.LogError("One or more meshes in "+this.gameObject.name+"'s hierarchy is tagged static. SurfaceNoise requires all objects to be not tagged static.");
		
    }

    private void RunWave()
    {

        float T = Time.time;
        float newSpeedScale = speedMultiplier * baseWaveSpeed;
        float newStrengthScale = strengthMultiplier * baseNoiseStrength;
       
        
        if (enableMultiMesh)
        {
            int ABV = allBaseVertices.Count;
            //for every mesh..
            for (int x = 0; x < ABV; x++)
            {
                if (enableOffset)//takes each mesh and makes them wave uniquely to one another.
                    T += randomOffsets[x];
                Vector3[] allNewVerts = new Vector3[allBaseVertices[x].Length];

                //animate one mesh at a time. plug in the new vertex values into the new array.

                for (int z = 0; z < allBaseVertices[x].Length; z++)
                {

                    vertex = allBaseVertices[x][z];
                    float val = Mathf.Sin(1.25f * ((T * newSpeedScale + vertex.x + vertex.y + vertex.z) * waveScale)) * newStrengthScale;

                    if (useVertexAlpha)
                        val *= allBaseVertexAlphas[x][z].a;
                    //val *= VAs[z];
                    if (ZAxis == true)
                        vertex.z += val;
                    if (XAxis == true)
                        vertex.x += val;
                    if (YAxis == true)
                        vertex.y += val;
                    allNewVerts[z] = vertex;
                }

                meshes[x].vertices = allNewVerts;
                if (RecalculateNormals)
                    meshes[x].RecalculateNormals();
            }
        }
        else  //it's not a polyworld terrain, so do optimized code that trusts only the same mesh being used as children of the heirarchy.
        {
            for (int i = 0; i < meshes[0].vertexCount; i++)
            {
                vertex = baseHeight[i];
                float val = Mathf.Sin(1.25f * ((T * newSpeedScale + vertex.x + vertex.y + vertex.z) * waveScale)) * newStrengthScale;
                if (useVertexAlpha)
                    val *= allBaseVertexAlphas[0][i].a;
                //val *= VAs[i];
                if (ZAxis == true)
                    vertex.z += val;
                if (XAxis == true)
                    vertex.x += val;
                if (YAxis == true)
                    vertex.y += val;
                newVerts[i] = vertex;
            }

            if (RecalculateNormals)
                meshes[0].RecalculateNormals();
            foreach (Mesh m in meshes)
            {
                m.vertices = newVerts;
                if (RecalculateNormals)
                    m.normals = meshes[0].normals;
            }
        }
    }
}
    
