using UnityEngine;
using System.Collections;

public class IconButtonBar : MonoBehaviour
{
  public GameObject clickDelegate;
  public string clickCallback;

  // Update is called once per frame
  void Start()
  {
    Utilities.RotateToFaceCamera(transform, Camera.main);
  }

  public void FadeIn(bool fadeIn)
  {
  }

  public void OnButtonClick(string buttonID)
  {
    if (clickDelegate != null)
    {
      clickDelegate.transform.gameObject.SendMessage(clickCallback, buttonID, SendMessageOptions.DontRequireReceiver);

      // we're done
      Destroy(gameObject);
    }
    else
      Debug.Log("icon button bar has no click delegate");
  }
}
