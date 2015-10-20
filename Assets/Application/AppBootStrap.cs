using UnityEngine;
using System.Collections;

public class AppBootStrap : MonoBehaviour
{
  public bool buildForCardboard = false;
  public GameObject cameraFadeScreenPrefab;
  public GameObject cardboardCameraPrefab;
  public GameObject oculusCameraPrefab;
  public GameObject reticlePrefab;

  // singleton access
  void Awake()
  {
    // does an AppCentral Already exist?
    AppCentral app = UnityEngine.Object.FindObjectOfType<AppCentral>();

    if (app == null)
    {
      GameObject appGameObj = new GameObject("AppCentral");
      app = appGameObj.gameObject.AddComponent<AppCentral>();
      app.buildForCardboard = buildForCardboard;
      app.cameraFadeScreenPrefab = cameraFadeScreenPrefab;
      app.cardboardCameraPrefab = cardboardCameraPrefab;
      app.oculusCameraPrefab = oculusCameraPrefab;
      app.reticlePrefab = reticlePrefab;

      app.Initialize();
    }

    // we are done
    Destroy(gameObject);
  }

}
