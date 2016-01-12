using System;
using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    private const float FADE_TRANSITION_SECONDS = 1.0f;
    private const float FADE_DELAY_SECONDS = 1.0f;

    private static GameObject screenFaderPrefab;

    [SerializeField]
    private Material faderMaterial = null;

    private Action fadeEnd;
    private Action fadeMiddle;
    private Action fadeStart;

    public static bool IsFaded { get; private set; }

    public static GameObject ScreenFaderPrefab
    {
        get
        {
            if (screenFaderPrefab == null)
            {
                screenFaderPrefab = Resources.Load<GameObject>("Prefab-ScreenFader");
            }
            return screenFaderPrefab;
        }
    }

    public static void Fade(Action fadeStart, Action fadeMiddle, Action fadeEnd)
    {
        if (ScreenFaderPrefab != null)
        {
            GameObject screenFaderObject = Instantiate(ScreenFaderPrefab);
            if (screenFaderObject != null)
            {
                ScreenFader screenFader = screenFaderObject.GetComponent<ScreenFader>();
                if (screenFader != null)
                {
                    screenFader.fadeStart = fadeStart;
                    screenFader.fadeMiddle = fadeMiddle;
                    screenFader.fadeEnd = fadeEnd;
                }
            }
        }
    }

    protected virtual void OnDestroy()
    {
        IsFaded = false;
    }

    protected virtual void Start()
    {
        transform.SetParent(Camera.main.transform, false);

        IsFaded = true;
        SetAlpha(0.0f);

        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        if (fadeStart != null)
        {
            fadeStart.Invoke();
        }

        yield return StartCoroutine(Fade(0.0f, 1.0f, FADE_TRANSITION_SECONDS, fadeMiddle));
        yield return new WaitForSeconds(FADE_DELAY_SECONDS);
        yield return StartCoroutine(Fade(1.0f, 0.0f, FADE_TRANSITION_SECONDS, fadeEnd));

        Destroy(gameObject);
    }

    private IEnumerator Fade(float from, float to, float seconds, Action callback)
    {
        if (faderMaterial != null)
        {
            float progress = 0.0f;

            while (progress <= 1.0f)
            {
                progress = progress + (Time.deltaTime / seconds);
                SetAlpha(Mathf.Lerp(from, to, progress));
                yield return null;
            }

            if (callback != null)
            {
                callback.Invoke();
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        if (faderMaterial != null)
        {
            Color colour = faderMaterial.color;
            colour.a = Mathf.Clamp01(alpha);
            faderMaterial.color = colour;
        }
    }
}