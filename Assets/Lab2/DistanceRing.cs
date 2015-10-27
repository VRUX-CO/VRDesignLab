using UnityEngine;
using System.Collections;

public class DistanceRing : MonoBehaviour
{
  public Material torusMaterial;
  public GameObject line;
  public float meters;
  public float lineLength;

  // Use this for initialization
  void Start()
  {
    float radius = meters / 2f;
    float radius2 = radius / 20f;
    int nbRadSeg = 44;
    int nbSides = 18;

    MeshFilter filter = MeshUtilities.AddTorusMeshFilter(gameObject, radius, radius2, nbRadSeg, nbSides);
    MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();

    renderer.material = torusMaterial;

    line.transform.localScale = new Vector3(.03f, lineLength, 1);
    line.transform.localPosition = new Vector3(0, lineLength / 2f, radius);
  }

  // Update is called once per frame
  void Update()
  {

  }
}
