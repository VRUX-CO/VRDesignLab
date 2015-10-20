using UnityEngine;
using System.Collections;

public class CameraFadeScreen : MonoBehaviour
{
  public LevelManager levelManager;
  Material material;
  Color baseColor;

  void Update()
  {
    //  transform.LookAt(Camera.main.transform);
  }

  void Start()
  {
    material = GetComponent<MeshRenderer>().material;
    baseColor = material.color;

    Utilities.RotateToFaceCamera(transform, Camera.main);
  }

  public void FadeOut(string callback)
  {
    iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "UpdateCallback", "time", 3.5f, "oncomplete", "FadeOutCallback", "oncompleteparams", callback));
  }

  public void FadeIn()
  {
    iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "UpdateCallback", "time", 3.5f));
  }

  public void UpdateCallback(float alpha)
  {
    material.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
  }

  public void FadeOutCallback(string callback)
  {
    levelManager.SendMessage(callback);
  }
}
