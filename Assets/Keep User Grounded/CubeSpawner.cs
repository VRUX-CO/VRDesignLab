using UnityEngine;
using System.Collections;

public class CubeSpawner : MonoBehaviour
{
  public GameObject cube;
  bool running = true;
  float xOffset = 3;

  void Start()
  {
    StartCoroutine(SpawnCube(0));
  }

  IEnumerator SpawnCube(int count)
  {
    for (int x = 0; x < 1; x++)
    {
      float startZ = 70;
      float xRandom = Random.RandomRange(3f, 22f);
      float zRandom = Random.RandomRange(-10f, 10f);

      Vector3 position = new Vector3(xOffset + xRandom, 0f, startZ + zRandom);
      if ((count % 2) == 0)
      {
        position = new Vector3(-xOffset - xRandom, 0f, startZ + zRandom);
      }

      Instantiate(cube, position, Quaternion.identity);
    }

    if (running)
    {
      yield return new WaitForSeconds(.2f);

      StartCoroutine(SpawnCube(count + 1));
    }

  }
}
