﻿using UnityEngine;
using System.Collections;

public class DistancesViewer : MonoBehaviour
{
  public GameObject HalfMeterMessage;
  public GameObject OneMeterMessage;
  public GameObject OneHalfMeterMessage;
  public GameObject ThreeMeterMessage;
  public GameObject FiveMeterMessage;

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
    if (Input.GetButtonDown(OVRGamepadController.ButtonNames[(int)OVRGamepadController.Button.A]) ||  // "Desktop_Button A"
  Input.GetButtonDown("Button A") ||
  Cardboard.SDK.Triggered)
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

      iTween.MoveTo(currentSign, iTween.Hash("position", position, "time", .5f, "easeType", iTween.EaseType.easeInExpo, "oncomplete", "OnCompleteCallback", "oncompleteparams", currentSign));

      currentSign = null;
    }

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
      default:
        signToShow = FiveMeterMessage;
        distance = 5f;

        break;
    }

    Vector3 newPosition = new Vector3(0, 1, distance);
    currentSign = Instantiate(signToShow, newPosition, Quaternion.identity) as GameObject;

    index++;
  }

  void OnCompleteCallback(GameObject sign)
  {
    Destroy(sign);
  }
}