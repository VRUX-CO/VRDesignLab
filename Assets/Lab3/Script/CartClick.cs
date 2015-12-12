using UnityEngine;

public class CartClick : CartTargetable
{
    private AttachCamera attachCamera;
    private RunWithPath runWithPath;

    public void Ride()
    {
        if (IsTargetable)
        {
            ScreenFader.Fade(null, () =>
            {
                // Attach the camera to the cart.
                if (attachCamera != null)
                {
                    attachCamera.Attach();
                }
                else
                {
                    Debug.LogError("CartClick: AttachCamera is null.");
                }
            }, () =>
            {
                // Start the cart running its path.
                if (runWithPath != null)
                {
                    runWithPath.enabled = true;
                }
                else
                {
                    Debug.LogError("CartClick: RunWithPath is null.");
                }
            });
        }
    }

    protected override void Awake()
    {
        base.Awake();

        runWithPath = GetComponent<RunWithPath>();
        attachCamera = GetComponent<AttachCamera>();
    }

    protected virtual void OnClick()
    {
        Ride();
    }
}