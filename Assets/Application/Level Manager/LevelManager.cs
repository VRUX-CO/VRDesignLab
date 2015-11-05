using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
  // private
  GameObject cameraFadeScreenPrefab;
  string levelToLoad = null;
  string currentLoadedLevel = null;

  void Awake()
  {
    // stay alive always so we can loadLevels with fadein/out and not get destroyed ourselves.
    DontDestroyOnLoad(gameObject);
  }

  public void Initialize(GameObject fadeScreenPrefab)
  {
    cameraFadeScreenPrefab = fadeScreenPrefab;
  }

  // pass null to remove the current Loaded Level
  public void UnloadLevel(string levelName)
  {
    // unload previous level
    if (currentLoadedLevel != null)
    {
      Application.UnloadLevel(currentLoadedLevel);
      currentLoadedLevel = null;
    }
  }

  public void LoadLevel(string levelName)
  {
    levelToLoad = levelName;
    GetFadeScreen().FadeOut("FadeOutDone");
  }

  public void LoadNextLevel()
  {
    int index = IndexForLevel(currentLoadedLevel);

    LoadLevel(LevelNameForIndex(index + 1));
  }

  public void FadeOutDone()
  {
    UnloadLevel(null); // null unloads currentLoadedLevel

    // synchronous
    Application.LoadLevelAdditive(levelToLoad);
    currentLoadedLevel = levelToLoad;

    GetFadeScreen().FadeIn();
  }

  private CameraFadeScreen GetFadeScreen()
  {
    CameraFadeScreen result = null;

    // are we attached to the current camera?  A scene change loads in a new camera, so we have to always check
    CameraFadeScreen[] fadeScreens = Camera.main.GetComponentsInChildren<CameraFadeScreen>();

    if (fadeScreens.Length > 0)
    {
      result = fadeScreens[0];
    }
    else
    {
      GameObject fadeScreen = Instantiate(cameraFadeScreenPrefab) as GameObject;

      fadeScreen.transform.parent = Camera.main.transform;

      fadeScreen.transform.localPosition = new Vector3(0, 0, .4f);

      result = fadeScreen.GetComponent<CameraFadeScreen>();

      result.levelManager = this;
    }

    return result;
  }

  // this complexity was added for the next button
  int IndexForLevel(string levelName)
  {
    int result;
    switch (levelName)
    {
      default:
      case "VRDL_Lab1":
        result = 0;
        break;
      case "VRDL_Lab2":
        result = 1;
        break;
      case "VRDL_Lab3":
        result = 2;
        break;
      case "VRDL_Lab4":
        result = 3;
        break;
      case "VRDL_Lab5":
        result = 4;
        break;
    }

    return result;
  }

  // this complexity was added for the next button
  string LevelNameForIndex(int index)
  {
    string result;
    switch (index)
    {
      default:
      case 0:
        result = "VRDL_Lab1";
        break;
      case 1:
        result = "VRDL_Lab2";
        break;
      case 2:
        result = "VRDL_Lab3";
        break;
      case 3:
        result = "VRDL_Lab4";
        break;
      case 4:
        result = "VRDL_Lab5";
        break;
    }

    return result;
  }
}

