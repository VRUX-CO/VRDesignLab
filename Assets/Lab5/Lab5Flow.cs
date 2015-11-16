using UnityEngine;
using System.Collections;

public class Lab5Flow : MonoBehaviour
{
  bool trackingDisabled = false;

  // Use this for initialization
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    if (Utilities.UserClicked())
    {
      StartCoroutine(DisableTrackingForSeconds(2));
    }
  }

  IEnumerator DisableTrackingForSeconds(float delay)
  {
    if (!trackingDisabled)
    {
      ToggleTracking();

      yield return new WaitForSeconds(delay);

      if (trackingDisabled)
        ToggleTracking();
    }

    yield return null;
  }

  void ToggleTracking()
  {
    trackingDisabled = !trackingDisabled;

    AppCentral.APP.DisableHeadTracking(trackingDisabled);
  }
}
