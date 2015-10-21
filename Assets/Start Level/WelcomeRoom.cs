using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WelcomeRoom : MonoBehaviour
{
  public GameObject proceduralRoomPrefab;
  public GameObject welcomeSignPrefab;
  public GameObject iconButtonBarPrefab;
  public GameObject levelMenuPrefab;

  GameObject proceduralRoom, welcomeSign, iconButtonBar, levelMenu;

  Material signMaterial;
  bool dismissedSign = false;
  int step = 0;

  // update these if wall size changes
  const float wallDimension = 12;
  const float zOffset = wallDimension / 2;

  // Use this for initialization
  void Start()
  {
    Vector3 roomPosition = new Vector3(0, 0f, -zOffset);
    Vector3 contentPosition = new Vector3(0, 1.2f, 2);

    proceduralRoom = Instantiate(proceduralRoomPrefab, roomPosition, Quaternion.identity) as GameObject;
    welcomeSign = Instantiate(welcomeSignPrefab, contentPosition, Quaternion.identity) as GameObject;
    iconButtonBar = Instantiate(iconButtonBarPrefab, contentPosition, Quaternion.identity) as GameObject;
    levelMenu = Instantiate(levelMenuPrefab, contentPosition, Quaternion.identity) as GameObject;

    // these prefabs start off inactive, so turn them on as needed
    welcomeSign.SetActive(true);

    // setup callback for button bar
    IconButtonBar btnBar = iconButtonBar.GetComponent<IconButtonBar>();
    btnBar.clickDelegate = gameObject;
    btnBar.clickCallback = "ButtonBarClickCallback";

    LevelMenu menu = levelMenu.GetComponent<LevelMenu>();
    menu.clickDelegate = gameObject;
    menu.clickCallback = "MenuCallback";

    signMaterial = welcomeSign.GetComponent<MeshRenderer>().material;
  }

  // Update is called once per frame
  void Update()
  {
    if (Utilities.UserClicked())
    {
      switch (step)
      {
        case 0:  // showing welcome sign
          FadeOutSign();
          break;
        case 1:  // showing icon bar
          break;
        case 2:  // showing level menu
          break;
      }
    }
  }

  public void ShowHome()
  {
    proceduralRoom.SetActive(true);
    proceduralRoom.GetComponent<ProceduralRoom>().SetColor(new Color(1, 1, 1, 1));

    FadeInButtonBar();
  }

  // -----------------------------------------------------

  void FadeOutSign()
  {
    if (!dismissedSign)
    {
      dismissedSign = true;
      iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeOutSignUpdate", "time", .5f, "oncomplete", "FadeOutSignDone"));
    }
  }

  public void FadeOutSignUpdate(float progress)
  {
    signMaterial.color = new Color(1, 1, 1, progress);
  }

  public void FadeOutSignDone()
  {
    Destroy(welcomeSign);
    welcomeSign = null;

    // show button bar
    FadeInButtonBar();
  }

  // -----------------------------------------------------
  void FadeInButtonBar()
  {
    iconButtonBar.SetActive(true);
    // iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeInButtonBarUpdate", "time", 3.5f));
  }

  public void FadeInButtonBarUpdate(float progress)
  {
  }

  // -----------------------------------------------------

  void FadeOutRoom()
  {
    iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeOutRoomUpdate", "time", 3.5f, "oncomplete", "FadeOutRoomDone"));
  }

  public void FadeOutRoomUpdate(float progress)
  {
    proceduralRoom.GetComponent<ProceduralRoom>().SetColor(new Color(1, 1, 1, progress));
  }

  public void FadeOutRoomDone()
  {
    proceduralRoom.SetActive(false);
  }

  // -----------------------------------------------------


  void ButtonBarClickCallback(string buttonID)
  {
    FadeOutRoom();
    iconButtonBar.SetActive(false);

    if (buttonID.Equals("Foundation"))
    {
      BuildMenuItems(1);

      levelMenu.SetActive(true);
    }
    else if (buttonID.Equals("Immersion"))
    {
      BuildMenuItems(0);

      levelMenu.SetActive(true);
    }
  }

  void MenuCallback(string command)
  {
    switch (command)
    {
      // Foundation menu
      case "reticle":
        AppCentral.APP.LoadLevel("VRDL_Lab1");
        break;
      case "depth":
        AppCentral.APP.LoadLevel("VRDL_Lab2");
        break;
      case "velocity":
        AppCentral.APP.LoadLevel("VRDL_Lab3");
        break;
      case "grounded":
        AppCentral.APP.LoadLevel("VRDL_Lab4");
        break;
      case "tracking":
        AppCentral.APP.LoadLevel("VRDL_Lab5");
        break;

      // Immersion menu
      case "light":
        AppCentral.APP.LoadLevel("VRDL_Lab1");
        break;
      case "scale":
        AppCentral.APP.LoadLevel("VRDL_Lab1");
        break;
      case "audio":
        AppCentral.APP.LoadLevel("VRDL_Lab1");
        break;
      case "gaze":
        AppCentral.APP.LoadLevel("VRDL_Lab1");
        break;
      case "beautiful":
        AppCentral.APP.LoadLevel("VRDL_Lab1");
        break;

      case "back":  // go back item
        Debug.Log("go back");
        break;
    }
  }

  void BuildMenuItems(int menuID)
  {
    Dictionary<string, string> newItem;
    LevelMenu menu = levelMenu.GetComponent<LevelMenu>();
    List<Dictionary<string, string>> menuItems = new List<Dictionary<string, string>>();

    if (menuID == 0)
    {
      newItem = new Dictionary<string, string>();
      newItem["name"] = "1. Using a Reticle";
      newItem["cmd"] = "reticle";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "2. UI Depth & Eye Strain";
      newItem["cmd"] = "depth";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "3. Using Constant Velocity";
      newItem["cmd"] = "velocity";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "4. Keep the User Grounded";
      newItem["cmd"] = "grounded";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "5. Maintaining Head Tracking";
      newItem["cmd"] = "tracking";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "< Go Back";
      newItem["cmd"] = "back";
      menuItems.Add(newItem);
    }
    else if (menuID == 1)
    {
      newItem = new Dictionary<string, string>();
      newItem["name"] = "6. Guiding with Light";
      newItem["cmd"] = "light";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "7. Leveraging Scale";
      newItem["cmd"] = "scale";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "8. Spatial Audio";
      newItem["cmd"] = "audio";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "9. Gaze Cues";
      newItem["cmd"] = "gaze";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "10. Make it Beautiful";
      newItem["cmd"] = "beautiful";
      menuItems.Add(newItem);

      newItem = new Dictionary<string, string>();
      newItem["name"] = "< Go Back";
      newItem["cmd"] = "back";
      menuItems.Add(newItem);
    }

    menu.SetupItems(menuItems);
  }

}
