using UnityEngine;
using System.Collections;

public class Lab1_Flow : MonoBehaviour
{
  public GameObject Card1;
  public GameObject Card2;

  private int balloonpop;

  // Use this for initialization
  void Start()
  {
    /*balloons = GameObject.FindGameObjectsWithTag("Ballons");
    if (balloons.length > 20)
    {
        // Do Something
    }*/
  }

  // Update is called once per frame
  public void PoppedBalloon()
  {
    Destroy(Card1);
    GameObject newCard = Instantiate(Card2, Card2.transform.position, Card2.transform.rotation) as GameObject;
    

    // parent this to gameObject so it gets cleaned up when level is unloaded
    newCard.transform.parent = gameObject.transform;
  }
}
