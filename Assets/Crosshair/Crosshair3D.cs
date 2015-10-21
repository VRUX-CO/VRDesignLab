// uncomment this to test the different modes.
// #define CROSSHAIR_TESTING

using UnityEngine;
using System.Collections;

public class Crosshair3D : MonoBehaviour
{
  static public string kCrosshairTargetable = "targetable";

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

  GameObject previousHitGameObject;
  GameObject previousHitButtonRevealer;
  AnimateTiledTexture _animatedCrosshair;
  OVRCameraRig cameraController = null;
  bool showOnClickOnly = false;

  void Awake()
  {
    cameraController = FindObjectOfType<OVRCameraRig>();

    // not required
    _animatedCrosshair = GetComponent<AnimateTiledTexture>();
  }

  void LateUpdate()
  {
    Ray ray;
    RaycastHit hit;
    bool cursorSet = false;

    CrosshairTesting();

    Ray camRay = CameraRay();
    Vector3 cameraPosition = camRay.origin;
    Vector3 cameraForward = camRay.direction;

    GetComponent<Renderer>().enabled = true;

    switch (mode)
    {
      case CrosshairMode.Dynamic:
        // cursor positions itself in 3D based on raycasts into the scene
        // trace to the spot that the player is looking at
        ray = new Ray(cameraPosition, cameraForward);
        if (Physics.Raycast(ray, out hit))
        {
          // distance = hit.distance;
          transform.position = hit.point + (-cameraForward * offsetFromObjects);

          cursorSet = true;
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
            transform.position = hit.point + (-cameraForward * offsetFromObjects);

            cursorSet = true;
          }
        }
        break;
      case CrosshairMode.FixedDepth:
        // gets set below as the default mode when fixed or nothing hit
        break;
    }

    if (!cursorSet)
    {
      transform.position = cameraPosition + (cameraForward * fixedDepth);
    }

    // keeps the sprite pointed towards camera
    transform.forward = cameraForward;

    if (Utilities.UserClicked())
    {
      ray = new Ray(cameraPosition, cameraForward);
      if (Physics.Raycast(ray, out hit))
      {
        hit.transform.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
      }
    }

    UpdateHoverState();
    UpdateButtonRevealer();
  }

  Ray CameraRay()
  {
    Vector3 cameraPosition;
    Vector3 cameraForward;

    if (cameraController == null)
    {
      cameraPosition = Camera.main.transform.position;
      cameraForward = Camera.main.transform.forward;
    }
    else
    {
      // get the camera forward vector and position
      cameraPosition = cameraController.centerEyeAnchor.position;
      cameraForward = cameraController.centerEyeAnchor.forward;
    }

    return new Ray(cameraPosition, cameraForward);
  }

  void EndHover()
  {
    if (previousHitGameObject != null)
    {
      previousHitGameObject.SendMessage("OnHoverEnd", SendMessageOptions.DontRequireReceiver);
      previousHitGameObject = null;
    }

    // it's save to call this every time.  sometimes previousHitGameObject becomes null and testing previousHitGameObject.tag == kCrosshairTargetable fails?
    _animatedCrosshair.SetState(ReticleState.kClosed);
  }

  void UpdateHoverState()
  {
    Ray ray;
    RaycastHit hit;

    // get the camera forward vector and position
    Ray camRay = CameraRay();
    Vector3 cameraPosition = camRay.origin;
    Vector3 cameraForward = camRay.direction;

    ray = new Ray(cameraPosition, cameraForward);
    if (Physics.Raycast(ray, out hit))
    {
      if (previousHitGameObject != hit.transform.gameObject)
      {
        EndHover();

        hit.transform.gameObject.SendMessage("OnHoverStart", SendMessageOptions.DontRequireReceiver);

        if (hit.transform.gameObject.tag == kCrosshairTargetable)
        {
          _animatedCrosshair.SetState(ReticleState.kOpen);
        }

        previousHitGameObject = hit.transform.gameObject;
      }
    }
    else
    {
      EndHover();
    }
  }

  void UpdateButtonRevealer()
  {
    Ray ray;
    RaycastHit hit;

    // get the camera forward vector and position
    Ray camRay = CameraRay();
    Vector3 cameraPosition = camRay.origin;
    Vector3 cameraForward = camRay.direction;

    ray = new Ray(cameraPosition, cameraForward);

    int layerMask = 1 << 11;

    // Does the ray intersect any objects which are in the player layer.
    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
    {
      if (previousHitButtonRevealer != hit.transform.gameObject)
      {
        EndButtonRevealer();

        hit.transform.gameObject.SendMessage("OnRevealStart", SendMessageOptions.DontRequireReceiver);

        previousHitButtonRevealer = hit.transform.gameObject;
      }
    }
    else
    {
      EndButtonRevealer();
    }
  }

  void EndButtonRevealer()
  {
    if (previousHitButtonRevealer != null)
    {
      previousHitButtonRevealer.SendMessage("OnRevealEnd", SendMessageOptions.DontRequireReceiver);

      previousHitButtonRevealer = null;
    }
  }

  void CrosshairTesting()
  {
#if CROSSHAIR_TESTING
    if (Input.GetButtonDown("RightShoulder"))
    {
      Material crosshairMaterial = GetComponent<Renderer>().material;

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
  }

  public void ShowReticleOnClick(bool showOnClick)
  {
    showOnClickOnly = showOnClick;

    _animatedCrosshair.SetVisible(!showOnClickOnly);
  }

  void UpdateCrosshairOnClick()
  {
    if (showOnClickOnly)
    {
      _animatedCrosshair.SetVisible(true);
      StartCoroutine(FadeOutCrosshair(1));
    }
  }

  IEnumerator FadeOutCrosshair(float delay)
  {
    yield return new WaitForSeconds(delay);

    if (showOnClickOnly)
    {
      _animatedCrosshair.SetVisible(false);
    }
  }

}
