using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBillboard : MonoBehaviour
{
    public const float FADE_DURATION_SECONDS = 1.0f;

    [SerializeField]
    private Text title = null;

    [SerializeField]
    private Text message = null;

    private CanvasGroup canvasGroup;
    private bool destroyOnHide;
    private bool isFading;

    /// <summary>
    ///     Creates and returns a new billboard canvas.
    /// </summary>
    public static CanvasBillboard Billboard(GameObject billboardPrefab, Vector3 position, Transform parent, bool destroyOnHide)
    {
        CanvasBillboard canvasBillboard = null;

        if (billboardPrefab != null)
        {
            // create billboard object from prefab
            GameObject canvasBillboardObject = Instantiate(billboardPrefab);
            canvasBillboardObject.transform.SetParent(parent, false);
            canvasBillboardObject.transform.localPosition = position;

            canvasBillboard = canvasBillboardObject.GetComponent<CanvasBillboard>();

            // set billboard data
            if (canvasBillboard != null)
            {
                canvasBillboard.destroyOnHide = destroyOnHide;
            }
        }

        return canvasBillboard;
    }

    /// <summary>
    ///     Hides the billboard.
    /// </summary>
    public void Hide(float delayInSeconds = 0.0f)
    {
        if (canvasGroup != null)
        {
            // Stop currently updating fades.
            if (isFading)
            {
                StopAllCoroutines();
            }

            // start fade
            StartCoroutine(Fade(canvasGroup.alpha, 0.0f, FADE_DURATION_SECONDS, delayInSeconds, OnHideComplete));
        }
    }

    /// <summary>
    ///     Shows the billboard.
    /// </summary>
    public void Show(float delayInSeconds = 0.0f)
    {
        if (canvasGroup != null)
        {
            // stop currently updating fades
            if (isFading)
            {
                StopAllCoroutines();
            }

            // start fade
            StartCoroutine(Fade(canvasGroup.alpha, 1.0f, FADE_DURATION_SECONDS, delayInSeconds, null));
        }
    }

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    ///     Coroutine used to fade the billboard from one alpha value to another.
    /// </summary>
    private IEnumerator Fade(float alphaFrom, float alphaTo, float durationInSeconds, float delayInSeconds, Action onFadeComplete)
    {
        isFading = true;

        // delay
        yield return new WaitForSeconds(delayInSeconds);

        if (canvasGroup != null)
        {
            float progress = 0.0f;

            // fade
            while (progress <= 1.0f)
            {
                progress = progress + (Time.deltaTime / durationInSeconds);
                canvasGroup.alpha = Mathf.Lerp(alphaFrom, alphaTo, progress);
                yield return null;
            }

            // callback
            if (onFadeComplete != null)
            {
                onFadeComplete.Invoke();
            }
        }

        isFading = false;
    }

    /// <summary>
    ///     Callback for hide complete to destroy the billboard if required.
    /// </summary>
    private void OnHideComplete()
    {
        if (destroyOnHide)
        {
            Destroy(gameObject);
        }
    }
}