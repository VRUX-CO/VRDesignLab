using System.Collections;
using UnityEngine;

public class ClickTarget : MonoBehaviour
{
    [SerializeField]
    private Transform movePositionTransform = null;

    protected virtual void OnClick()
    {
        if (movePositionTransform != null)
        {
            StartCoroutine(MoveCamera(movePositionTransform.position));
        }
    }

    private IEnumerator MoveCamera(Vector3 newPosition)
    {
        GameObject cameraObject = AppCentral.APP.GetCameraObject();
        if (cameraObject != null)
        {
            Vector3 startPosition = cameraObject.transform.position;
            newPosition.y = startPosition.y;

            float progress = 0.0f;
            while (progress <= 1.0f)
            {
                progress = progress + Time.deltaTime;
                cameraObject.transform.position = Vector3.Lerp(startPosition, newPosition, progress);
                yield return null;
            }
        }
    }
}