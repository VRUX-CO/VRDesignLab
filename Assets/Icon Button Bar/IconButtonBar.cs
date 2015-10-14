using UnityEngine;
using System.Collections;

public class IconButtonBar : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    MoveInfrontOfCamera();

  }

  void MoveInfrontOfCamera()
  {
    Vector3 pos = Camera.main.transform.position;
    pos.z += 1f;
    pos.y += 1f;

    transform.position = pos;
  }

}
