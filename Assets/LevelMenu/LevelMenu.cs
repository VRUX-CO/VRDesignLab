using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMenu : MonoBehaviour
{
  public List<string> LevelNames;
  public GameObject itemPrefab;
  public GameObject clickDelegate;
  public string clickCallback;

  // Use this for initialization
  void Start()
  {
    Vector3 position = gameObject.transform.position;
    int index = 0;

    LevelNames.Reverse();
    foreach (string name in LevelNames)
    {
      // Quaternion.Euler(0.0f, 0f, 0.0f)
      GameObject item = Instantiate(itemPrefab, position, Quaternion.identity) as GameObject;
      item.transform.parent = gameObject.transform;

      LevelMenuItem menuItem = item.GetComponent<LevelMenuItem>() as LevelMenuItem;

      menuItem.SetupItem(name, index++, this);

      position.y += .15f;
    }
  }

  public void ButtonWasClicked(int buttonIndex)
  {
    clickDelegate.SendMessage(clickCallback, buttonIndex, SendMessageOptions.DontRequireReceiver);
  }

}
