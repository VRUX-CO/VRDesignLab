using UnityEngine;
using System.Collections;

public class CameraFadeScreen : MonoBehaviour
{
  public LevelManager levelManager;
  Material material;
  Color baseColor;

  const float fadeTime = 1.25f;

  void Start()
  {
    material = GetComponent<MeshRenderer>().material;
    baseColor = material.color;

    Utilities.RotateToFaceCamera(transform, Camera.main);
  }

  public void FadeOut(string callback)
  {
    iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "UpdateCallback", "time", fadeTime, "oncomplete", "FadeOutCallback", "oncompleteparams", callback));
  }

  public void FadeIn()
  {
    iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "UpdateCallback", "time", fadeTime));
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
