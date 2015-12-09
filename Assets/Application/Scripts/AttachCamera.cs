using System;
using UnityEngine;

public class AttachCamera : MonoBehaviour
{
    public Vector3 CameraPositionOffset;
    public float CameraRotation;

    // Detachment of the camera before destruction will not work if attached to this transform. A different transform
    // has to be used. A child transform of this transform will work fine though as the destruction call will not have
    // propogated to that level in the hierarchy.
    public Transform AttachmentPoint;

    public static event Action PreAttach;

    /// <summary>
    ///     Attach the main camera object to the attachment point.
    /// </summary>
    public void Attach()
    {
        if (enabled && AppCentral.APP != null && AttachmentPoint != null)
        {
            GameObject mainCameraObject = AppCentral.APP.GetCameraObject();

            if (mainCameraObject != null)
            {
                if (PreAttach != null)
                {
                    PreAttach.Invoke();
                }

                CameraPositionOffset = mainCameraObject.transform.localPosition + transform.position;
                mainCameraObject.transform.SetParent(AttachmentPoint, false);

                AttachmentPoint.position = CameraPositionOffset;
                AttachmentPoint.Rotate(Vector3.up * CameraRotation);
            }
        }
        else
        {
            enabled = true;
        }
    }

    /// <summary>
    ///     Detach the main camera object back to the scene root.
    /// </summary>
    public void Detach()
    {
        if (AppCentral.APP != null)
        {
            GameObject mainCameraObject = AppCentral.APP.GetCameraObject();

            if (mainCameraObject != null && mainCameraObject.transform.parent == AttachmentPoint)
            {
                mainCameraObject.transform.SetParent(null, false);
            }
        }
    }

    protected virtual void Awake()
    {
        PreAttach += Detach;
    }

    protected virtual void OnDestroy()
    {
        PreAttach -= Detach;
        Detach();
    }

    protected virtual void OnDisable()
    {
        Detach();
    }

    protected virtual void OnEnable()
    {
        Attach();
    }

    protected virtual void Start()
    {
        Attach();
    }
}