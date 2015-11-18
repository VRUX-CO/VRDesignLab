using UnityEngine;
using System.Collections;

public class DistancesViewer : MonoBehaviour
{
  public GameObject DistanceRingObject;

  DistanceSign distanceSign;
  int index = 0;
  GameObject currentSign;

  // Use this for initialization
  void Start()
  {
    currentSign = Instantiate(DistanceRingObject) as GameObject;
    distanceSign = currentSign.GetComponent<DistanceSign>();

    // parent so it gets destroyed on scene swap
    currentSign.transform.parent = transform;

    Next();
  }

  // Update is called once per frame
  void Update()
  {
    if (Utilities.UserClicked())
    {
      Next();
    }
  }

  void Next()
  {
    // update distance loop
    bool success = distanceSign.Show(index);

    if (success)
    {
      index++;
    }
    else // failed, so show them all
    {
      index = 0;

      distanceSign.ShowAll();
    }
  }

}
