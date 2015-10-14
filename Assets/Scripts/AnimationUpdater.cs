using UnityEngine;
using System.Collections;

public class AnimationUpdater
{
  float duration = .5f;

  float timeStartedLerping;
  bool _isRunning = false;

  public void StartUpdater(float inDuration)
  {
    duration = inDuration;
    timeStartedLerping = Time.time;

    _isRunning = true;
  }

  public float PercentageComplete()
  {
    float result = 1f;

    if (_isRunning)
    {
      float timeSinceStarted = Time.time - timeStartedLerping;
      float percentageComplete = timeSinceStarted / duration;

      result = percentageComplete;

      if (percentageComplete >= 1.0f)
        _isRunning = false;
    }

    return result;
  }

  public bool IsRunning()
  {
    return _isRunning;
  }

}
