using UnityEngine;
using System.Collections;

public class BalloonClick : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void OnClick()
  {



    // destroy everything
    foreach (Transform child in this.transform)
    {
      Destroy(child.gameObject);
    }
    Destroy(this);
  }
}
