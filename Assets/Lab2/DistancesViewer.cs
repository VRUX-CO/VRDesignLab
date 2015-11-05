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

    // maintain distance to camera without attaching
    if (currentSign != null)
    {
      Vector3 newPosition = new Vector3(0, 0, Camera.main.transform.position.z);

      currentSign.transform.localPosition = newPosition;
    }
  }

  void Next()
  {
    // update distance loop
    distanceSign.Show(index, true);

    index++;
  }

}
