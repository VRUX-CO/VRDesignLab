using UnityEngine;
using System.Collections;

public class LookButton : MonoBehaviour
{
  public string buttonID = "none";
  public Material mat;
  MeshRenderer buttonRenderer;
  Color defaultColor;
  Color fadeStartColor;
  public ButtonRevealPlane revealPlane;

  public float fadeDuration = .5f;
  private float _timeStartedLerping;
  bool isUpdatingFade = false;
  bool isFadingIn = false;

  void Update()
  {
    if (isUpdatingFade)
    {
      float alpha = 1;
      float timeSinceStarted = Time.time - _timeStartedLerping;
      float percentageComplete = timeSinceStarted / fadeDuration;

      if (isFadingIn)
        alpha = Mathf.Lerp(fadeStartColor.a, defaultColor.a, percentageComplete);
      else
        alpha = Mathf.Lerp(fadeStartColor.a, 0f, percentageComplete);

      SetColorAlpha(alpha);

      if (percentageComplete >= 1.0f)
        isUpdatingFade = false;
    }
  }

  // Use this for initialization
  void Start()
  {
    buttonRenderer = gameObject.AddComponent<MeshRenderer>();
    buttonRenderer.material = mat;

    defaultColor = CurrentColor();

    MeshUtilities.AddMeshComponent(gameObject, .15f, .15f);

    // start off hidden
    SetColorAlpha(0);
  }

  void SetColorAlpha(float alpha)
  {
    buttonRenderer.material.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, alpha);
  }

  Color CurrentColor()
  {
    return buttonRenderer.material.GetColor("_Color");
  }

  public void OnClick()
  {
    revealPlane.OnLookButtonClick(buttonID);
  }

  public void OnHoverStart()
  {
    transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
  }

  public void OnHoverEnd()
  {
    transform.localScale = new Vector3(1f, 1f, 1f);
  }

  public void FadeIn(bool fadeIn)
  {
    fadeStartColor = CurrentColor();

    _timeStartedLerping = Time.time;

    isUpdatingFade = true;
    isFadingIn = fadeIn;
  }

}
