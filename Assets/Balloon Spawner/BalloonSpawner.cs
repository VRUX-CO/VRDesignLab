using UnityEngine;
using System.Collections;

public class BalloonSpawner : MonoBehaviour
{
  public GameObject balloonPrefab;                // The enemy prefab to be spawned.
    public Lab1_Flow flow;
    private int balloonpop = 0;
    private bool spawned;
    private bool endlab;

    // Use this for initialization
    void Start()
  {
    SpawnBalloon();
  }

  public void SpawnBalloon()
  {
    StartCoroutine(Spawn());
  }

  // Update is called once per frame
  IEnumerator Spawn()
  {
        
        yield return new WaitForSeconds(1.0f);

    Vector3 spawnPosition = new Vector3(
      transform.position.x + Random.Range(1f, 1.5f),
      transform.position.y,
      transform.position.z + Random.Range(3f, 4f)
    );

    GameObject balloon = Instantiate(balloonPrefab, spawnPosition, Quaternion.identity) as GameObject;

    BalloonClick click = balloon.GetComponent<BalloonClick>();
    click.spawner = this;
        //balloonpop += 1;
        //Debug.Log(balloonpop.ToString());
        if (balloonpop == 2)
        {
            flow.PoppedBalloon();
        }
        //spawned = false;
        yield return null;
  }

  public void BalloonPopped()
  {
        //spawned = true;
        balloonpop += 1;
        Debug.Log(balloonpop.ToString());
        SpawnBalloon();
  }

    /*void Update()
    {
        if (spawned)
        {
            balloonpop += 1;
            Debug.Log(balloonpop.ToString());
        }


        if (balloonpop == 2)
        {
            CardAppear();
        }
    }*/
    void CardAppear()
    {
        flow.PoppedBalloon();
    }
}
