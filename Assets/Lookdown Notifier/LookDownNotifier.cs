using UnityEngine;
using System.Collections;

public class LookDownNotifier : MonoBehaviour
{
  public GameObject downArrow;
  public GameObject sign;

  Vector3 startPosition;
  Material signMaterial;
  Material downArrayMaterial;

  // Use this for initialization
  void Start()
  {
    if (downArrow == null || sign == null)
    {
      Debug.Log("LookDownNotifier: bad parameters");
      return;
    }

    startPosition = transform.position;
    signMaterial = sign.GetComponent<MeshRenderer>().material;
    downArrayMaterial = downArrow.GetComponent<MeshRenderer>().material;

    signMaterial.color = new Color(1, 1, 1, 0);
    downArrayMaterial.color = new Color(0, 0, 0, 0);

    StartCoroutine(StartAnimation(1));
  }

  IEnumerator StartAnimation(float delay)
  {
    yield return new WaitForSeconds(delay);

    // bounces down arrow forever
    iTween.MoveBy(downArrow, iTween.Hash("y", .03f, "time", .4f, "easeType", iTween.EaseType.easeInOutSine, "loopType", iTween.LoopType.pingPong));

    // move into position and fade in
    iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "AnimationUpdateCallback", "time", 1f, "oncomplete", "AnimationDoneCallback"));
  }

  public void AnimationUpdateCallback(float progress)
  {
    Vector3 vect = gameObject.transform.position;

    vect.y = startPosition.y + (progress * 1);

    gameObject.transform.position = vect;


    signMaterial.color = new Color(1, 1, 1, progress);
    downArrayMaterial.color = new Color(0, 0, 0, progress);
  }

  public void AnimationDoneCallback()
  {
    StartCoroutine(FadeOutAndDestroy(3));
  }

  IEnumerator FadeOutAndDestroy(float delay)
  {
    yield return new WaitForSeconds(delay);

    iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeOutUpdateCallback", "time", .5f, "oncomplete", "FadeOutDoneCallback"));
  }

  public void FadeOutUpdateCallback(float progress)
  {
    signMaterial.color = new Color(1, 1, 1, progress);
    downArrayMaterial.color = new Color(0, 0, 0, progress);
  }

  public void FadeOutDoneCallback()
  {
    gameObject.SetActive(false);
  }
}
