using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
  // private
  GameObject cameraFadeScreenPrefab;
  string levelToLoad = null;
  string currentLoadedLevel = null;
  List<Dictionary<string, string>> menuItems;

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

  public void LoadNextLevel(string category)
  {
    int index = IndexForLevel(currentLoadedLevel, category);

    LoadLevel(LevelNameForIndex(index + 1, category));
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
  int IndexForLevel(string levelName, string category)
  {
    List<Dictionary<string, string>> menuItems = MenuItems(category);

    int result = -1;
    int index = 0;

    foreach (Dictionary<string, string> item in menuItems)
    {
      if (item["scene"].Equals(levelName))
      {
        result = index;
        break;
      }

      index++;
    }

    return result;
  }

  // this complexity was added for the next button
  string LevelNameForIndex(int index, string category)
  {
    List<Dictionary<string, string>> menuItems = MenuItems(category);

    string result = menuItems[index]["scene"];

    return result;
  }

  public List<Dictionary<string, string>> MenuItems(string category)
  {
    List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
    List<Dictionary<string, string>> allItems;

    allItems = AllMenuItems();
    foreach (Dictionary<string, string> item in allItems)
    {
      if (category.Equals(item["category"]))
      {
        result.Add(item);
      }
    }

    // add back item
    result.Add(BackMenuItem());

    return result;
  }

  List<Dictionary<string, string>> AllMenuItems()
  {
    if (menuItems == null)
    {
      Dictionary<string, string> newItem;
      List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

      newItem = new Dictionary<string, string>();
      newItem["name"] = "1. Using a Reticle";
      newItem["cmd"] = "reticle";
      newItem["scene"] = "VRDL_Lab1";
      newItem["category"] = "1";
      result.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "2. UI Depth & Eye Strain";
      newItem["cmd"] = "depth";
      newItem["scene"] = "VRDL_Lab2";
      newItem["category"] = "1";
      result.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "3. Using Constant Velocity";
      newItem["cmd"] = "velocity";
      newItem["scene"] = "VRDL_Lab3";
      newItem["category"] = "1";
      result.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "4. Keep the User Grounded";
      newItem["cmd"] = "grounded";
      newItem["scene"] = "VRDL_Lab4";
      newItem["category"] = "1";
      result.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "5. Maintaining Head Tracking";
      newItem["cmd"] = "tracking";
      newItem["scene"] = "VRDL_Lab5";
      newItem["category"] = "1";
      result.Add(newItem);

      // category 2
      newItem = new Dictionary<string, string>();
      newItem["name"] = "6. Guiding with Light";
      newItem["cmd"] = "light";
      newItem["scene"] = "VRDL_Lab101";
      newItem["category"] = "2";
      result.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "7. Leveraging Scale";
      newItem["cmd"] = "scale";
      newItem["scene"] = "VRDL_Lab101";
      newItem["category"] = "2";
      result.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "8. Spatial Audio";
      newItem["cmd"] = "audio";
      newItem["scene"] = "VRDL_Lab101";
      newItem["category"] = "2";
      result.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "9. Gaze Cues";
      newItem["cmd"] = "gaze";
      newItem["scene"] = "VRDL_Lab101";
      newItem["category"] = "2";
      result.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "10. Make it Beautiful";
      newItem["cmd"] = "beautiful";
      newItem["scene"] = "VRDL_Lab101";
      newItem["category"] = "2";
      result.Add(newItem);

      menuItems = result;
    }

    return menuItems;
  }

  Dictionary<string, string> BackMenuItem()
  {
    Dictionary<string, string> newItem;

    newItem = new Dictionary<string, string>();
    newItem["name"] = "< Go Back";
    newItem["cmd"] = "back";

    return newItem;
  }
}

