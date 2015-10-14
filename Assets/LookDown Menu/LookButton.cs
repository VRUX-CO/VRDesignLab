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
  public GameObject textLabel;

  bool isFadingIn = false;

  TextMesh labelTextMesh;
  bool isFadingTextIn = false;
  AnimationUpdater fadeUpdater = new AnimationUpdater();
  AnimationUpdater textUpdater = new AnimationUpdater();


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

    if (textUpdater.IsRunning())
    {
      if (isFadingTextIn)
      {

      }
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

    labelTextMesh = textLabel.GetComponent<TextMesh>();
    labelTextMesh.color = Color.clear;

    Vector3 ScreenPos = new Vector3(0f, .5f, 1f);  // Camera.main.WorldToScreenPoint(transform.position);
    GUI.Label(new Rect((float)ScreenPos.x, (float)(Screen.height - ScreenPos.y), 100, 20), "Fuck this shit");
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

    fadeUpdater.StartUpdater(.5f);

    isFadingIn = fadeIn;
  }

}
