using UnityEngine;
using System.Collections;

public class ButtonRevealPlane : MonoBehaviour
{
  Camera m_Camera;
  LookButton[] buttons;

  // called only from LookButton when it's clicked
  public void OnLookButtonClick(string buttonID)
  {
    Debug.Log(buttonID);
    ShowButtons(false);

  }

  // Use this for initialization
  void Start()
  {
    buttons = gameObject.GetComponentsInChildren<LookButton>();

    // clicks from buttons are sent from the button to OnLookButtonClick()
    foreach (LookButton button in buttons)
      button.revealPlane = this;

    MeshUtilities.AddMeshComponent(gameObject, 1f, 2.5f);

    // m_Camera = Camera.main;

    // StartCoroutine(Dupdate(0));
  }

  // Update is called once per frame
  void Update()
  {
    MoveInfrontOfCamera();

    //   transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
    //         Vector3.forward);
  }

  void MoveInfrontOfCamera()
  {
    Vector3 pos = Camera.main.transform.position;
    pos.z += .5f;
    pos.y = .1f;

    transform.position = pos;
  }

  IEnumerator Dupdate(int duh)
  {
    Vector3 relativePos = Camera.main.transform.position - transform.position;

    Quaternion rotation = Quaternion.LookRotation(relativePos);

    rotation = rotation * Quaternion.Euler(duh, 0, 0);
    transform.rotation = rotation;

    yield return new WaitForSeconds(.1f);
    StartCoroutine(Dupdate(duh + 10));
  }

  public void OnRevealStart()
  {
    ShowButtons(true);
  }

  public void OnRevealEnd()
  {
    ShowButtons(false);
  }

  public void ShowButtons(bool show)
  {
    for (int i = 0; i < buttons.Length; i++)
    {
      buttons[i].FadeIn(show);
    }
  }
}
