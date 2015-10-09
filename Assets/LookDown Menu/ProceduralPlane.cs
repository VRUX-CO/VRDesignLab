using UnityEngine;
using System.Collections;

public class ProceduralPlane : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    BuildMesh();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void BuildMesh()
  {
    // You can change that line to provide another MeshFilter
    MeshFilter filter = gameObject.AddComponent<MeshFilter>();
    Mesh mesh = filter.mesh;
    mesh.Clear();

    float length = 1f;
    float width = 1f;
    int resX = 2; // 2 minimum
    int resZ = 2;

    Vector3[] vertices = new Vector3[resX * resZ];
    for (int z = 0; z < resZ; z++)
    {
      // [ -length / 2, length / 2 ]
      float zPos = ((float)z / (resZ - 1) - .5f) * length;
      for (int x = 0; x < resX; x++)
      {
        // [ -width / 2, width / 2 ]
        float xPos = ((float)x / (resX - 1) - .5f) * width;
        vertices[x + z * resX] = new Vector3(xPos, 0f, zPos);
      }
    }

    Vector3[] normales = new Vector3[vertices.Length];
    for (int n = 0; n < normales.Length; n++)
      normales[n] = Vector3.up;

    Vector2[] uvs = new Vector2[vertices.Length];
    for (int v = 0; v < resZ; v++)
    {
      for (int u = 0; u < resX; u++)
      {
        uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
      }
    }

    int nbFaces = (resX - 1) * (resZ - 1);
    int[] triangles = new int[nbFaces * 6];
    int t = 0;
    for (int face = 0; face < nbFaces; face++)
    {
      // Retrieve lower left corner from face ind
      int i = face % (resX - 1) + (face / (resZ - 1) * resX);

      triangles[t++] = i + resX;
      triangles[t++] = i + 1;
      triangles[t++] = i;

      triangles[t++] = i + resX;
      triangles[t++] = i + resX + 1;
      triangles[t++] = i + 1;
    }

    mesh.vertices = vertices;
    mesh.normals = normales;
    mesh.uv = uvs;
    mesh.triangles = triangles;

    mesh.RecalculateBounds();
    mesh.Optimize();
  }
}
