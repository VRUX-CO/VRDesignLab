using UnityEngine;
using System.Collections;

public class DistanceRing : MonoBehaviour
{
  public Material goodMaterial;
  public Material warningMaterial;
  public float lineLength;

  GameObject ringObject;
  GameObject lineObject;
  GameObject sphereObject;

  GameObject CreateRing(float radius)
  {
    float innerRadius = .01f;
    int nbRadSeg = 44;
    int nbSides = 18;

    GameObject result = new GameObject();

    MeshFilter filter = MeshUtilities.AddTorusMeshFilter(result, radius, innerRadius, nbRadSeg, nbSides);

    MeshRenderer renderer = result.AddComponent<MeshRenderer>();

    renderer.material = goodMaterial;
    if (radius < 1f || radius > 5)
    {
      renderer.material = warningMaterial;
    }

    return result;
  }

  public void UpdateRadius(float radius)
  {
    if (ringObject != null)
      Destroy(ringObject);
    if (lineObject != null)
      Destroy(lineObject);
    if (sphereObject != null)
      Destroy(sphereObject);

    ringObject = CreateRing(radius);
    lineObject = CreateLine(radius);
    sphereObject = CreateSphere(radius);
  }

  GameObject CreateLine(float radius)
  {
    GameObject result = GameObject.CreatePrimitive(PrimitiveType.Quad);

    MeshRenderer renderer = result.GetComponent<MeshRenderer>();

    renderer.material = goodMaterial;
    if (radius < 1f || radius > 5)
    {
      renderer.material = warningMaterial;
    }

    result.transform.position = new Vector3(0, lineLength / 2f, radius);
    result.transform.localScale = new Vector3(.01f, lineLength, 1);

    return result;
  }

  GameObject CreateSphere(float radius)
  {
    GameObject result = GameObject.CreatePrimitive(PrimitiveType.Sphere);

    MeshRenderer renderer = result.GetComponent<MeshRenderer>();

    renderer.material = goodMaterial;
    if (radius < 1f || radius >= 5f)
    {
      renderer.material = warningMaterial;
    }

    result.transform.position = new Vector3(0, 0, radius);
    result.transform.localScale = new Vector3(.05f, .05f, .05f);

    return result;
  }
}
