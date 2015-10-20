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

  // Use this for initialization
  void Start()
  {
    Vector3 roomPosition = new Vector3(0, 0f, -3);
    Vector3 contentPosition = new Vector3(0, 1.2f, 1);

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
    Destroy(proceduralRoom);
    proceduralRoom = null;
  }

  // -----------------------------------------------------


  void ButtonBarClickCallback(string buttonID)
  {
    Debug.Log(buttonID);

    if (buttonID.Equals("Immersion"))
    {

    }
    else if (buttonID.Equals("Foundation"))
    {
    }
  }

}
