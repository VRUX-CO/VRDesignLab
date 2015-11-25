using UnityEngine;
using System.Collections;

public class CubeSpawner : MonoBehaviour
{
  public GameObject cube;
  bool running = true;
  float xOffset = 3;
  const float speed = 1f;
  bool leftSide = false;
  bool scaleUp = false;

  GameObject lastSpawned = null;

  void Start()
  {
    StartCoroutine(SpawnCube());
  }

  IEnumerator SpawnCube()
  {
    while (running)
    {
      SpawnCubesToFit();
      yield return new WaitForSeconds(.1f);
    }
  }

  float LastSpawnZ()
  {
    float result = 0;

    if (lastSpawned != null && !lastSpawned.Equals(null))
    {
      result = lastSpawned.transform.position.z;
    }

    return result;
  }

  void SpawnCubesToFit()
  {
    float z = LastSpawnZ();

    while (z < 150)
    {
      z += 2;

      float xRandom = Random.Range(3f, 22f);
      float zRandom = Random.Range(-10f, 10f);

      Vector3 position = new Vector3(xOffset + xRandom, 0f, z + zRandom);
      if (leftSide)
      {
        position = new Vector3(-xOffset - xRandom, 0f, z + zRandom);
      }

      GameObject newCube = Instantiate(cube) as GameObject;
      newCube.transform.localPosition = position;

      // make sure it gets deleted when scene is swapped out
      newCube.transform.parent = transform;

      newCube.GetComponent<CubeSpawnerCube>().StartMoving(scaleUp);

      lastSpawned = newCube;

      leftSide = !leftSide;
    }

    scaleUp = true;

  }

  void ffff()
  {

    // newCube.GetComponent<CubeSpawnerCube>().StartMoving(speed);

    float startZ = 70;
    float xRandom = Random.Range(3f, 22f);
    float zRandom = Random.Range(-10f, 10f);

    Vector3 position = new Vector3(xOffset + xRandom, 0f, startZ + zRandom);
    //  if ((count % 2) == 0)
    {
      position = new Vector3(-xOffset - xRandom, 0f, startZ + zRandom);
    }

    GameObject newCube = Instantiate(cube) as GameObject;
    newCube.transform.localPosition = position;

    // make sure it gets deleted when scene is swapped out
    newCube.transform.parent = transform;
  }
}
