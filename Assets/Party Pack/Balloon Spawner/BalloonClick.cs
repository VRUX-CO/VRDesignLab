using UnityEngine;
using System.Collections;

public class BalloonClick : MonoBehaviour, CrosshairTargetable
{
  public BalloonSpawner spawner;

  public void OnClick()
  {
    // show particles
    // gameObject.GetComponentInChildren<ParticleSystem>().Play();

    // play pop sound
    if (gameObject.GetComponent<AudioSource>() != null) gameObject.GetComponent<AudioSource>().Play();

    // tell the spawner we've been popped
    spawner.BalloonPopped();

    // destroy everything after delay to give time for particle system to finish
    int destroyDelay = 1;
    foreach (Transform child in this.transform)
    {
      Destroy(child.gameObject, destroyDelay);
    }
    Destroy(this, destroyDelay);
  }

  bool CrosshairTargetable.IsTargetable()
  {
    return true;
  }

}
