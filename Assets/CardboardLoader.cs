using UnityEngine;
using System.Collections;

public class CardboardLoader : MonoBehaviour
{
  public GameObject cardboardPrefab;
  public bool buildingForCardboard = false;

  // Use this for initialization
  void Start()
  {
    if (buildingForCardboard)
    {
      Instantiate(cardboardPrefab, new Vector3(0, 1.2f, 2f), Quaternion.identity);

      Cardboard cardboard = GetComponent<Cardboard>();
      cardboard.VRModeEnabled = true;
    }
  }
}
