using UnityEngine;

public class CartClick : MonoBehaviour
{
    private AttachCamera attachCamera;
    private RunWithPath runWithPath;

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

    /// <summary>
    ///     Sets the targetable state of the game object.
    /// </summary>
    public void SetTargetable(bool state)
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

    protected virtual void Awake()
    {
        runWithPath = GetComponent<RunWithPath>();
        attachCamera = GetComponent<AttachCamera>();

        // subscribe to cart path events
        RunWithPath.PathStarted += OnPathStarted;
        RunWithPath.PathEnded += OnPathEnded;
    }

    protected virtual void OnClick()
    {
        if (IsTargetable)
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

    protected virtual void OnDestroy()
    {
        // unsubscribe to cart path events
        RunWithPath.PathStarted -= OnPathStarted;
        RunWithPath.PathEnded -= OnPathEnded;
    }

    protected virtual void Start()
    {
        SetTargetable(true);
    }

    /// <summary>
    ///     Called when a cart ends its path.
    /// </summary>
    private void OnPathEnded()
    {
        SetTargetable(true);
    }

    /// <summary>
    ///     Called when a cart starts its path.
    /// </summary>
    private void OnPathStarted()
    {
        SetTargetable(false);
    }
}