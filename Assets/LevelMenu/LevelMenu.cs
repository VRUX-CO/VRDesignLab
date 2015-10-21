using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMenu : MonoBehaviour
{
  public GameObject itemPrefab;
  public GameObject clickDelegate;
  public string clickCallback;

  // Use this for initialization
  public void SetupItems(List<Dictionary<string, string>> menuItems)
  {
    Vector3 position = gameObject.transform.position;
    int index = 0;
    const float itemHeight = .15f;
    float startY = position.y;
    int cnt = menuItems.Count;

    foreach (Dictionary<string, string> item in menuItems)
    {
      position.y = startY + ((cnt - index) * itemHeight);
      GameObject itemGO = Instantiate(itemPrefab, position, Quaternion.identity) as GameObject;

      itemGO.transform.parent = gameObject.transform;

      LevelMenuItem menuItem = itemGO.GetComponent<LevelMenuItem>() as LevelMenuItem;

      menuItem.SetupItem(item["name"], item["cmd"], this);

      index++;
    }
  }

  public void ItemWasClicked(string command)
  {
    clickDelegate.SendMessage(clickCallback, command, SendMessageOptions.DontRequireReceiver);

    // hide the menu
    gameObject.SetActive(false);
  }

}
