using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralRoom : MonoBehaviour
{
  public GameObject wallPrefab;
  List<Material> materials = new List<Material>();

  // Use this for initialization
  void Start()
  {
    MakeSide(0);
    MakeSide(1);
    MakeSide(2);
    MakeSide(3);
    MakeSide(4);
    MakeSide(5);
  }

  public void SetColor(Color color)
  {
    foreach (Material mat in materials)
    {
      mat.color = color;
    }
  }

  // Update is called once per frame
  void MakeSide(int side)
  {
    GameObject wall = Instantiate(wallPrefab) as GameObject;
    wall.transform.parent = transform;

    materials.Add(wall.GetComponent<MeshRenderer>().material);

    float d = wall.transform.localScale.x;

    switch (side)
    {
      case 0:
        wall.transform.localPosition = new Vector3(0, 0, d);
        break;
      case 1:
        wall.transform.localPosition = new Vector3(-d / 2, 0, d / 2);
        wall.transform.localRotation = Quaternion.Euler(0, -90, 0);
        break;
      case 2:
        wall.transform.localPosition = new Vector3(d / 2, 0, d / 2);
        wall.transform.localRotation = Quaternion.Euler(0, 90, 0);

        break;
      case 3:
        wall.transform.localPosition = new Vector3(0, 0, 0);
        wall.transform.localRotation = Quaternion.Euler(0, 180, 0);

        break;
      case 4:
        wall.transform.localPosition = new Vector3(0, -d / 2, d / 2);
        wall.transform.localRotation = Quaternion.Euler(90, 0, 0);
        break;
      case 5:
        wall.transform.localPosition = new Vector3(0, d / 2, d / 2);
        wall.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        break;
    }
  }
}
