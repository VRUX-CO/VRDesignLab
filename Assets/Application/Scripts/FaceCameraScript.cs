using UnityEngine;
using System.Collections;

public class FaceCameraScript : MonoBehaviour
{
  // Update is called once per frame
  void Update()
  {
    Utilities.RotateToFaceCamera(transform, Camera.main);
  }
}
