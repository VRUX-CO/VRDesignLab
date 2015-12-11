using UnityEngine;

public class FixWorldRotation : MonoBehaviour
{
    [SerializeField]
    private bool x = false;

    [SerializeField]
    private bool y = false;

    [SerializeField]
    private bool z = false;

    private Vector3 originalRotation;

    protected virtual void LateUpdate()
    {
        if (x || y || z)
        {
            Vector3 currentRotation = transform.eulerAngles;

            if (x)
            {
                currentRotation.x = originalRotation.x;
            }

            if (y)
            {
                currentRotation.y = originalRotation.y;
            }

            if (z)
            {
                currentRotation.z = originalRotation.z;
            }

            transform.eulerAngles = currentRotation;
        }
    }

    protected virtual void Start()
    {
        Reset();
    }

    private void Reset()
    {
        originalRotation = transform.eulerAngles;
    }
}