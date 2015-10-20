using UnityEngine;
using System.Collections;

public class Utilities : MonoBehaviour
{
  public static bool UserClicked()
  {
    if (Input.GetButtonDown(OVRGamepadController.ButtonNames[(int)OVRGamepadController.Button.A]) ||   // "Desktop_Button A"
Input.GetButtonDown("Button A") ||
AppCentral.APP.CardboardClickEvent())
    {
      return true;
    }

    return false;
  }

  public static void ReverseNormals(GameObject gameObject)
  {
    MeshFilter filter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
    if (filter != null)
    {
      Mesh mesh = filter.mesh;

      Vector3[] normals = mesh.normals;
      for (int i = 0; i < normals.Length; i++)
        normals[i] = -normals[i];
      mesh.normals = normals;

      for (int m = 0; m < mesh.subMeshCount; m++)
      {
        int[] triangles = mesh.GetTriangles(m);
        for (int i = 0; i < triangles.Length; i += 3)
        {
          int temp = triangles[i + 0];
          triangles[i + 0] = triangles[i + 1];
          triangles[i + 1] = temp;
        }
        mesh.SetTriangles(triangles, m);
      }
    }
  }

  public static void RotateToFaceCamera(Transform transform, Camera camera)
  {
    transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
        camera.transform.rotation * Vector3.up);
  }


}
