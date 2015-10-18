using UnityEngine;
using System.Collections;

public class CubeSpawnerCube : MonoBehaviour
{
  // Use this for initialization
  void Start()
  {
    float startScale = 3f;
    float yScale = 40f;
    float moveTime = 12f;

    Vector3 startPosition = transform.position;

    transform.localScale = new Vector3(startScale, 0, startScale);

    iTween.ScaleTo(gameObject, new Vector3(startScale, yScale, startScale), Random.RandomRange(9f, 13f));

    startPosition.z = -4;
    iTween.MoveTo(gameObject, iTween.Hash("position", startPosition, "time", moveTime, "easeType", iTween.EaseType.linear, "oncomplete", "OnCompleteCallback"));





  }

  public void OnCompleteCallback()
  {
    Destroy(gameObject);
  }

}
