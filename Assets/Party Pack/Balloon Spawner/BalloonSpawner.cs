using UnityEngine;
using System.Collections;

public class BalloonSpawner : MonoBehaviour
{
  public GameObject balloonPrefab;                // The enemy prefab to be spawned.

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
      transform.position.x + Random.Range(-2f, 2f),
      transform.position.y,
      transform.position.z + Random.Range(-1f, 1f)
    );

    GameObject balloon = Instantiate(balloonPrefab, spawnPosition, Quaternion.identity) as GameObject;

    BalloonClick click = balloon.GetComponent<BalloonClick>();
    click.spawner = this;

    yield return null;
  }

  public void BalloonPopped()
  {
    SpawnBalloon();
  }
}
