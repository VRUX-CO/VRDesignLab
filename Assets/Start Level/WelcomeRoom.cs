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
      BuildMenuItems(menu, "1");
    }
    else if (buttonID.Equals("Immersion"))
    {
      BuildMenuItems(menu, "2");
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

  void MenuCallback(Dictionary<string, string> info)
  {
    DestroyLevelMenu();

    if (info.ContainsKey("scene"))
    {
      string scene = info["scene"];

      AppCentral.APP.LoadLevel(scene);
    }
    else if (info.ContainsKey("cmd"))
    {
      string command = info["cmd"];

      switch (command)
      {
        case "back":  // go back item
          AppCentral.APP.HandleNavigation("Home");
          break;
      }
    }
  }

  void BuildMenuItems(LevelMenu menu, string category)
  {
    menu.SetupItems(AppCentral.APP.MenuItems(category));
  }

}
