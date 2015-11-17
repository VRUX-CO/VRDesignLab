using UnityEngine;
using System.Collections;

public class CameraMount : MonoBehaviour
{
  bool disableTracking = false;
  Quaternion initialRotation;

  public void DiabledTracking(bool disable)
  {
    if (disableTracking != disable)
    {
      disableTracking = disable;

      if (!disableTracking)
      {
        // reset if reenabling tracking
        Reset();
      }
      else
      {
        initialRotation = Camera.main.transform.localRotation;
      }
    }
  }

  void Update()
  {
    if (disableTracking)
      InverseRotateCamera();

  }
  void LateUpdate()
  {
    if (disableTracking)
      InverseRotateCamera();
  }

  void InverseRotateCamera()
  {
    Quaternion quat = Quaternion.Inverse(Camera.main.transform.localRotation);

    quat = quat * initialRotation; // adding is done with multiply
    transform.localRotation = quat;
  }

  void Reset()
  {
    transform.localRotation = Quaternion.identity;
  }




}
