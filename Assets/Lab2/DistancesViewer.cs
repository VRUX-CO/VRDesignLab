using UnityEngine;
using System.Collections;

public class DistancesViewer : MonoBehaviour
{
  public GameObject DistanceRingObject;

  DistanceSign distanceSign;
  int index = 0;
  TextureBillboard introBillboard;

  // Use this for initialization
  void Start()
  {
    GameObject dSign = Instantiate(DistanceRingObject) as GameObject;
    distanceSign = dSign.GetComponent<DistanceSign>();

    // parent so it gets destroyed on scene swap
    dSign.transform.parent = transform;

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
    }
  }

}
