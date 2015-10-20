using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
  public GameObject cameraFadeScreenPrefab;

  // Use this for initialization
  void Start()
  {
    // AttachFadeScreenToCamera();
  }

  void AttachFadeScreenToCamera()
  {
    GameObject fadeScreen = Instantiate(cameraFadeScreenPrefab) as GameObject;

    fadeScreen.transform.parent = Camera.main.transform;

    fadeScreen.transform.localPosition = new Vector3(0, 0, .4f);
  }
}
