using UnityEngine;
using System.Collections;

public class LookButton : MonoBehaviour
{
  public Material mat;

  // Use this for initialization
  void Start()
  {
    MeshUtilities.AddMeshComponent(gameObject, .15f, .15f);

    MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
    renderer.material = mat;

    // start off hidden
    renderer.enabled = false;
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

}
