using UnityEngine;
using System.Collections;

public class IconButtonBar : MonoBehaviour
{
  public GameObject clickDelegate;
  public string clickCallback;

  public void OnButtonClick(string buttonID)
  {
    if (clickDelegate != null)
    {
      clickDelegate.transform.gameObject.SendMessage(clickCallback, buttonID, SendMessageOptions.DontRequireReceiver);
    }
    else
      Debug.Log("icon button bar has no click delegate");
  }

  public void FadeIn(bool fadeIn)
  {
    IconButton[] buttons = GetComponentsInChildren<IconButton>();

    foreach (IconButton button in buttons)
    {
      button.FadeIn(fadeIn);
    }

    if (!fadeIn)
      StartCoroutine(DestroyAfterDelay());
  }

  IEnumerator DestroyAfterDelay()
  {
    yield return new WaitForSeconds(2.0f);

    DestroyObject(gameObject);
  }
}
