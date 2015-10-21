using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
  // private
  static LevelManager lm = null;
  GameObject cameraFadeScreenPrefab;
  string levelToLoad = null;
  string loadedLevel = null;

  void Awake()
  {
    // stay alive always so we can loadLevels with fadein/out and not get destroyed ourselves.
    DontDestroyOnLoad(gameObject);
  }

  public void Initialize(GameObject fadeScreenPrefab)
  {
    cameraFadeScreenPrefab = fadeScreenPrefab;
  }

  public void LoadLevel(string levelName)
  {
    levelToLoad = levelName;
    GetFadeScreen().FadeOut("FadeOutDone");
  }

  public void FadeOutDone()
  {
    // unload previous level
    if (loadedLevel != null)
    {
      Application.UnloadLevel(loadedLevel);
      loadedLevel = null;
    }

    // synchronous
    Application.LoadLevelAdditive(levelToLoad);
    loadedLevel = levelToLoad;

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
}
