using UnityEngine;
using System.Collections;

public static class MeshUtilities
{
  // http://wiki.unity3d.com/index.php/ProceduralPrimitives
  public static MeshFilter AddTorusMeshFilter(GameObject gameObject, float radius1 = 1f, float radius2 = .05f, int nbRadSeg = 44, int nbSides = 18)
  {
    MeshFilter filter = gameObject.AddComponent<MeshFilter>();
    Mesh mesh = filter.mesh;
    mesh.Clear();

    #region Vertices
    Vector3[] vertices = new Vector3[(nbRadSeg + 1) * (nbSides + 1)];
    float _2pi = Mathf.PI * 2f;
    for (int seg = 0; seg <= nbRadSeg; seg++)
    {
      int currSeg = seg == nbRadSeg ? 0 : seg;

      float t1 = (float)currSeg / nbRadSeg * _2pi;
      Vector3 r1 = new Vector3(Mathf.Cos(t1) * radius1, 0f, Mathf.Sin(t1) * radius1);

      for (int side = 0; side <= nbSides; side++)
      {
        int currSide = side == nbSides ? 0 : side;

        float t2 = (float)currSide / nbSides * _2pi;
        Vector3 r2 = Quaternion.AngleAxis(-t1 * Mathf.Rad2Deg, Vector3.up) * new Vector3(Mathf.Sin(t2) * radius2, Mathf.Cos(t2) * radius2);

        vertices[side + seg * (nbSides + 1)] = r1 + r2;
      }
    }
    #endregion

    #region Normales
    Vector3[] normales = new Vector3[vertices.Length];
    for (int seg = 0; seg <= nbRadSeg; seg++)
    {
      int currSeg = seg == nbRadSeg ? 0 : seg;

      float t1 = (float)currSeg / nbRadSeg * _2pi;
      Vector3 r1 = new Vector3(Mathf.Cos(t1) * radius1, 0f, Mathf.Sin(t1) * radius1);

      for (int side = 0; side <= nbSides; side++)
      {
        normales[side + seg * (nbSides + 1)] = (vertices[side + seg * (nbSides + 1)] - r1).normalized;
      }
    }
    #endregion

    #region UVs
    Vector2[] uvs = new Vector2[vertices.Length];
    for (int seg = 0; seg <= nbRadSeg; seg++)
      for (int side = 0; side <= nbSides; side++)
        uvs[side + seg * (nbSides + 1)] = new Vector2((float)seg / nbRadSeg, (float)side / nbSides);
    #endregion

    #region Triangles
    int nbFaces = vertices.Length;
    int nbTriangles = nbFaces * 2;
    int nbIndexes = nbTriangles * 3;
    int[] triangles = new int[nbIndexes];

    int i = 0;
    for (int seg = 0; seg <= nbRadSeg; seg++)
    {
      for (int side = 0; side <= nbSides - 1; side++)
      {
        int current = side + seg * (nbSides + 1);
        int next = side + (seg < (nbRadSeg) ? (seg + 1) * (nbSides + 1) : 0);

        if (i < triangles.Length - 6)
        {
          triangles[i++] = current;
          triangles[i++] = next;
          triangles[i++] = next + 1;

          triangles[i++] = current;
          triangles[i++] = next + 1;
          triangles[i++] = current + 1;
        }
      }
    }
    #endregion

    mesh.vertices = vertices;
    mesh.normals = normales;
    mesh.uv = uvs;
    mesh.triangles = triangles;

    mesh.RecalculateBounds();

    return filter;
  }
}
