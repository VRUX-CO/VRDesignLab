using UnityEngine;
using System.Collections;

public class DistancesViewer : MonoBehaviour
{
  public Material introSignMaterial;
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

    introBillboard = TextureBillboard.ShowBillboard(introSignMaterial, new Vector3(1, 1, 1), 1, new Vector3(0, 1.5f, 2f), transform);
  }

  // Update is called once per frame
  void Update()
  {
    if (Utilities.UserClicked())
    {
      if (introBillboard != null)
      {
        introBillboard.Hide(.2f);
        introBillboard = null;
      }

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
      index = -1;

      distanceSign.ShowAll();
    }
  }

}
