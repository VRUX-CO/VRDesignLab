/************************************************************************************

Filename    :   Crosshair3D.cs
Content     :   An example of a 3D cursor in the world based on player view
Created     :   June 30, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

// uncomment this to test the different modes.
#define CROSSHAIR_TESTING

using UnityEngine;
using System.Collections;				// required for Coroutines

public class Crosshair3D : MonoBehaviour
{

  // NOTE: three different crosshair methods are shown here.  The most comfortable for the
  // user is going to be when the crosshair is located in the world (or slightly in front of)
  // the position where the user's gaze is.  Positioning the cursor a fixed distance from the
  // camera inbetween the camera and the player's gaze will be uncomfortable and unfocused.
  public enum CrosshairMode
  {
    Dynamic = 0,			// cursor positions itself in 3D based on raycasts into the scene
    DynamicObjects = 1,		// similar to Dynamic but cursor is only visible for objects in a specific layer
    FixedDepth = 2,			// cursor positions itself based on camera forward and draws at a fixed depth
  }

  public CrosshairMode mode = CrosshairMode.Dynamic;
  public int objectLayer = 8;
  public float offsetFromObjects = 0.1f;
  public float fixedDepth = 3.0f;
  public OVRCameraRig cameraController = null;

  private Transform thisTransform = null;
  private Material crosshairMaterial = null;
  private Vector3 originalScale;
  private GameObject previousHitGameObject;
  private AnimateTiledTexture _animatedCrosshair;

  //private float distance;

  /*void Start()
  {
      originalScale = this.transform.localScale;
  }*/

  /// <summary>
  /// Initialize the crosshair
  /// </summary>
  void Awake()
  {
    thisTransform = transform;
    if (cameraController == null)
    {
      Debug.LogError("ERROR: missing camera controller object on " + name);
      enabled = false;
      return;
    }

    // not required
    _animatedCrosshair = GetComponent<AnimateTiledTexture>();

    // clone the crosshair material
    crosshairMaterial = GetComponent<Renderer>().material;
  }

  /// <summary>
  /// Cleans up the cloned material
  /// </summary>
  void OnDestroy()
  {
    if (crosshairMaterial != null)
    {
      Destroy(crosshairMaterial);
    }
  }

  /// <summary>
  /// Updates the position of the crosshair.
  /// </summary>
  void LateUpdate()
  {
#if CROSSHAIR_TESTING
    if (Input.GetButtonDown("RightShoulder"))
    {
      //*************************
      // toggle the crosshair mode .. dynamic -> dynamic objects -> fixed depth
      //*************************
      switch (mode)
      {
        case CrosshairMode.Dynamic:
          mode = CrosshairMode.DynamicObjects;
          crosshairMaterial.color = Color.red;
          break;
        case CrosshairMode.DynamicObjects:
          mode = CrosshairMode.FixedDepth;
          crosshairMaterial.color = Color.blue;
          break;
        case CrosshairMode.FixedDepth:
          mode = CrosshairMode.Dynamic;
          crosshairMaterial.color = Color.white;
          break;
      }
      Debug.Log("Mode: " + mode);
    }
#endif
    Ray ray;
    RaycastHit hit;

    // get the camera forward vector and position
    Vector3 cameraPosition = cameraController.centerEyeAnchor.position;
    Vector3 cameraForward = cameraController.centerEyeAnchor.forward;

    GetComponent<Renderer>().enabled = true;

    //*************************
    // position the cursor based on the mode
    //*************************
    switch (mode)
    {
      case CrosshairMode.Dynamic:
        // cursor positions itself in 3D based on raycasts into the scene
        // trace to the spot that the player is looking at
        ray = new Ray(cameraPosition, cameraForward);
        if (Physics.Raycast(ray, out hit))
        {
          //distance = hit.distance;
          thisTransform.position = hit.point + (-cameraForward * offsetFromObjects);
          thisTransform.forward = -cameraForward;
          // thisTransform.localScale = originalScale * distance;
        }
        break;
      case CrosshairMode.DynamicObjects:
        // similar to Dynamic but cursor is only visible for objects in a specific layer
        ray = new Ray(cameraPosition, cameraForward);
        if (Physics.Raycast(ray, out hit))
        {
          if (hit.transform.gameObject.layer != objectLayer)
          {
            GetComponent<Renderer>().enabled = false;
          }
          else
          {
            thisTransform.position = hit.point + (-cameraForward * offsetFromObjects);
            thisTransform.forward = -cameraForward;
          }
        }
        break;
      case CrosshairMode.FixedDepth:
        // cursor positions itself based on camera forward and draws at a fixed depth
        thisTransform.position = cameraPosition + (cameraForward * fixedDepth);
        thisTransform.forward = -cameraForward;
        break;
    }

    // I don't fully understand how this works, but to get Button A working on XBox controller, had to add Button A
    if (Input.GetButtonDown(OVRGamepadController.ButtonNames[(int)OVRGamepadController.Button.A]) ||  // "Desktop_Button A"
      Input.GetButtonDown("Button A"))
    {
      ray = new Ray(cameraPosition, cameraForward);
      if (Physics.Raycast(ray, out hit))
      {
        hit.transform.gameObject.BroadcastMessage("OnClick", SendMessageOptions.DontRequireReceiver);
      }
    }


    // steve's bullshit
    if (true)
    {
      ray = new Ray(cameraPosition, cameraForward);
      if (Physics.Raycast(ray, out hit))
      {
        if (previousHitGameObject != hit.transform.gameObject)
        {
          EndHover();

          hit.transform.gameObject.BroadcastMessage("OnHoverStart", SendMessageOptions.DontRequireReceiver);

          if (hit.transform.gameObject.GetComponent<CrosshairTargetable>() != null)
          {
            _animatedCrosshair.Play(true);
          }

          previousHitGameObject = hit.transform.gameObject;
        }
      }
      else
      {
        EndHover();
      }
    }
  }

  void EndHover()
  {
    if (previousHitGameObject != null)
    {
      previousHitGameObject.BroadcastMessage("OnHoverEnd", SendMessageOptions.DontRequireReceiver);

      if (previousHitGameObject.GetComponent<CrosshairTargetable>() != null)
      {
        _animatedCrosshair.Play(false);
      }

      previousHitGameObject = null;
    }
  }
}
