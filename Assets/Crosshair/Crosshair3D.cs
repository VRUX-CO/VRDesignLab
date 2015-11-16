using UnityEngine;
using System.Collections;

public class Crosshair3D : MonoBehaviour
{
  static public string kCrosshairTargetable = "targetable";
  static public int kRevealerLayer = 11;

  public float offsetFromObjects = 0.1f;
  public float fixedDepth = 3.0f;
  public bool matchDepthOnHitMode = false;

  GameObject previousHitGameObject;
  GameObject previousHitButtonRevealer;
  AnimateTiledTexture _animatedCrosshair;
  bool showOnClickOnly = false;

  void Awake()
  {
    // not required
    _animatedCrosshair = GetComponent<AnimateTiledTexture>();
  }

  void LateUpdate()
  {
    Ray ray;
    RaycastHit hit;
    bool cursorSet = false;
    Ray camRay = CameraRay();
    Vector3 cameraPosition = camRay.origin;
    Vector3 cameraForward = camRay.direction;

    if (matchDepthOnHitMode)  // disabled this, but might be needed later
    {
      // cursor positions itself in 3D based on raycasts into the scene
      // trace to the spot that the player is looking at
      ray = new Ray(cameraPosition, cameraForward);
      if (Physics.Raycast(ray, out hit))
      {
        // distance = hit.distance;
        transform.position = hit.point + (-cameraForward * offsetFromObjects);

        cursorSet = true;
      }
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
        SendEventToGameObject(hit.transform.gameObject, "OnClick");
      }

      // show cross hair temporarily if in the showOnClickOnly mode
      UpdateCrosshairOnClick();
    }

    UpdateHoverState();
    UpdateButtonRevealer();
  }

  Ray CameraRay()
  {
    Vector3 cameraPosition;
    Vector3 cameraForward;

    cameraPosition = Camera.main.transform.position;
    cameraForward = Camera.main.transform.forward;

    return new Ray(cameraPosition, cameraForward);
  }

  void SendEventToGameObject(GameObject go, string eventName)
  {
    // send to event delegate if exists, otherwise send it to the game object hit
    EventDelegate eventDelegate = go.GetComponent<EventDelegate>();
    if (eventDelegate)
      eventDelegate.SendMessage(eventName, SendMessageOptions.DontRequireReceiver);
    else
      go.SendMessage(eventName, SendMessageOptions.DontRequireReceiver);
  }

  void EndHover()
  {
    if (previousHitGameObject != null)
    {
      SendEventToGameObject(previousHitGameObject, "OnHoverEnd");
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

        SendEventToGameObject(hit.transform.gameObject, "OnHoverStart");

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

    // Does the ray intersect any objects which are in the player layer.
    if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << kRevealerLayer))
    {
      if (previousHitButtonRevealer != hit.transform.gameObject)
      {
        EndButtonRevealer();

        SendEventToGameObject(hit.transform.gameObject, "OnRevealStart");

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
      SendEventToGameObject(previousHitButtonRevealer, "OnRevealEnd");

      previousHitButtonRevealer = null;
    }
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
      StartCoroutine(FadeOutCrosshair(.5f));
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
