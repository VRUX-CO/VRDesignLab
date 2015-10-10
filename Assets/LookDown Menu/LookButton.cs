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

  }

  // Update is called once per frame
  void Update()
  {

  }
}
