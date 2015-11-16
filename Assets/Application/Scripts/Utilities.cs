using UnityEngine;
using System.Collections;

public class Utilities : MonoBehaviour
{
  public static bool UserClicked()
  {
    if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump") ||
AppCentral.APP.CardboardClickEvent() || (Input.GetMouseButtonDown(0)))
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

  public static void RotateToFaceCamera(Transform trans, Camera camera, bool xAxis, bool yAxis, bool zAxis)
  {
    // only rotates on y axis
    Quaternion r1 = Quaternion.LookRotation(trans.position - Camera.main.transform.position, Vector3.up);
    Vector3 euler2 = trans.eulerAngles;

    if (xAxis)
      euler2.x = r1.eulerAngles.x;
    if (yAxis)
      euler2.y = r1.eulerAngles.y;
    if (zAxis)
      euler2.z = r1.eulerAngles.z;

    trans.rotation = Quaternion.Euler(euler2.x, euler2.y, euler2.z);
  }


  public static void PlaySound(AudioClip clip)
  {
    GameObject go = new GameObject();

    AudioSource audio = go.AddComponent<AudioSource>();
    audio.clip = clip;
    audio.Play();

    Destroy(go, audio.clip.length);
  }

  // stupid debug tool.  throw out a cube to verify we hit a codepath. returns where cube was placed and Debug.Logs it
  static float cnt = 0;
  public static Vector3 DebugCube(string message)
  {
    Vector3 result = new Vector3(0, cnt, 5);

    GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = result;
    cnt += 1.1f;

    Debug.Log(message + result.ToString());

    return result;
  }


}
