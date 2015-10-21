using UnityEngine;
using System.Collections;

public class BalloonSpawner : MonoBehaviour
{
  public GameObject balloonPrefab;
  public Lab1_Flow flow;
  private int balloonpop = 0;
  private bool swapped;
  private bool endlab;

  // Use this for initialization
  void Start()
  {
    // SNG, need to reset this if the person bails out to main menu
    AppCentral.APP.ShowReticleOnClick(true);

    SpawnBalloon();
  }

  public void SpawnBalloon()
  {
    StartCoroutine(Spawn());
  }

  // Update is called once per frame
  IEnumerator Spawn()
  {
    Vector3 spawnPosition;

    yield return new WaitForSeconds(1.0f);

    if (!swapped)
    {
      spawnPosition = new Vector3(
      transform.position.x + Random.Range(1f, 1.5f),
      transform.position.y,
      transform.position.z + Random.Range(3f, 4f));
    }
    else
    {
      spawnPosition = new Vector3(
          transform.position.x + Random.Range(-2f, -1f),
          transform.position.y,
          transform.position.z + Random.Range(3f, 4f));
    }

    GameObject balloon = Instantiate(balloonPrefab, spawnPosition, Quaternion.identity) as GameObject;

    // make this a child of the spawner so balloons will get destroyed when level is swapped out
    balloon.transform.parent = gameObject.transform;

    BalloonClick click = balloon.GetComponent<BalloonClick>();
    click.spawner = this;

    yield return null;
  }

  public void BalloonPopped()
  {
    balloonpop += 1;
    if (balloonpop == 2)
    {
      flow.PoppedBalloon();

      AppCentral.APP.ShowReticleOnClick(false);

      swapped = true;
    }
    SpawnBalloon();

    if (balloonpop == 5)
    {
      AppCentral.APP.ShowLookdownNotifier();
    }
  }

}
