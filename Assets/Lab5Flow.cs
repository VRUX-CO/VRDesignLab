using UnityEngine;
using System.Collections;

public class Lab5Flow : MonoBehaviour
{
  bool trackingEnabled = true;

  // Use this for initialization
  void Start()
  {
    //OVRManager.capiHmd.ConfigureTracking(0, 0);
  }

  // Update is called once per frame
  void Update()
  {
    if (Utilities.UserClicked())
    {
      trackingEnabled = !trackingEnabled;

      AppCentral.APP.DisableHeadTracking(trackingEnabled);
    }
  }
}
