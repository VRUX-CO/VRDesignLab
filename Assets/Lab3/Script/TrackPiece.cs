using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackPiece : MonoBehaviour
{
  public bool reversePoints;

  // Use this for initialization
  public List<Transform> trackPoints()
  {
    List<Transform> result = new List<Transform>();

    for (int x = 0; x < transform.childCount; x++)
    {
      result.Add(transform.GetChild(x));
    }

    if (reversePoints)
      result.Reverse();

    return result;
  }

}
