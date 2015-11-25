using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnvironmentEnum
{
  kNone,
  kMountains,
  kForest
}

public class AppCentral : MonoBehaviour
{
  public bool buildForCardboard = false;
  public bool isMainScene = false;
  public GameObject cardboardCameraPrefab;
  public GameObject oculusCameraPrefab;
  public GameObject reticlePrefab;
  public GameObject lookdownMenuPrefab;
  public GameObject lookdownNotifierPrefab;
  public GameObject cameraFadeScreenPrefab;
  public GameObject mainScenePrefab;
  public GameObject mountainPrefab;
  public GameObject forestPrefab;

  bool savedReticleClickOnlyState;
  bool isCardboard = false;
  LevelManager levelManager;
  WelcomeRoom mainScene;
  Crosshair3D reticle;
  GameObject lookDownMenu = null;
  GameObject environment = null;
  string currentCategory;

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

  void Update()
  {
    // quit game when escape key is pressed
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      Application.Quit();
    }
    else if (Input.GetKeyDown(KeyCode.R))
      RecenterHeadTracking();
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
    const float cameraHeight = 1.5f;

    // install camera for platform
    if (buildForCardboard)
    {
      Instantiate(cardboardCameraPrefab, new Vector3(0, cameraHeight, 0f), Quaternion.identity);
      isCardboard = true;

      Cardboard.SDK.EnableSettingsButton = false;
    }
    else
    {
      Instantiate(oculusCameraPrefab, new Vector3(0, cameraHeight, 0f), Quaternion.identity);
    }

    // must create reticle after cameras since it trys to access them
    reticle = Instantiate(reticlePrefab).GetComponent<Crosshair3D>();

    mainScene = Instantiate(mainScenePrefab).GetComponent<WelcomeRoom>();

    mainScene.gameObject.SetActive(isMainScene);

    // add level manager to app
    levelManager = gameObject.AddComponent<LevelManager>();
    levelManager.Initialize(cameraFadeScreenPrefab);

    ShowEnvironment(EnvironmentEnum.kMountains);
  }

  public void LoadLevel(string levelName)
  {
    // reset this if set by the level
    ShowReticleOnClick(false);

    levelManager.LoadLevel(levelName);
  }

  public List<Dictionary<string, string>> MenuItems(string category)
  {
    currentCategory = category;  // save this for the next button

    return levelManager.MenuItems(category);
  }

  void NextLevel()
  {
    // reset this if set by the level
    ShowReticleOnClick(false);

    levelManager.LoadNextLevel(currentCategory);
  }

  public void InstallLookdownMenu()
  {
    if (lookDownMenu == null)
      lookDownMenu = Instantiate(lookdownMenuPrefab, new Vector3(0f, .01f, 1f), Quaternion.identity) as GameObject;
  }

  public void ShowEnvironment(EnvironmentEnum env)
  {
    // destroy existing
    if (environment != null)
    {
      Destroy(environment);
      environment = null;
    }

    switch (env)
    {
      default:
      case EnvironmentEnum.kNone:
        break;
      case EnvironmentEnum.kMountains:
        environment = Instantiate(mountainPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        break;
      case EnvironmentEnum.kForest:
        environment = Instantiate(forestPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        break;

    }
  }

  public void HandleNavigation(string navigationID)
  {
    switch (navigationID)
    {
      case "Reset":
        break;
      case "Home":
        ResetToHomeState();
        break;
      case "Next":
        NextLevel();
        break;
    }
  }

  void ResetToHomeState()
  {
    levelManager.UnloadLevel(null);

    // reset this if set by the level
    ShowReticleOnClick(false);

    // restore state to main icon bar
    mainScene.ShowHome();

    // remove look down menu
    Destroy(lookDownMenu);
    lookDownMenu = null;
  }

  public void ShowLookdownNotifier()
  {
    // deletes self when done
    Instantiate(lookdownNotifierPrefab);
  }

  public void ShowReticleOnClick(bool showOnClick)
  {
    reticle.SetShowReticleOnClick(showOnClick);
  }

  public void DisableHeadTracking(bool disable)
  {
    if (isCardboard)
    {
      CardboardHead head = Cardboard.Controller.Head;

      head.trackRotation = !disable;
      head.trackPosition = !disable;
    }
    else
    {
      CameraMount mount = Camera.main.transform.parent.GetComponent<CameraMount>();
      if (mount != null)
      {
        mount.DiabledTracking(disable);
      }
    }
  }

  public void RecenterHeadTracking()
  {
    if (isCardboard)
    {
      Cardboard.SDK.Recenter();
    }
    else
    {
      UnityEngine.VR.InputTracking.Recenter();
    }
  }

  // always show crosshair if turned off for reticle lab or future lab
  public void ShowCrosshairIfHidden(bool start)
  {
    if (start)
    {
      savedReticleClickOnlyState = reticle.GetShowReticleOnClick();

      reticle.SetShowReticleOnClick(false);  // false, true is the hidden mode
    }
    else
    {
      reticle.SetShowReticleOnClick(savedReticleClickOnlyState);
    }
  }

}
