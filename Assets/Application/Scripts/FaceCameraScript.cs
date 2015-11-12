using UnityEngine;
using System.Collections;

public class FaceCameraScript : MonoBehaviour
{
  // Update is called once per frame
  void LateUpdate()
  {
    Utilities.RotateToFaceCamera(transform, Camera.main, false, true, false);
  }
}
