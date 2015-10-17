using UnityEngine;
using System.Collections;

public class LevelMenuItem : MonoBehaviour
{
  public Material material;
  public GameObject backgroundObject;
  public GameObject textObject;
  public LevelMenu levelMenu;
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

    backgroundObject.GetComponent<MeshRenderer>().material = material;
  }

  public void SetupItem(string name, int identifier, LevelMenu menu)
  {
    textObject.GetComponent<TextMesh>().text = name;
    itemIdentifier = identifier;
    levelMenu = menu;
  }

  public void OnHoverStart()
  {
    float time = .5f;
    float scaleBy = 1.05f;

    material.color = new Color(0f, 0f, 0f, .2f);
    iTween.ScaleTo(gameObject, scaleBy * Vector3.one, time);
  }

  public void OnHoverEnd()
  {
    material.color = new Color(0f, 0f, 0f, 0f);
    transform.localScale = new Vector3(1f, 1f, 1f);
  }

  public void OnClick()
  {
    levelMenu.ButtonWasClicked(itemIdentifier);
  }

}
