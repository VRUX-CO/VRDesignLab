using UnityEngine;
using System.Collections;

public class IconButtonBar : MonoBehaviour
{
  public GameObject clickDelegate;
  public string clickCallback;

  void Start()
  {
    MoveInfrontOfCamera();
  }

  // Update is called once per frame
  void Update()
  {
    Utilities.RotateToFaceCamera(transform, Camera.main);
  }

  void MoveInfrontOfCamera()
  {
    Vector3 pos = Camera.main.transform.position;
    pos.z += 1f;
    pos.y += 1f;

    transform.position = pos;
  }

  public void FadeIn(bool fadeIn)
  {

  }

  public void OnButtonClick(string buttonID)
  {
    if (clickDelegate != null)
    {
      clickDelegate.transform.gameObject.SendMessage(clickCallback, buttonID, SendMessageOptions.DontRequireReceiver);
    }
    else
      Debug.Log("icon button bar has no click delegate");
  }
}
