using UnityEngine;
using System.Collections;

public class LookDownNotifier : MonoBehaviour
{
  public GameObject downArrow;

  // Use this for initialization
  void Start()
  {
    if (downArrow != null)
      iTween.MoveBy(downArrow, iTween.Hash("y", .05f, "time", .4f, "easeType", iTween.EaseType.easeInOutSine, "loopType", iTween.LoopType.pingPong));



    iTween.MoveBy(gameObject, iTween.Hash("y", 2f, "time", .5f, "easeType", iTween.EaseType.easeOutExpo, "onupdate", "AnimationUpdateCallback", "oncomplete", "AnimationDoneCallback"));


  }

  public void AnimationUpdateCallback()
  {
    Debug.Log("update");
  }

  public void AnimationDoneCallback()
  {
    Debug.Log("done");
  }

  // Update is called once per frame
  void Update()
  {

  }
}
