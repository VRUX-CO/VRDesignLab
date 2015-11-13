using UnityEngine;
using System.Collections;

public enum ReticleState
{
  kUnknown,
  kOpen,
  kClosed,
};

public class AnimateTiledTexture : MonoBehaviour
{
  public int _columns = 2;                        // The number of columns of the texture
  public int _rows = 2;                           // The number of rows of the texture
  public int _maxRows = 0;                        // keep 0 if you want rows*cols.  If you have less frames than rows*cols, set it here

  int _index = 0;                         // Keeps track of the current frame 
  Vector2 _textureSize = Vector2.zero;    // Keeps track of the texture scale 
  long _currentCoroutineIndex = 0;
  float _framesPerSecond = 60f;
  ReticleState _reticleState = ReticleState.kUnknown;
  bool visible = true;
  Renderer reticleRenderer;

  public void SetState(ReticleState newState)
  {
    if (_reticleState != newState)
    {
      _reticleState = newState;

      _currentCoroutineIndex++;

      // Start the update tiling coroutine
      StartCoroutine(updateTiling());
    }
  }

  void Awake()
  {
    reticleRenderer = GetComponent<Renderer>();
    reticleRenderer.enabled = visible;

    // Create the material instance, if needed. else, just use this function to recalc the texture size
    CalcTextureSize();

    // Assign the new texture size
    reticleRenderer.sharedMaterial.SetTextureScale("_MainTex", _textureSize);

    // index is 0, set up for default state
    ApplyOffset();
  }

  public void SetVisible(bool inVisible)
  {
    if (visible != inVisible)
    {
      visible = inVisible;

      reticleRenderer.enabled = visible;
    }
  }

  void CalcTextureSize()
  {
    //set the tile size of the texture (in UV units), based on the rows and columns
    _textureSize = new Vector2(1f / _columns, 1f / _rows);
  }

  // The main update function of this script
  IEnumerator updateTiling()
  {
    long routineIndex = _currentCoroutineIndex;

    // This is the max number of frames
    int maxIndex = (_rows * _columns);

    // _maxRows defaults to zero, but if set use it
    if (_maxRows > 0)
      maxIndex = _maxRows;

    while (true)
    {
      bool breakOutLoop = false;

      if (routineIndex != _currentCoroutineIndex)
      {
        // our cheap way of short circuiting a running coroutine
        // wanted to avoid any rare timing issues. but not sure if this is the best way.
        breakOutLoop = true;
      }
      else
      {
        ApplyOffset();

        int newIndex = _index;

        if (_reticleState == ReticleState.kOpen)
        {
          newIndex++;

          if (newIndex >= maxIndex)
            breakOutLoop = true;
        }
        else
        {
          newIndex--;

          if (newIndex < 0)
            breakOutLoop = true;
        }

        if (!breakOutLoop)
          _index = newIndex;
      }

      if (breakOutLoop)
        break;
      else
        yield return new WaitForSeconds(1f / _framesPerSecond);
    }
  }

  void ApplyOffset()
  {
    float x = (_index % _columns) / (float)_columns;
    float y = 1f - (Mathf.Floor((_index / _columns)) / (float)_columns);

    // y == 1 is off the texture, shift y down by one slot
    y -= (1f / _columns);

    Vector2 offset = new Vector2(x, y);

    offset.x += ((1f / _columns) - _textureSize.x) / 2.0f;
    offset.y += ((1f / _rows) - _textureSize.y) / 2.0f;

    // Update the material
    reticleRenderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
  }
}