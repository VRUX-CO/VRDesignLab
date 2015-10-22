using UnityEngine;
using System.Collections;

public class CubeSpawnerCube : MonoBehaviour
{
  const float startScale = 3f;
  const float yScale = 40f;
  const float moveTime = 12f;

  public void StartMoving(float inSpeed = 1f)
  {
    transform.localScale = new Vector3(startScale, 0, startScale);

    iTween.ScaleTo(gameObject, new Vector3(startScale, yScale, startScale), Random.RandomRange(9f, 13f) / inSpeed);

    Vector3 startPosition = transform.position;
    startPosition.z = -4;
    iTween.MoveTo(gameObject, iTween.Hash("position", startPosition, "time", moveTime / inSpeed, "easeType", iTween.EaseType.linear, "oncomplete", "OnCompleteCallback"));
  }

  void OnCompleteCallback()
  {
    Destroy(gameObject);
  }
}
