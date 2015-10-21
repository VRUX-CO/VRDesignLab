using UnityEngine;
using System.Collections;

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

    if (buttonID.Equals("Immersion"))
    {
      levelMenu.SetActive(true);
    }
    else if (buttonID.Equals("Foundation"))
    {
      levelMenu.SetActive(true);
    }
  }

  void MenuCallback(int buttonIndex)
  {
    switch (buttonIndex)
    {
      case 0:
        AppCentral.APP.LoadLevel("VRDL_Lab1");
        break;
      case 1:
        AppCentral.APP.LoadLevel("VRDL_Lab2");
        break;
      case 2:
        AppCentral.APP.LoadLevel("VRDL_Lab3");
        break;
      case 3:
        AppCentral.APP.LoadLevel("VRDL_Lab4");
        break;
      case 4:
        AppCentral.APP.LoadLevel("VRDL_Lab5");
        break;
      case 5:  // go back item
        Debug.Log("go back");
        break;
    }
  }

}
