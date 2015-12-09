using UnityEngine;

public class CartClick : MonoBehaviour
{
    private AttachCamera attachCamera;
    private RunWithPath runWithPath;

    protected virtual void Awake()
    {
        runWithPath = GetComponent<RunWithPath>();
        attachCamera = GetComponent<AttachCamera>();
    }

    protected virtual void OnClick()
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

        // Start the cart running its path.
        if (runWithPath != null)
        {
            runWithPath.enabled = true;
        }
        else
        {
            Debug.LogError("CartClick: RunWithPath is null.");
        }
    }
}