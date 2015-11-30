using UnityEngine;
using System.Collections;

public class ClickTarget : MonoBehaviour
{
  public GameObject targetPrefab;

  // private
  GameObject clickDelegate;
  string clickCallback;
  GameObject targetModel;
  string targetID;

  void Update()
  {
  }

  // Use this for initialization
  void Start()
  {
    targetModel = Instantiate(targetPrefab);

    targetModel.transform.parent = transform;  // parent this quad
    targetModel.transform.localPosition = Vector3.zero;

    targetModel.tag = Crosshair3D.kCrosshairTargetable;

    EventDelegate ed = targetModel.AddComponent<EventDelegate>();
    ed.eventDelegate = gameObject;

    targetModel.transform.localScale = new Vector3(22, 22, 22);

  }

  public void SetClickDelegate(GameObject del, string callb, string inTargetID)
  {
    clickDelegate = del;
    clickCallback = callb;

    targetID = inTargetID;
  }

  public void OnClick()
  {
    if (clickDelegate != null)
    {
      clickDelegate.SendMessage(clickCallback, targetID, SendMessageOptions.DontRequireReceiver);
    }
  }

  public void OnHoverStart()
  {
  }

  public void OnHoverEnd()
  {
  }

}
