using UnityEngine;
using System.Collections;

public class DistanceSign : MonoBehaviour
{
  public GameObject HalfMeterMessage;
  public GameObject OneMeterMessage;
  public GameObject OneHalfMeterMessage;
  public GameObject ThreeMeterMessage;
  public GameObject SixMeterMessage;
  public GameObject TwelveMeterMessage;
  public Material goodMaterial;
  public Material warningMaterial;

  GameObject ringObject;
  GameObject lineObject;
  GameObject sphereObject;
  GameObject signObject;
  GameObject mainObject;

  GameObject CreateRing(float radius)
  {
    float innerRadius = .01f;
    int nbRadSeg = 44;
    int nbSides = 18;

    GameObject result = new GameObject();

    MeshUtilities.AddTorusMeshFilter(result, radius, innerRadius, nbRadSeg, nbSides);

    MeshRenderer renderer = result.AddComponent<MeshRenderer>();

    renderer.material = goodMaterial;
    if (radius < 1f || radius > 5)
    {
      renderer.material = warningMaterial;
    }

    return result;
  }

  public void Show(int index, bool deletePrevious)
  {
    if (deletePrevious)
    {
      // deleting parent deletes children
      if (mainObject != null)
        Destroy(mainObject);
    }

    if (index < 6)
    {
      mainObject = new GameObject("Sign");

      // parent it all together so we can move it to stay z distance from camera, and so it gets deleted properly
      mainObject.transform.parent = transform;

      float radius = RadiusForIndex(index);

      ringObject = CreateRing(radius);
      ringObject.transform.parent = mainObject.transform;

      signObject = CreateSign(index, radius);
      signObject.transform.parent = mainObject.transform;

      lineObject = CreateLine(radius, signObject);
      lineObject.transform.parent = mainObject.transform;

      sphereObject = CreateSphere(radius);
      sphereObject.transform.parent = mainObject.transform;

      mainObject.transform.Rotate(new Vector3(0, .25f * 360, 0f));
      iTween.RotateBy(mainObject, new Vector3(0f, -.25f, 0f), 3f);
    }
  }

  GameObject CreateLine(float radius, GameObject sign)
  {
    Bounds signBounds = sign.GetComponent<Renderer>().bounds;
    float lineLength = signBounds.min.y;

    if (lineLength < .2f)
    {
      Vector3 newPos = sign.transform.localPosition;
      newPos.y = signBounds.extents.y + .2f;

      sign.transform.position = newPos;

      lineLength = .2f;
      Debug.Log("duh: " + lineLength.ToString());
    }

    GameObject result = GameObject.CreatePrimitive(PrimitiveType.Quad);

    MeshRenderer renderer = result.GetComponent<MeshRenderer>();

    renderer.material = goodMaterial;
    if (radius < 1f || radius > 5)
    {
      renderer.material = warningMaterial;
    }

    result.transform.localPosition = new Vector3(0, lineLength / 2f, radius);
    result.transform.localScale = new Vector3(.01f, lineLength, 1);

    return result;
  }

  GameObject CreateSphere(float radius)
  {
    GameObject result = GameObject.CreatePrimitive(PrimitiveType.Sphere);

    MeshRenderer renderer = result.GetComponent<MeshRenderer>();

    renderer.material = goodMaterial;
    if (radius < 1f || radius >= 5f)
    {
      renderer.material = warningMaterial;
    }

    result.transform.localPosition = new Vector3(0, 0, radius);
    result.transform.localScale = new Vector3(.05f, .05f, .05f);

    return result;
  }

  GameObject CreateSign(int index, float radius)
  {
    GameObject result = null;
    GameObject signPrefab;
    Vector3 scale = new Vector3(1f, 1f, 1f);
    float verticalPosition = 1f;

    switch (index)
    {
      case 0:
        signPrefab = HalfMeterMessage;
        scale = new Vector3(1f, 1f, 1f);
        verticalPosition = 1f;
        break;
      case 1:
        signPrefab = OneMeterMessage;
        verticalPosition = 1f;
        break;
      case 2:
        signPrefab = OneHalfMeterMessage;
        scale = new Vector3(1.5f, 1.5f, 1.5f);
        verticalPosition = 1f;
        break;
      case 3:
        signPrefab = ThreeMeterMessage;
        scale = new Vector3(2f, 2f, 2f);
        verticalPosition = 1f;
        break;
      case 4:
        signPrefab = SixMeterMessage;
        scale = new Vector3(3f, 3f, 3f);
        verticalPosition = 1f;
        break;
      case 5:
      default:
        signPrefab = TwelveMeterMessage;
        scale = new Vector3(4f, 4f, 4f);
        verticalPosition = 1f;
        break;
    }

    Vector3 newPosition = new Vector3(0, -22f, radius);
    result = Instantiate(signPrefab, newPosition, Quaternion.identity) as GameObject;

    // parent this so it gets deleted when scene is swapped out
    result.transform.parent = transform;

    result.transform.localPosition = new Vector3(0, verticalPosition, radius);

    result.transform.localScale = scale;


    return result;
  }

  float RadiusForIndex(int index)
  {
    float result = -1f;

    if (index < 6)
    {
      switch (index)
      {
        case 0:
          result = .5f;
          break;
        case 1:
          result = 1f;
          break;
        case 2:
          result = 1.5f;
          break;
        case 3:
          result = 3f;
          break;
        case 4:
          result = 6f;
          break;
        case 5:
        default:
          result = 12f;
          break;
      }
    }

    return result;
  }

  void Update()
  {
    // maintain distance to camera without attaching
    if (mainObject != null)
    {
      Vector3 newPosition = new Vector3(0, 0, Camera.main.transform.position.z);

      mainObject.transform.localPosition = newPosition;
    }
  }
}
