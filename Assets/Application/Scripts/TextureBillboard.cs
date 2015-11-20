using UnityEngine;
using System.Collections;

public class TextureBillboard : MonoBehaviour
{
  Material fadeMaterial;
  GameObject sign;
  bool destroyOnFadeOut = false;
  bool fading = false;
  int pendingFade = 0;  // used if a fadein or out is called when a fade is in progress

  // Use this for initialization
  void Start()
  {
  }

  public static TextureBillboard Billboard(Material signMat, Vector3 scale, float delay, Vector3 position, Transform parent, bool destroyOnHide)
  {
    GameObject go = new GameObject("TextureBillboard");
    go.transform.parent = parent;
    go.transform.localPosition = position;

    TextureBillboard tbb = go.AddComponent<TextureBillboard>();

    tbb.sign = tbb.MakeSign(signMat, scale);
    tbb.destroyOnFadeOut = destroyOnHide;

    return tbb;
  }

  public void Show(float delay)
  {
    if (fading)
    {
      pendingFade = 1;
    }
    else
    {
      fading = true;

      StartCoroutine(FadeIn(delay));
    }
  }

  public void Hide(float delay)
  {
    if (fading)
    {
      pendingFade = -1;
    }
    else
    {
      fading = true;

      if (sign)
      {
        StartCoroutine(FadeOut(delay));
      }
      else
        Debug.Log("wtf?");
    }
  }

  GameObject MakeSign(Material signMat, Vector3 scale)
  {
    GameObject result = GameObject.CreatePrimitive(PrimitiveType.Quad);
    result.transform.parent = transform;
    result.transform.localPosition = Vector3.zero;

    result.AddComponent<FaceCameraScript>();
    MeshRenderer renderer = result.GetComponent<MeshRenderer>();

    signMat.color = new Color(1, 1, 1, 0f);
    renderer.material = signMat;

    fadeMaterial = renderer.sharedMaterial;

    result.transform.localScale = scale;

    return result;
  }

  IEnumerator FadeIn(float delay)
  {
    yield return new WaitForSeconds(delay);

    iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeInUpdateCallback", "time", 1f, "oncomplete", "FadeInDoneCallback"));
  }

  void FadeInUpdateCallback(float progress)
  {
    fadeMaterial.color = new Color(1, 1, 1, progress);
  }

  void FadeInDoneCallback()
  {
    EndFade();
  }

  IEnumerator FadeOut(float delay)
  {
    yield return new WaitForSeconds(delay);

    iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeOutUpdateCallback", "time", 1f, "oncomplete", "FadeOutDoneCallback"));
  }

  void FadeOutUpdateCallback(float progress)
  {
    fadeMaterial.color = new Color(1, 1, 1, progress);
  }

  void FadeOutDoneCallback()
  {
    EndFade();

    if (destroyOnFadeOut)
      Destroy(gameObject);
  }

  void EndFade()
  {
    fading = false;


    switch (pendingFade)
    {
      case 0:
        break;
      case -1:
        Hide(0);
        break;
      case 1:
        Show(0);
        break;
    }

    pendingFade = 0;
  }

}
