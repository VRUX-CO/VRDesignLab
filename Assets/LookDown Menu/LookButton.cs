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
    Color c = startColor;

    buttonRenderer.material.color = new Color(c.r, c.g, c.b, .3f); // Mathf.Lerp(c.a, 0.2f, (Time.deltaTime * FadeSpeed)));
  }

  // Use this for initialization
  void Start()
  {
    MeshUtilities.AddMeshComponent(gameObject, .15f, .15f);

    buttonRenderer = gameObject.AddComponent<MeshRenderer>();
    buttonRenderer.material = mat;

    startColor = buttonRenderer.material.color;

    // start off hidden
    buttonRenderer.enabled = true;
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
      Color alphaFadedColor = Color.white;

      // modify alpha
      // create your own time based algorithm and start it when you want the fade to start
      alphaFadedColor.a = Time.realtimeSinceStartup / 10f;
    }
    else
    {

    }
  }

}
