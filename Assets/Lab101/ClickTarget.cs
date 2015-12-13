using System;
using System.Collections;
using UnityEngine;

public class ClickTarget : MonoBehaviour
{
    private const float FADE_DURATION_SECONDS = 0.5f;

    [SerializeField]
    private Transform movePositionTransform = null;

    [SerializeField]
    private Material[] materials = null;

    /// <summary>
    ///     Gets whether the game object is targetable.
    /// </summary>
    public bool IsTargetable
    {
        get
        {
            return (gameObject.tag == "targetable");
        }
    }

    public static event Action DestinationReached;

    public static event Action MovingCamera;

    protected virtual void Awake()
    {
        MovingCamera += OnMovingCamera;
        DestinationReached += OnDestinationReached;
    }

    protected virtual void OnClick()
    {
        if (IsTargetable && movePositionTransform != null)
        {
            StartCoroutine(MoveCamera(movePositionTransform.position));
        }
    }

    protected virtual void OnDestroy()
    {
        MovingCamera -= OnMovingCamera;
    }

    protected virtual void Start()
    {
        SetTargetable(true);
        SetMaterialsAlpha(1.0f);
    }

    /// <summary>
    ///     Fades the attached materials from an alpha value to another.
    /// </summary>
    private IEnumerator Fade(float from, float to, float seconds)
    {
        if (materials != null && materials.Length > 0)
        {
            float progress = 0.0f;
            while (progress <= 1.0f)
            {
                progress = progress + (Time.deltaTime / seconds);
                SetMaterialsAlpha(Mathf.Lerp(from, to, progress));
                yield return null;
            }
        }
    }

    /// <summary>
    ///     Moves the camera to a new position.
    /// </summary>
    private IEnumerator MoveCamera(Vector3 newPosition)
    {
        GameObject cameraObject = AppCentral.APP.GetCameraObject();
        if (cameraObject != null)
        {
            if (MovingCamera != null)
            {
                MovingCamera.Invoke();
            }

            Vector3 startPosition = cameraObject.transform.position;
            newPosition.y = startPosition.y;

            float progress = 0.0f;
            while (progress <= 1.0f)
            {
                progress = progress + Time.deltaTime;
                cameraObject.transform.position = Vector3.Lerp(startPosition, newPosition, progress);
                yield return null;
            }

            if (DestinationReached != null)
            {
                DestinationReached.Invoke();
            }
        }
    }

    /// <summary>
    ///     Called when the camera reaches its destination.
    /// </summary>
    private void OnDestinationReached()
    {
        StartCoroutine(Fade(0.0f, 1.0f, FADE_DURATION_SECONDS));
        SetTargetable(true);
    }

    /// <summary>
    ///     Called when the camera starts moving.
    /// </summary>
    private void OnMovingCamera()
    {
        StartCoroutine(Fade(1.0f, 0.0f, FADE_DURATION_SECONDS));
        SetTargetable(false);
    }

    /// <summary>
    ///     Sets attached material alpha values.
    /// </summary>
    private void SetMaterialsAlpha(float alpha)
    {
        if (materials != null)
        {
            for (int i = 0; i < materials.Length; ++i)
            {
                Material material = materials[i];
                if (material != null)
                {
                    // check if the material is using a particle shader.
                    if (material.shader.name == "Particles/Additive")
                    {
                        // must adjust the shader's tint colour property.
                        Color colour = material.GetColor("_TintColor");
                        colour.a = alpha;
                        material.SetColor("_TintColor", colour);
                    }
                    else
                    {
                        // can adjust the regular main texture colour property.
                        Color colour = material.color;
                        colour.a = alpha;
                        material.color = colour;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Gets the targetable state of the game object.
    /// </summary>
    private void SetTargetable(bool state)
    {
        if (state)
        {
            gameObject.tag = "targetable";
        }
        else
        {
            gameObject.tag = "Untagged";
        }
    }
}