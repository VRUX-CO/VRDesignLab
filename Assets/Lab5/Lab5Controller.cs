using UnityEngine;
using System.Collections;

public class Lab5Controller : MonoBehaviour
{
  public Material trackingOffMat;

  Material fadeMaterial;

  bool trackingDisabled = false;

  // Use this for initialization
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {
    if (Utilities.UserClicked())
    {
      StartCoroutine(DisableTrackingForSeconds(2));
    }
  }

  IEnumerator DisableTrackingForSeconds(float delay)
  {
    if (!trackingDisabled)
    {
      GameObject trackingOffCard = ShowCard(0);
      StartCoroutine(FadeIn(trackingOffCard, 0));

      ToggleTracking();

      yield return new WaitForSeconds(delay);

      if (trackingDisabled)
      {
        ToggleTracking();

        StartCoroutine(FadeOut(trackingOffCard, 1));
      }

    }

    //  yield return null;
  }

  void ToggleTracking()
  {
    trackingDisabled = !trackingDisabled;

    AppCentral.APP.DisableHeadTracking(trackingDisabled);
  }

  GameObject ShowCard(int cardIndex)
  {
    Vector3 signPosition = new Vector3(0, 1, 2);

    GameObject result = GameObject.CreatePrimitive(PrimitiveType.Quad);
    result.AddComponent<FaceCameraScript>();
    MeshRenderer renderer = result.GetComponent<MeshRenderer>();

    switch (cardIndex)
    {
      default:
      case 0:
        trackingOffMat.color = new Color(1, 1, 1, 0f);
        renderer.material = trackingOffMat;

        fadeMaterial = renderer.sharedMaterial;

        result.transform.parent = Camera.main.transform;
        result.transform.localPosition = signPosition;
        break;
    }

    return result;
  }

  IEnumerator FadeIn(GameObject sign, float delay)
  {
    yield return new WaitForSeconds(delay);

    iTween.ValueTo(sign, iTween.Hash("from", 0f, "to", 1f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeInUpdateCallback", "time", 1f, "oncomplete", "FadeInDoneCallback", "onupdatetarget", gameObject, "oncompletetarget", gameObject));
  }

  void FadeInUpdateCallback(float progress)
  {
    fadeMaterial.color = new Color(1, 1, 1, progress);
  }

  void FadeInDoneCallback()
  {
  }

  IEnumerator FadeOut(GameObject sign, float delay)
  {
    yield return new WaitForSeconds(delay);


    iTween.ValueTo(sign, iTween.Hash("from", 1f, "to", 0f, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeOutUpdateCallback", "time", 1f, "oncomplete", "FadeOutDoneCallback", "oncompleteparams", sign, "onupdatetarget", gameObject, "oncompletetarget", gameObject));
  }

  void FadeOutUpdateCallback(float progress)
  {
    fadeMaterial.color = new Color(1, 1, 1, progress);
  }

  void FadeOutDoneCallback(GameObject sign)
  {
    Destroy(sign);
  }


}
