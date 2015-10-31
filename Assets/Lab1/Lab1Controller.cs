using UnityEngine;
using System.Collections;

public class Lab1Controller : MonoBehaviour
{
  public GameObject balloonSpawnerPrefab;

  public GameObject Card1;
  public GameObject Card2;

  // Use this for initialization
  void Start()
  {
    GameObject gobj = Instantiate(balloonSpawnerPrefab);

    BalloonSpawner spawnerr = gobj.GetComponent<BalloonSpawner>();
    spawnerr.controller = this;

    /*balloons = GameObject.FindGameObjectsWithTag("Ballons");
    if (balloons.length > 20)
    {
        // Do Something
    }*/
  }

  // Update is called once per frame
  public void PoppedBalloon()
  {
    //   Destroy(Card1);
    //   GameObject newCard = Instantiate(Card2, Card2.transform.position, Card2.transform.rotation) as GameObject;


    // parent this to gameObject so it gets cleaned up when level is unloaded
    //   newCard.transform.parent = gameObject.transform;
  }
}
