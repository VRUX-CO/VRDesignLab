using UnityEngine;
using System.Collections;

public class IconButton : MonoBehaviour, CrosshairTargetable
{
  public string buttonID = "none";
  public Material mat;
  MeshRenderer buttonRenderer;
  Color defaultColor;
  Color fadeStartColor;
  public ButtonRevealPlane revealPlane;
  public GameObject textLabel;

  bool isFadingIn = false;

  TextMesh labelTextMesh;
  AnimationUpdater fadeUpdater = new AnimationUpdater();

  void Update()
  {
    if (fadeUpdater.IsRunning())
    {
      float alpha = 1;
      float percentageComplete = fadeUpdater.PercentageComplete();

      if (isFadingIn)
        alpha = Mathf.Lerp(fadeStartColor.a, defaultColor.a, percentageComplete);
      else
        alpha = Mathf.Lerp(fadeStartColor.a, 0f, percentageComplete);

      SetColorAlpha(alpha);
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
    // SetColorAlpha(0);

    labelTextMesh = textLabel.GetComponent<TextMesh>();
    labelTextMesh.color = new Color(0f, 0f, 0f, .9f);
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

    // play click sound
    if (gameObject.GetComponent<AudioSource>() != null) gameObject.GetComponent<AudioSource>().Play();
  }

  public void OnHoverEnd()
  {
    transform.localScale = new Vector3(1f, 1f, 1f);
  }

  public void FadeIn(bool fadeIn)
  {
    fadeStartColor = CurrentColor();

    fadeUpdater.StartUpdater(.5f);

    isFadingIn = fadeIn;
  }



  bool CrosshairTargetable.IsTargetable()
  {
    return true;
  }
}
