using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMenu : MonoBehaviour
{
  public GameObject itemPrefab;
  public GameObject clickDelegate;
  public string clickCallback;

  // Use this for initialization
  public void SetupItems(List<Dictionary<string, string>> inItems)
  {
    Vector3 position = gameObject.transform.position;
    int index = 0;
    const float itemHeight = .15f;
    float startY = position.y;
    int cnt = inItems.Count;
    float totalHeight = cnt * itemHeight;

    foreach (Dictionary<string, string> item in inItems)
    {
      position.y = startY + ((cnt - index) * itemHeight);

      // center vertically
      position.y -= totalHeight / 2;

      GameObject anItem = Instantiate(itemPrefab, position, Quaternion.identity) as GameObject;

      anItem.transform.parent = gameObject.transform;

      LevelMenuItem menuItem = anItem.GetComponent<LevelMenuItem>() as LevelMenuItem;

      menuItem.SetupItem(item, this);

      index++;
    }
  }

  public void ItemWasClicked(Dictionary<string, string> info)
  {
    clickDelegate.SendMessage(clickCallback, info, SendMessageOptions.DontRequireReceiver);
  }

}
