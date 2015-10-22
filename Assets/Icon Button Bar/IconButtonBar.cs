using UnityEngine;
using System.Collections;

public class IconButtonBar : MonoBehaviour
{
  public GameObject clickDelegate;
  public string clickCallback;

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
