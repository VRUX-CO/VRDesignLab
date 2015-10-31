using UnityEngine;
using System.Collections;

public class BalloonClick : MonoBehaviour
{
  public BalloonSpawner spawner;
  public SplitMeshIntoTriangles balloonsplit;

  void Start()
  {
    iTween.MoveBy(gameObject, iTween.Hash("y", .1f, "time", 1f, "easeType", iTween.EaseType.easeInOutSine, "loopType", iTween.LoopType.pingPong));
  }

  public void OnClick()
  {
    //Split the Balloon 
    balloonsplit.SplitBalloon();

    // play pop sound
    if (gameObject.GetComponent<AudioSource>() != null)
      gameObject.GetComponent<AudioSource>().Play();

    // tell the spawner we've been popped
    spawner.BalloonPopped();

    // give time for the sound to play
    Destroy(gameObject, 1);

  }

}
