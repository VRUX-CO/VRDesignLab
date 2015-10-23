using UnityEngine;
using System.Collections;

public class DistanceRing : MonoBehaviour
{
  public Material torusMaterial;

  // Use this for initialization
  void Start()
  {
    MeshFilter filter = MeshUtilities.AddTorusMeshFilter(gameObject);

    MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();

    renderer.material = torusMaterial;
  }

  // Update is called once per frame
  void Update()
  {

  }
}
