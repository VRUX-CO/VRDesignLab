using UnityEngine;
using System.Collections;

public class LookButton : MonoBehaviour
{
  public string buttonID = "none";
  public Material mat;
  MeshRenderer buttonRenderer;
  Color defaultColor;
  Color fadeStartColor;
  public GameObject textLabel;
  GameObject clickDelegate;
  string clickCallback;

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
      float alpha = 1;
      float percentageComplete = textUpdater.PercentageComplete();

      if (isFadingTextIn)
        alpha = Mathf.Lerp(0, 1, percentageComplete);
      else
        alpha = Mathf.Lerp(1, 0f, percentageComplete);

      labelTextMesh.color = new Color(0, 0, 0, alpha);
    }
  }

  // Use this for initialization
  void Start()
  {
    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
    quad.transform.parent = transform;  // parent this quad
    quad.transform.localPosition = Vector3.zero;
    quad.tag = Crosshair3D.kCrosshairTargetable;
    EventDelegate ed = quad.AddComponent<EventDelegate>();
    ed.eventDelegate = gameObject;

    quad.transform.localScale = new Vector3(.15f, .15f, .15f);
    buttonRenderer = quad.GetComponent<MeshRenderer>();

    defaultColor = CurrentColor();
    buttonRenderer.material = mat;

    // start off hidden
    SetColorAlpha(0);

    labelTextMesh = textLabel.GetComponent<TextMesh>();
    labelTextMesh.color = Color.clear;
  }

  public void SetClickDelegate(GameObject del, string callb)
  {
    clickDelegate = del;
    clickCallback = callb;
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
    clickDelegate.SendMessage(clickCallback, buttonID, SendMessageOptions.DontRequireReceiver);
  }

  public void OnHoverStart()
  {
    transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

    // play click sound
    if (gameObject.GetComponent<AudioSource>() != null)
      gameObject.GetComponent<AudioSource>().Play();

    FadeInText(true);
  }

  public void OnHoverEnd()
  {
    transform.localScale = new Vector3(1f, 1f, 1f);

    FadeInText(false);
  }

  public void FadeIn(bool fadeIn)
  {
    fadeStartColor = CurrentColor();

    fadeUpdater.StartUpdater(.5f);

    isFadingIn = fadeIn;
  }

  public void FadeInText(bool fadeIn)
  {
    textUpdater.StartUpdater(.5f);

    isFadingTextIn = fadeIn;
  }
}
