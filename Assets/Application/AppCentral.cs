using UnityEngine;
using System.Collections;

public class AppCentral : MonoBehaviour
{
  public bool buildForCardboard = false;
  public GameObject cameraFadeScreenPrefab;
  public GameObject cardboardCameraPrefab;
  public GameObject oculusCameraPrefab;
  public GameObject reticlePrefab;
  bool isCardboard = false;

  static AppCentral app = null;

  // singleton access
  public static AppCentral APP
  {
    get
    {
      if (app == null)
      {
        app = UnityEngine.Object.FindObjectOfType<AppCentral>();
      }

      return app;
    }
  }

  public bool CardboardClickEvent()
  {
    // Cardboard.SDK will lazily load a Cardboard gameobject if called, so we are wrapping all cardboard.sdk calls here
    if (isCardboard)
      return Cardboard.SDK.Triggered;

    return false;
  }

  public void Initialize()
  {
    LevelManager.LM.Initialize(cameraFadeScreenPrefab);

    // install camera for platform
    if (buildForCardboard)
    {
      Instantiate(cardboardCameraPrefab, new Vector3(0, 1.2f, 0f), Quaternion.identity);
      isCardboard = true;
    }
    else
    {
      Instantiate(oculusCameraPrefab, new Vector3(0, 1.2f, 0f), Quaternion.identity);
    }

    // must create reticle after cameras since it trys to access them
    Instantiate(reticlePrefab);
  }

}
