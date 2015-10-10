using UnityEngine;
using System.Collections;

public class ProceduralPlane : MonoBehaviour
{
  Camera m_Camera;

  // Use this for initialization
  void Start()
  {
    m_Camera = Camera.main;

    MeshUtilities.AddMeshComponent(gameObject, 1f, 2.5f);
    // StartCoroutine(Dupdate(0));
  }

  // Update is called once per frame
  void Update()
  {
    //   transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
    //         Vector3.forward);
  }

  IEnumerator Dupdate(int duh)
  {
    Debug.Log(string.Format("fuck: {0}", duh));

    Vector3 relativePos = Camera.main.transform.position - transform.position;

    Quaternion rotation = Quaternion.LookRotation(relativePos);

    rotation = rotation * Quaternion.Euler(duh, 0, 0);
    transform.rotation = rotation;

    yield return new WaitForSeconds(.1f);
    StartCoroutine(Dupdate(duh + 10));
  }

}
