using UnityEngine;
using System.Collections;

public class CameraFadeScreen : MonoBehaviour
{
  Material material;
  Color baseColor;

  void Start()
  {
    material = GetComponent<MeshRenderer>().material;
    baseColor = material.color;
  }

  public void FadeOut()
  {
    iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "UpdateCallback", "time", 3.5f, "oncomplete", "DoneCallback"));
  }

  public void FadeIn()
  {
    iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "UpdateCallback", "time", 3.5f, "oncomplete", "DoneCallback"));
  }

  public void UpdateCallback(float alpha)
  {
    material.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
  }

  public void DoneCallback()
  {
  }
}
