using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMenuItem : MonoBehaviour
{
  public Material material;
  public GameObject backgroundObject;
  public GameObject textObject;
  public LevelMenu levelMenu;

  Dictionary<string, string> itemInfo;
  float scaleTime = .5f;
  float scaleBy = 1.05f;

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

  public void SetupItem(Dictionary<string, string> info, LevelMenu menu)
  {
    textObject.GetComponent<TextMesh>().text = info["name"];
    itemInfo = info;
    levelMenu = menu;
  }

  public void OnHoverStart()
  {
    material.color = new Color(0f, 0f, 0f, .2f);
    iTween.ScaleTo(gameObject, scaleBy * Vector3.one, scaleTime);
  }

  public void OnHoverEnd()
  {
    material.color = new Color(0f, 0f, 0f, 0f);
    iTween.ScaleTo(gameObject, Vector3.one, scaleTime);
  }

  public void OnClick()
  {
    levelMenu.ItemWasClicked(itemInfo);
  }

}
