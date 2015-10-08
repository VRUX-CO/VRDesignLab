using UnityEngine;
using System.Collections;

public class BalloonClick : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void OnClick()
  {
    // Luis, fix this, it's ugly and I'm sure you have a better way

    // show particles
    gameObject.GetComponentInChildren<ParticleSystem>().Play();

    // play pop sound
    if (gameObject.GetComponent<AudioSource>() != null) gameObject.GetComponent<AudioSource>().Play();

    // destroy everything after delay to give time for particle system to finish
    int destroyDelay = 1;
    foreach (Transform child in this.transform)
    {
      Destroy(child.gameObject, destroyDelay);
    }
    Destroy(this, destroyDelay);
  }
}
