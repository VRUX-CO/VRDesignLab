using UnityEngine;
using System.Collections;

public class Lab5Controller : MonoBehaviour
{
  public Material trackingOffMat;
  public Material instructionsMat;

  Material fadeMaterial;

  bool trackingDisabled = false;

  // Use this for initialization
  void Start()
  {
    TextureBillboard.Billboard(instructionsMat, new Vector3(1, 1, 1), 1, new Vector3(0, 1.4f, 1.5f), transform, true).Show(0f);
  }

  void OnDestroy()
  {
    if (trackingDisabled)
    {
      ToggleTracking();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (AppCentral.APP.UserClicked())
    {
      StartCoroutine(DisableTrackingForSeconds(2));
    }
  }

  IEnumerator DisableTrackingForSeconds(float delay)
  {
    if (!trackingDisabled)
    {
      TextureBillboard tbb = TextureBillboard.Billboard(trackingOffMat, new Vector3(.8f, .4f, .8f), 0, new Vector3(0, 1.8f, 1.5f), transform, true);
      tbb.Show(0f);

      ToggleTracking();

      yield return new WaitForSeconds(delay);

      if (trackingDisabled)
      {
        ToggleTracking();

        tbb.Hide(.25f);

        yield return new WaitForSeconds(.5f);

        AppCentral.APP.ShowLookdownNotifier();
      }
    }

    yield return null;
  }

  void ToggleTracking()
  {
    trackingDisabled = !trackingDisabled;

    AppCentral.APP.DisableHeadTracking(trackingDisabled);
  }

}
