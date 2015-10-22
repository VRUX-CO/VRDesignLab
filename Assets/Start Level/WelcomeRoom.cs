using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WelcomeRoom : MonoBehaviour
{
  public GameObject proceduralRoomPrefab;
  public GameObject welcomeSignPrefab;
  public GameObject iconButtonBarPrefab;
  public GameObject levelMenuPrefab;

  GameObject proceduralRoom, welcomeSign, levelMenu, iconButtonBar;

  // used when fading out only
  GameObject fadingGameObject;
  ProceduralRoom fadingProceduralRoom;

  Material signMaterial;
  bool dismissedSign = false;
  int step = 0;

  // Use this for initialization
  void Start()
  {
    // not reticle on start screen
    AppCentral.APP.ShowReticleOnClick(true);

    CreateRoom();
    welcomeSign = Instantiate(welcomeSignPrefab, ContentPosition(), Quaternion.identity) as GameObject;

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

  Vector3 ContentPosition()
  {
    return new Vector3(0, 1.2f, 2);
  }

  void CreateRoom()
  {
    if (proceduralRoom == null)
    {
      // update these if wall size changes
      const float wallDimension = 12;
      const float zOffset = wallDimension / 2;

      Vector3 roomPosition = new Vector3(0, 0f, -zOffset);
      proceduralRoom = Instantiate(proceduralRoomPrefab, roomPosition, Quaternion.identity) as GameObject;
    }
  }

  public void ShowHome()
  {
    CreateRoom();
    DestroyLevelMenu();
    DestroyIconBar();
    ShowButtonBar();
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

    AppCentral.APP.ShowReticleOnClick(false);

    // show button bar
    ShowButtonBar();
  }

  // -----------------------------------------------------
  void ShowButtonBar()
  {
    iconButtonBar = Instantiate(iconButtonBarPrefab, ContentPosition(), Quaternion.identity) as GameObject;

    // setup callback for button bar
    IconButtonBar btnBar = iconButtonBar.GetComponent<IconButtonBar>();
    btnBar.clickDelegate = gameObject;
    btnBar.clickCallback = "ButtonBarClickCallback";
  }

  // -----------------------------------------------------

  void FadeOutRoom()
  {
    if (proceduralRoom != null)
    {
      if (fadingGameObject == null)
      {
        fadingGameObject = proceduralRoom;
        proceduralRoom = null;

        fadingProceduralRoom = fadingGameObject.GetComponent<ProceduralRoom>();

        iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeOutRoomUpdate", "time", 2f, "oncomplete", "FadeOutRoomDone"));
      }
      else
      {
        // don't bother fading if already in progress
        Destroy(proceduralRoom);
        proceduralRoom = null;
      }
    }
  }

  public void FadeOutRoomUpdate(float progress)
  {
    fadingProceduralRoom.SetColor(new Color(1, 1, 1, progress));
  }

  public void FadeOutRoomDone()
  {
    Destroy(fadingGameObject);

    fadingGameObject = null;
    fadingProceduralRoom = null;
  }

  // -----------------------------------------------------

  void ButtonBarClickCallback(string buttonID)
  {
    FadeOutRoom();

    levelMenu = Instantiate(levelMenuPrefab, ContentPosition(), Quaternion.identity) as GameObject;

    LevelMenu menu = levelMenu.GetComponent<LevelMenu>();
    menu.clickDelegate = gameObject;
    menu.clickCallback = "MenuCallback";

    if (buttonID.Equals("Foundation"))
    {
      BuildMenuItems(menu, 0);
    }
    else if (buttonID.Equals("Immersion"))
    {
      BuildMenuItems(menu, 1);
    }

    DestroyIconBar();

    // install the lookdown menu at this point
    AppCentral.APP.InstallLookdownMenu();
  }

  void DestroyLevelMenu()
  {
    if (levelMenu != null)
    {
      Destroy(levelMenu);
      levelMenu = null;
    }
  }

  void DestroyIconBar()
  {
    if (iconButtonBar != null)
    {
      Destroy(iconButtonBar);
      iconButtonBar = null;
    }
  }

  void MenuCallback(string command)
  {
    DestroyLevelMenu();

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
        AppCentral.APP.HandleNavigation("Home");
        break;
    }
  }

  void BuildMenuItems(LevelMenu menu, int menuID)
  {
    Dictionary<string, string> newItem;
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
