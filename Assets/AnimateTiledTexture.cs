using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimateTiledTexture : MonoBehaviour
{
  public int _columns = 2;                        // The number of columns of the texture
  public int _rows = 2;                           // The number of rows of the texture
  public Vector2 _scale = new Vector3(1f, 1f);    // Scale the texture. This must be a non-zero number. negative scale flips the image
  public float _framesPerSecond = 10f;            // Frames per second that you want to texture to play at
  public bool _newMaterialInstance = false;       // Set this to true if you want to create a new material instance
  public int _maxRows = 0;                        // keep 0 if you want rows*cols.  If you have less frames than rows*cols, set it here

  private int _index = 0;                         // Keeps track of the current frame 
  private Vector2 _textureSize = Vector2.zero;    // Keeps track of the texture scale 
  private Material _materialInstance = null;      // Material instance of the material we create (if needed)
  private bool _hasMaterialInstance = false;      // A flag so we know if we have a material instance we need to clean up (better than a null check i think)
  private bool _isPlaying = false;                // A flag to determine if the animation is currently playing

  public delegate void VoidEvent();               // The Event delegate
  private List<VoidEvent> _voidEventCallbackList; // A list of functions we need to call if events are enabled

  public void RegisterCallback(VoidEvent cbFunction)
  {
    _voidEventCallbackList.Add(cbFunction);
  }

  public void UnRegisterCallback(VoidEvent cbFunction)
  {
    _voidEventCallbackList.Remove(cbFunction);
  }

  public void Play()
  {
    if (_isPlaying)
    {
      StopCoroutine("updateTiling");
      _isPlaying = false;
    }
    // Make sure the renderer is enabled
    GetComponent<Renderer>().enabled = true;

    // Start the update tiling coroutine
    StartCoroutine(updateTiling());
  }

  public void ChangeMaterial(Material newMaterial, bool newInstance = false)
  {
    if (newInstance)
    {
      // First check our material instance, if we already have a material instance
      // and we want to create a new one, we need to clean up the old one
      if (_hasMaterialInstance)
        Object.Destroy(GetComponent<Renderer>().sharedMaterial);

      // create the new material
      _materialInstance = new Material(newMaterial);

      // Assign it to the renderer
      GetComponent<Renderer>().sharedMaterial = _materialInstance;

      // Set the flag
      _hasMaterialInstance = true;
    }
    else // if we dont have create a new instance, just assign the texture
      GetComponent<Renderer>().sharedMaterial = newMaterial;

    // We need to recalc the texture size (since different material = possible different texture)
    CalcTextureSize();

    // Assign the new texture size
    GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", _textureSize);
  }

  private void Awake()
  {
    _voidEventCallbackList = new List<VoidEvent>();

    //Create the material instance, if needed. else, just use this function to recalc the texture size
    ChangeMaterial(GetComponent<Renderer>().sharedMaterial, _newMaterialInstance);
  }

  private void OnDestroy()
  {
    if (_hasMaterialInstance)
    {
      Object.Destroy(GetComponent<Renderer>().sharedMaterial);
      _hasMaterialInstance = false;
    }
  }

  private void HandleCallbacks(List<VoidEvent> cbList)
  {
    // For now simply loop through them all and call yet.
    for (int i = 0; i < cbList.Count; ++i)
      cbList[i]();
  }

  private void OnEnable()
  {
    CalcTextureSize();
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
    _isPlaying = true;

    // This is the max number of frames
    int checkAgainst = (_rows * _columns);

    // _maxRows defaults to zero, but if set use it
    if (_maxRows > 0)
      checkAgainst = _maxRows;

    while (true)
    {
      if (_index >= checkAgainst)
      {
        _index = 0;  // Reset the index
      }

      ApplyOffset();

      _index++;

      yield return new WaitForSeconds(1f / _framesPerSecond);
    }
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