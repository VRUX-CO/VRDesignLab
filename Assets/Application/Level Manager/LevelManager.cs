using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
  // private
  static LevelManager lm = null;
  GameObject cameraFadeScreenPrefab;

  // singleton access
  public static LevelManager LM
  {
    get
    {
      if (lm == null)
      {
        lm = UnityEngine.Object.FindObjectOfType<LevelManager>();
      }
      if (lm == null)
      {
        Debug.Log("Creating LevelManager object");
        var go = new GameObject("LevelManager");
        lm = go.AddComponent<LevelManager>();
        go.transform.localPosition = Vector3.zero;
      }
      return lm;
    }
  }

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
    FadeOut();

    Debug.Log(levelName);

    Application.LoadLevel(levelName);

    FadeIn();
  }

  private CameraFadeScreen GetFadeScreen()
  {
    CameraFadeScreen result = null;

    // are we attached to the current camera?  A scene change loads in a new camera, so we have to always check
    CameraFadeScreen[] fadeScreens = Camera.main.GetComponentsInChildren<CameraFadeScreen>();

    if (fadeScreens.Length > 0)
    {
      result = fadeScreens[0];
      Debug.Log("already there");
    }
    else
    {
      GameObject fadeScreen = Instantiate(cameraFadeScreenPrefab) as GameObject;

      fadeScreen.transform.parent = Camera.main.transform;

      fadeScreen.transform.localPosition = new Vector3(0, 0, .4f);

      result = fadeScreen.GetComponent<CameraFadeScreen>();
    }

    return result;
  }

  public void FadeOut()
  {
    CameraFadeScreen fadeScreen = GetFadeScreen();

    fadeScreen.FadeOut();
  }

  public void FadeIn()
  {
    CameraFadeScreen fadeScreen = GetFadeScreen();

    fadeScreen.FadeIn();
  }
}
