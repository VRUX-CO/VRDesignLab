using UnityEngine;
using System.Collections;

public class IconButtonBar : MonoBehaviour
{
  // Update is called once per frame
  void Update()
  {
    MoveInfrontOfCamera();
  }

  void MoveInfrontOfCamera()
  {
    Vector3 pos = Camera.main.transform.position;
    pos.z += 1f;
    pos.y += 1f;

    transform.position = pos;
  }

  public void OnButtonClick(string buttonID)
  {
    Debug.Log(buttonID);

    if (buttonID.Equals("Immersion"))
    {
      Application.LoadLevelAdditive("negro");
      Application.UnloadLevel("honkey");

    }
    else if (buttonID.Equals("Foundation"))
    {
      Application.LoadLevelAdditive("honkey");
      Application.UnloadLevel("negro");

    }


    // LevelManager.LM.LoadLevel("VRDL_Lab1_BallonSplitTest");
  }
}
