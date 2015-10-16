using UnityEngine;
using System.Collections;

public class LevelMenuItem : MonoBehaviour
{
  public Material material;

  string itemName;
  int itemIdentifier;

  // Use this for initialization
  void Start()
  {
    if (material == null)
    {
      Debug.Log("material is null");
      return;
    }

    material = new Material(material);  // copy it so they are all independent materials

    MeshUtilities.AddMeshComponent(gameObject, .2f, .1f);

    MeshRenderer buttonRenderer = gameObject.AddComponent<MeshRenderer>();
    buttonRenderer.material = material;

    //  gameObject.transform.forward = Camera.main.transform.forward;

  }

  public void SetupItem(string name, int identifier)
  {
    itemName = name;
    itemIdentifier = identifier;
  }

  // Update is called once per frame
  void Update()
  {
  }

  public void OnHoverStart()
  {
    material.color = new Color(1f, 0f, 0f, .3f);
  }

  public void OnHoverEnd()
  {
    material.color = new Color(1f, 1f, 0f, .3f);
  }

  public void OnClick()
  {
    Debug.Log("sdf");
  }

}
