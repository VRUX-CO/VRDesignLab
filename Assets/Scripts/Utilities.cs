using UnityEngine;
using System.Collections;

public class Utilities : MonoBehaviour
{
  public static bool UserClicked()
  {
    if (Input.GetButtonDown(OVRGamepadController.ButtonNames[(int)OVRGamepadController.Button.A]) ||   // "Desktop_Button A"
Input.GetButtonDown("Button A") ||
Cardboard.SDK.Triggered)
    {
      return true;
    }

    return false;
  }
  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
