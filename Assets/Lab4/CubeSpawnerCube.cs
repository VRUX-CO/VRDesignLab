using UnityEngine;
using System.Collections;

public class CubeSpawnerCube : MonoBehaviour
{
  const float startScale = 3f;
  const float yScale = 40f;
  const float moveTime = 12f;

  public void StartMoving(bool scaleUp)
  {
    // scale up vertically
    transform.localScale = new Vector3(startScale, 0, startScale);
    iTween.ScaleTo(gameObject, new Vector3(startScale, Random.Range(15f, 48f), startScale), scaleUp ? Random.Range(9f, 13f) : 0f);

    // move from where we are now to endPosition
    Vector3 endPosition = transform.localPosition;

    float startZ = endPosition.z;
    endPosition.z = -6;

    float animationTime = moveTime;

    // animation time is set by distance to travel
    float distance = startZ - endPosition.z;
    animationTime *= distance / 80;

    iTween.MoveTo(gameObject, iTween.Hash("position", endPosition, "time", animationTime, "easeType", iTween.EaseType.linear, "oncomplete", "OnCompleteCallback"));
  }

  void OnCompleteCallback()
  {
    Destroy(gameObject);
  }
}
