using UnityEngine;
using System.Collections;

public class DistancesViewer : MonoBehaviour
{
  public GameObject DistanceRingObject;
  public GameObject DistanceLineObject;
  public float lineLength;

  public GameObject HalfMeterMessage;
  public GameObject OneMeterMessage;
  public GameObject OneHalfMeterMessage;
  public GameObject ThreeMeterMessage;
  public GameObject FiveMeterMessage;

  GameObject currentRingObject;
  int index = 0;
  GameObject currentSign;

  // Use this for initialization
  void Start()
  {
    Next();
  }

  // Update is called once per frame
  void Update()
  {
    if (Utilities.UserClicked())
    {
      Next();
    }
  }

  void Next()
  {
    GameObject signToShow;
    float distance;

    if (currentSign != null)
    {
      Vector3 position = currentSign.transform.position;
      position.y += 12f;

      iTween.MoveTo(currentSign, iTween.Hash("position", position, "time", .5f, "easeType", iTween.EaseType.easeInExpo, "oncomplete", "OnCompleteOutCallback", "oncompleteparams", currentSign));

      currentSign = null;
    }

    if (index < 6)
    {
      switch (index)
      {
        case 0:
          signToShow = HalfMeterMessage;
          distance = .5f;
          break;
        case 1:
          signToShow = OneMeterMessage;
          distance = 1f;
          break;
        case 2:
          signToShow = OneHalfMeterMessage;
          distance = 1.5f;
          break;
        case 3:
          signToShow = ThreeMeterMessage;
          distance = 3f;
          break;
        case 4:
          signToShow = FiveMeterMessage;
          distance = 5f;

          break;
        case 5:
        default:
          signToShow = FiveMeterMessage;
          distance = 12f;

          break;
      }

      // update distance loop
      if (currentRingObject != null)
        Destroy(currentRingObject);
      currentRingObject = Instantiate(DistanceRingObject) as GameObject;
      DistanceRing distanceRing;
      distanceRing = currentRingObject.GetComponent<DistanceRing>();
      distanceRing.UpdateDistance(distance);

      DistanceLineObject.transform.position = new Vector3(0, lineLength / 2f, distance);
      DistanceLineObject.transform.localScale = new Vector3(.01f, lineLength, 1);

      Vector3 newPosition = new Vector3(0, -22f, distance);
      currentSign = Instantiate(signToShow, newPosition, Quaternion.identity) as GameObject;

      // parent this so it gets deleted when scene is swapped out
      currentSign.transform.parent = transform;

      newPosition = new Vector3(0, 1f, distance);
      iTween.MoveTo(currentSign, iTween.Hash("position", newPosition, "time", .5f, "easeType", iTween.EaseType.easeInExpo, "oncomplete", "OnCompleteInCallback", "oncompleteparams", currentSign));
    }

    index++;
  }

  void OnCompleteInCallback(GameObject sign)
  {
  }

  void OnCompleteOutCallback(GameObject sign)
  {
    Destroy(sign);
  }
}
