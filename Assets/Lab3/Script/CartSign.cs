using System.Collections;
using UnityEngine;

public class CartSign : CartTargetable
{
    private const float HOVER_SCALE = 1.2f;
    private const float SCALE_SECONDS = 0.25f;

    [SerializeField]
    private Transform hoverScaleTransform = null;

    [SerializeField]
    private CartClick cartClick = null;

    protected virtual void OnClick()
    {
        if (cartClick != null)
        {
            cartClick.Ride();
        }
    }

    protected virtual void OnHoverEnd()
    {
        if (hoverScaleTransform != null)
        {
            StartCoroutine(ScaleTo(hoverScaleTransform, HOVER_SCALE, 1.0f, SCALE_SECONDS));
        }
    }

    protected virtual void OnHoverStart()
    {
        if (hoverScaleTransform != null)
        {
            StartCoroutine(ScaleTo(hoverScaleTransform, 1.0f, HOVER_SCALE, SCALE_SECONDS));
        }
    }

    protected override void Start()
    {
        base.Start();

        Utilities.RotateToFaceCamera(transform, Camera.main, false, true, false);
    }

    private IEnumerator ScaleTo(Transform obj, float from, float to, float seconds)
    {
        if (obj != null)
        {
            float progress = 0.0f;

            while (progress <= 1.0f)
            {
                progress = progress + (Time.deltaTime / seconds);
                obj.localScale = Vector3.one * Mathf.Lerp(from, to, progress);
                yield return null;
            }
        }
    }
}