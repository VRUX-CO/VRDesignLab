using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface CrosshairTargetable
{
  bool IsTargetable();
}

public class AnimateTiledTexture : MonoBehaviour
{
  public int _columns = 2;                        // The number of columns of the texture
  public int _rows = 2;                           // The number of rows of the texture
  public Vector2 _scale = new Vector3(1f, 1f);    // Scale the texture. This must be a non-zero number. negative scale flips the image
  public int _maxRows = 0;                        // keep 0 if you want rows*cols.  If you have less frames than rows*cols, set it here

  private int _index = 0;                         // Keeps track of the current frame 
  private Vector2 _textureSize = Vector2.zero;    // Keeps track of the texture scale 
  private bool _isPlaying = false;                // A flag to determine if the animation is currently playing

  private bool _forwards = true;    // set to backwards to animate in reverse
  private long _currentCoroutineIndex = 0;
  private float _framesPerSecond = 24f;

  public void Play(bool openReticle)
  {
    _forwards = openReticle;

    _currentCoroutineIndex++;

    // Start the update tiling coroutine
    StartCoroutine(updateTiling());
  }

  private void Awake()
  {
    GetComponent<Renderer>().enabled = true;

    // Create the material instance, if needed. else, just use this function to recalc the texture size
    CalcTextureSize();

    // Assign the new texture size
    GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", _textureSize);

    // index is 0, set up for default state
    ApplyOffset();
  }

  private void CalcTextureSize()
  {
    //set the tile size of the texture (in UV units), based on the rows and columns
    _textureSize = new Vector2(1f / _columns, 1f / _rows);

    // Add in the scale
    _textureSize.x = _textureSize.x / _scale.x;
    _textureSize.y = _textureSize.y / _scale.y;
  }

  // The main update function of this script
  private IEnumerator updateTiling()
  {
    long routineIndex = _currentCoroutineIndex;

    _isPlaying = true;

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

        if (_forwards)
          newIndex++;
        else
          newIndex--;

        if (_forwards)
        {
          if (newIndex >= maxIndex)
            breakOutLoop = true;
        }
        else
        {
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

    _isPlaying = false;
  }

  private void ApplyOffset()
  {
    float x = (_index % _columns) / (float)_columns;
    float y = 1f - (Mathf.Floor((_index / _columns)) / (float)_columns);

    // y == 1 is off the texture, shift y down by one slot
    y -= (1f / _columns);

    Vector2 offset = new Vector2(x, y);

    // If we have scaled the texture, we need to reposition the texture to the center of the object
    offset.x += ((1f / _columns) - _textureSize.x) / 2.0f;
    offset.y += ((1f / _rows) - _textureSize.y) / 2.0f;

    // Update the material
    GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
  }
}