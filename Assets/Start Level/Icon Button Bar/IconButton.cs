using UnityEngine;
using System.Collections;

public class IconButton : MonoBehaviour
{
  public string buttonID = "none";
  public Material mat;
  MeshRenderer buttonRenderer;
  Color defaultColor;
  Color fadeStartColor;
  public IconButtonBar buttonBar;
  public GameObject textLabel;

  MeshRenderer labelTextRenderer;

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
    buttonRenderer.material = mat;

    defaultColor = CurrentColor();

    labelTextRenderer = textLabel.GetComponent<MeshRenderer>();

    // start off invisible
    SetColorAlpha(0f);
    labelTextRenderer.material.color = new Color(1f, 1f, 1f, 0f);
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
    buttonBar.OnButtonClick(buttonID);
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
    float start = 1f;
    float end = 0f;

    if (fadeIn)
    {
      start = 0f;
      end = 1f;
    }

    iTween.ValueTo(gameObject, iTween.Hash("from", start, "to", end, "easetype", iTween.EaseType.easeOutExpo, "onupdate", "FadeUpdate", "time", 1.5f, "oncomplete", "FadeOutSignDone"));
  }

  void FadeUpdate(float alpha)
  {
    SetColorAlpha(alpha);

    labelTextRenderer.material.color = new Color(1f, 1f, 1f, alpha);
  }

  public void FadeOutSignDone()
  {
  }
}
