using UnityEngine;
using System.Collections;

public class CubeSpawner : MonoBehaviour
{
  public GameObject cube;
  public GameObject floor;
  public Material floorMat;
  public Material floorMatLined;
  bool linedMode = false;
  MeshRenderer renderer;

  bool running = true;
  float xOffset = 3;
  const float speed = 1f;

  void Start()
  {
    renderer = floor.GetComponent<MeshRenderer>();

    StartCoroutine(SpawnCube(0));
  }

  void Update()
  {
    if (Utilities.UserClicked())
    {
      if (linedMode)
      {
        renderer.material = floorMat;
      }
      else
      {
        renderer.material = floorMatLined;
      }

      linedMode = !linedMode;
    }
  }

  IEnumerator SpawnCube(int count)
  {
    float startZ = 70;
    float xRandom = Random.RandomRange(3f, 22f);
    float zRandom = Random.RandomRange(-10f, 10f);

    Vector3 position = new Vector3(xOffset + xRandom, 0f, startZ + zRandom);
    if ((count % 2) == 0)
    {
      position = new Vector3(-xOffset - xRandom, 0f, startZ + zRandom);
    }

    GameObject gameObject = Instantiate(cube, position, Quaternion.identity) as GameObject;
    gameObject.GetComponent<CubeSpawnerCube>().StartMoving(speed);

    if (running)
    {
      yield return new WaitForSeconds(.2f / speed);

      StartCoroutine(SpawnCube(count + 1));
    }
  }
}
