using UnityEngine;
using System.Collections;

public class DistanceRing : MonoBehaviour
{
  public Material torusMaterial;

  public void UpdateDistance(float distance)
  {
    MeshFilter filter;
    float radius = distance;
    float radius2 = radius / 30f;
    int nbRadSeg = 44;
    int nbSides = 18;

    filter = MeshUtilities.AddTorusMeshFilter(gameObject, radius, radius2, nbRadSeg, nbSides);

    MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
    renderer.material = torusMaterial;
  }
}
