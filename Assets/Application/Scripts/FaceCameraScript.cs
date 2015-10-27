using UnityEngine;
using System.Collections;

public class FaceCameraScript : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    Utilities.RotateToFaceCamera(transform, Camera.main);
  }
}
