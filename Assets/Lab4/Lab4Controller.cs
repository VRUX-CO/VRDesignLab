using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lab4Controller : MonoBehaviour
{
  public GameObject CubeSpawnerPrefab;
  public GameObject FlagPrefab;

  public GameObject floor;
  public Material floorMat;
  public Material floorMatLined;

  public Material SignMat;
  public Texture SignTexture;

  bool linedMode = false;
  MeshRenderer meshRenderer;
  List<GameObject> flags = new List<GameObject>();

  // Use this for initialization
  void Start()
  {
    GameObject theFloor = Instantiate(floor);
    // make sure it gets removed when scene is swapped out
    theFloor.transform.parent = transform;

    meshRenderer = theFloor.GetComponent<MeshRenderer>();

    ShowSign();

    // remove any environment
    AppCentral.APP.ShowEnvironment(EnvironmentEnum.kNone);

    GameObject gobj = Instantiate(CubeSpawnerPrefab);
    // make sure it gets removed when scene is swapped out
    gobj.transform.parent = transform;
  }


  void ShowSign()
  {
    // deep clone the material
    Material newSignMat = Instantiate(SignMat) as Material;

    newSignMat.mainTexture = SignTexture;


    Vector3 newPosition = new Vector3(0, .2f, 1.5f);
    TextureBillboard sign = TextureBillboard.Billboard(newSignMat, new Vector3(1, 1, 1), 1, newPosition, transform, false);

    sign.Show(0);
  }


  void Update()
  {
    if (Utilities.UserClicked())
    {
      if (linedMode)
      {
        meshRenderer.material = floorMat;

        foreach (GameObject flag in flags)
        {
          Destroy(flag);
        }
        flags.Clear();
      }
      else
      {
        flags.Add(Instantiate(FlagPrefab, new Vector3(2, 0, 6), Quaternion.Euler(0, 45, 0)) as GameObject);
        flags.Add(Instantiate(FlagPrefab, new Vector3(0, 0, 6), Quaternion.Euler(0, -12, 0)) as GameObject);
        flags.Add(Instantiate(FlagPrefab, new Vector3(-2, 0, 6), Quaternion.Euler(0, -45, 0)) as GameObject);

        flags.Add(Instantiate(FlagPrefab, new Vector3(2, 0, 16), Quaternion.Euler(0, 45, 0)) as GameObject);
        flags.Add(Instantiate(FlagPrefab, new Vector3(0, 0, 16), Quaternion.Euler(0, 12, 0)) as GameObject);
        flags.Add(Instantiate(FlagPrefab, new Vector3(-2, 0, 16), Quaternion.Euler(0, -45, 0)) as GameObject);

        foreach (GameObject flag in flags)
        {
          flag.transform.localScale = new Vector3(4, 4, 4);

          // make sure it gets removed when scene is swapped out
          flag.transform.parent = transform;
        }

        meshRenderer.material = floorMatLined;
      }

      linedMode = !linedMode;
    }
  }


}
