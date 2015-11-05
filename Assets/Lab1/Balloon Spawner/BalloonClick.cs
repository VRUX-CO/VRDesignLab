using UnityEngine;
using System.Collections;

public class BalloonClick : MonoBehaviour
{
  public BalloonSpawner spawner;
  public SplitMeshIntoTriangles balloonsplit;
  public AudioClip popSound;

  void Start()
  {
    iTween.MoveBy(gameObject, iTween.Hash("y", .1f, "time", 1f, "easeType", iTween.EaseType.easeInOutSine, "loopType", iTween.LoopType.pingPong));
  }

  public void OnClick()
  {
    //Split the Balloon 
    balloonsplit.SplitBalloon();

    // play pop sound
    if (popSound != null)
      Utilities.PlaySound(popSound);

    // tell the spawner we've been popped
    spawner.BalloonPopped();

    // give time for the sound to play
    Destroy(gameObject);
  }
}
