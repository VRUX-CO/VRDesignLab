using UnityEngine;
using System.Collections;

public class AppCentral : MonoBehaviour
{
  public bool buildForCardboard = false;
  public GameObject cardboardCameraPrefab;
  public GameObject oculusCameraPrefab;
  public GameObject reticlePrefab;
  public GameObject lookdownMenuPrefab;
  public GameObject lookdownNotifierPrefab;
  public GameObject cameraFadeScreenPrefab;
  public GameObject mainScenePrefab;

  bool isCardboard = false;
  LevelManager levelManager;
  GameObject lookdownNotifier;
  WelcomeRoom mainScene;

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

    Instantiate(lookdownMenuPrefab);
    lookdownNotifier = Instantiate(lookdownNotifierPrefab);
    mainScene = Instantiate(mainScenePrefab).GetComponent<WelcomeRoom>();

    // add level manager to app
    levelManager = gameObject.AddComponent<LevelManager>();
    levelManager.Initialize(cameraFadeScreenPrefab);
  }

  public void LoadLevel(string levelName)
  {
    levelManager.LoadLevel(levelName);
  }

  public void HandleNavigation(string navigationID)
  {
    switch (navigationID)
    {
      case "Reset":
        Debug.Log("reset");
        break;
      case "Home":
        Debug.Log("home");
        levelManager.UnloadLevel(null);

        // restore state to main icon bar
        mainScene.ShowHome();

        break;
      case "Next":
        Debug.Log("next");
        break;
    }
  }

  public void ShowLookdownNotifier()
  {
    lookdownNotifier.SetActive(true);
  }
}
