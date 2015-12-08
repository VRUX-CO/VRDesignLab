using UnityEngine;
using System.Collections;

public class AttachCamera : MonoBehaviour {

    public Vector3 cameraPositionOffset;
    public float cameraRotation;

    // Set the maincamera as a child of this game object.
    void Start ()
    {
        AppCentral.APP.AttachCamera(gameObject);

        transform.position = cameraPositionOffset;


        transform.Rotate(Vector3.up * cameraRotation);
    }
}
