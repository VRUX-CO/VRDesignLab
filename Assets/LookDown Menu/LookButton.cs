using UnityEngine;
using System.Collections;

public class LookButton : MonoBehaviour
{
  public Material mat;
  float FadeSpeed = 2;
  MeshRenderer buttonRenderer;
  Color startColor;

  void Update()
  {
    //  Color c = startColor;

    //  buttonRenderer.material.color = new Color(c.r, c.g, c.b, .3f); // Mathf.Lerp(c.a, 0.2f, (Time.deltaTime * FadeSpeed)));
  }

  // Use this for initialization
  void Start()
  {
    buttonRenderer = gameObject.AddComponent<MeshRenderer>();
    startColor = buttonRenderer.material.color;

    MeshUtilities.AddMeshComponent(gameObject, .15f, .15f);

    mat.SetColor("_TintColor", new Color(1, 0, 0, .1f)); // Mathf.Lerp(c.a, 0.2f, (Time.deltaTime * FadeSpeed)));

    buttonRenderer.material = mat;
    Debug.Log(string.Format("{0} {1}", mat.color, " fuck"));

    // start off hidden
    buttonRenderer.enabled = false;
  }

  public void OnClick()
  {
    Debug.Log("clicked button");
  }

  public void OnHoverStart()
  {
    Debug.Log("Start");

    transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
  }

  public void OnHoverEnd()
  {
    Debug.Log("End");
    transform.localScale = new Vector3(1f, 1f, 1f);
  }

  public void FadeIn(bool fadeIn)
  {
    if (fadeIn)
    {

    }
    else
    {

    }
  }

}
