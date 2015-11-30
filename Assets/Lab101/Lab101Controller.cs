using UnityEngine;
using System.Collections;

public class Lab101Controller : MonoBehaviour
{
  public GameObject waypointTarget;

  // Use this for initialization
  void Start()
  {
    AppCentral.APP.ShowEnvironment(EnvironmentEnum.kForest);

    CreateClickTarget("c", new Vector3(0, 2, 1));
    CreateClickTarget("cd", new Vector3(-3, 2, 1));
    CreateClickTarget("ce", new Vector3(3, 2, 1));

  }

  void TargetClicked(GameObject clickedObject)
  {
    Debug.Log(clickedObject.transform.position.ToString());

    AppCentral.APP.MoveCamera(clickedObject.transform.position);
  }

  void CreateClickTarget(string targetID, Vector3 position)
  {
    GameObject target = Instantiate(waypointTarget, position, Quaternion.identity) as GameObject;
    ClickTarget clickTarget = target.GetComponent<ClickTarget>();

    clickTarget.SetClickDelegate(gameObject, "TargetClicked", targetID);

    target.transform.parent = transform;
  }
}

