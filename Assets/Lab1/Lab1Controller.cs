using UnityEngine;
using System.Collections;

public class Lab1Controller : MonoBehaviour
{
  public GameObject balloonSpawnerPrefab;

  public Material Card1Mat;
  public Material Card2Mat;

  GameObject currentCard = null;

  // Use this for initialization
  void Start()
  {
    GameObject gobj = Instantiate(balloonSpawnerPrefab);

    BalloonSpawner spawner = gobj.GetComponent<BalloonSpawner>();
    spawner.controller = this;

    ShowCard(0);
  }

  // Update is called once per frame
  public void PoppedBalloon()
  {
    Destroy(currentCard);
    ShowCard(1);
  }

  void ShowCard(int cardIndex)
  {
    Vector3 endPosition = new Vector3(0, 1, 2);

    currentCard = GameObject.CreatePrimitive(PrimitiveType.Quad);
    currentCard.AddComponent<FaceCameraScript>();
    MeshRenderer renderer = currentCard.GetComponent<MeshRenderer>();

    switch (cardIndex)
    {
      case 0:
        renderer.material = Card1Mat;
        endPosition = new Vector3(0, 1.5f, 2);
        break;
      default:
      case 1:
        renderer.material = Card2Mat;
        endPosition = new Vector3(-1, 1.5f, 2);
        break;
    }

    // parent this to gameObject so it gets cleaned up when level is unloaded
    currentCard.transform.parent = gameObject.transform;

    currentCard.transform.position = new Vector3(0, 20, 2);

    iTween.MoveTo(currentCard, iTween.Hash("position", endPosition, "time", 2f, "easeType", iTween.EaseType.linear, "oncomplete", "OnCompleteCallback", "oncompleteparams", currentCard));
  }

  void OnCompleteCallback(GameObject card)
  {
    // Destroy(card);
  }



}
