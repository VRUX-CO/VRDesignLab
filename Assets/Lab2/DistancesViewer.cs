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
    GameObject obj = Instantiate(DistanceRingObject) as GameObject;
    distanceSign = obj.GetComponent<DistanceSign>();

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
    distanceSign.Show(index, true);

    index++;
  }

}
