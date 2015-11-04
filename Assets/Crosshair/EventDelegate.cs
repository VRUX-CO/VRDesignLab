using UnityEngine;
using System.Collections;

// add this to a gameobject, set the eventDelegate and events will forward
public class EventDelegate : MonoBehaviour
{
  public GameObject eventDelegate;

  public void OnClick()
  {
    if (eventDelegate != null)
    {
      eventDelegate.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
    }
  }

  public void OnHoverStart()
  {
    if (eventDelegate != null)
    {
      eventDelegate.SendMessage("OnHoverStart", SendMessageOptions.DontRequireReceiver);
    }
  }

  public void OnHoverEnd()
  {
    if (eventDelegate != null)
    {
      eventDelegate.SendMessage("OnHoverEnd", SendMessageOptions.DontRequireReceiver);
    }
  }

  public void OnRevealStart()
  {
    if (eventDelegate != null)
    {
      eventDelegate.SendMessage("OnRevealStart", SendMessageOptions.DontRequireReceiver);
    }
  }

  public void OnRevealEnd()
  {
    if (eventDelegate != null)
    {
      eventDelegate.SendMessage("OnRevealEnd", SendMessageOptions.DontRequireReceiver);
    }
  }

}
