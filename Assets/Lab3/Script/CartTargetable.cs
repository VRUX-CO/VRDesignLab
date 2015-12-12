using UnityEngine;

public abstract class CartTargetable : MonoBehaviour
{
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
        // subscribe to cart path events
        RunWithPath.PathStarted += OnPathStarted;
        RunWithPath.PathEnded += OnPathEnded;
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