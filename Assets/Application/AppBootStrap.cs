using UnityEngine;
using System.Collections;

public class AppBootStrap : MonoBehaviour
{
  public bool buildForCardboard = false;
  public GameObject cameraFadeScreenPrefab;
  public GameObject cardboardCameraPrefab;
  public GameObject oculusCameraPrefab;
  public GameObject reticlePrefab;
  public GameObject lookdownMenuPrefab;
  public GameObject lookdownNotifierPrefab;
  public GameObject mainScenePrefab;

  // singleton access
  void Awake()
  {
    // does an AppCentral Already exist?
    AppCentral app = UnityEngine.Object.FindObjectOfType<AppCentral>();

    if (app == null)
    {
      GameObject appGameObj = new GameObject("AppCentral");

      // add appcentral
      app = appGameObj.AddComponent<AppCentral>();
      app.buildForCardboard = buildForCardboard;
      app.cardboardCameraPrefab = cardboardCameraPrefab;
      app.oculusCameraPrefab = oculusCameraPrefab;
      app.reticlePrefab = reticlePrefab;
      app.lookdownMenuPrefab = lookdownMenuPrefab;
      app.lookdownNotifierPrefab = lookdownNotifierPrefab;
      app.cameraFadeScreenPrefab = cameraFadeScreenPrefab;
      app.mainScenePrefab = mainScenePrefab;
      app.Initialize();
    }

    // we are done
    Destroy(gameObject);
  }

}
