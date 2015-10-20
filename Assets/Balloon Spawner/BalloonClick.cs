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
    // show particles
    // gameObject.GetComponentInChildren<ParticleSystem>().Play();

    //Split the Balloon 
    balloonsplit.SplitBalloon();
    // play pop sound
    if (gameObject.GetComponent<AudioSource>() != null) gameObject.GetComponent<AudioSource>().Play();

    // tell the spawner we've been popped
    spawner.BalloonPopped();

        // destroy everything after delay to give time for particle system to finish
        /*int destroyDelay = 1;
        foreach (Transform child in this.transform)
        {
          Destroy(child.gameObject, destroyDelay);
        }
        Destroy(this, destroyDelay);*/
        
        Destroy(gameObject);
    
    }

 }
