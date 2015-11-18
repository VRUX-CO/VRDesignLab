using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DistanceSign : MonoBehaviour
{
  public Material introSignMaterial;
  public Material HalfMeterMaterial;
  public Material OneMeterMaterial;
  public Material OneHalfMeterMaterial;
  public Material ThreeMeterMaterial;
  public Material SixMeterMaterial;
  public Material TwelveMeterMaterial;

  public Material goodMaterial;
  public Material warningMaterial;


  GameObject signAnchor_lazy;
  List<GameObject> signs = new List<GameObject>();

  const int numSigns = 7;

  void Start()
  {
    MakeSigns();
  }

  GameObject GetSignAnchor()
  {
    if (signAnchor_lazy == null)
    {
      signAnchor_lazy = new GameObject("Anchor");
      signAnchor_lazy.transform.parent = transform;
    }

    return signAnchor_lazy;
  }

  GameObject CreateRing(float radius)
  {
    float innerRadius = .01f;
    int nbRadSeg = 44;
    int nbSides = 88;

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

  void DeleteAllSigns()
  {
    foreach (GameObject obj in signs)
    {
      Destroy(obj);
    }

    signs.Clear();
  }

  public bool Show(int index)
  {
    float degress = DegressForIndex(index % numSigns);
    iTween.RotateTo(GetSignAnchor(), new Vector3(0f, -degress, 0f), 1f);

    return true;
  }

  float DegressForIndex(int index)
  {
    return ((float)index / (float)numSigns) * 360f;
  }

  //    iTween.RotateBy(sign, new Vector3(0f, degress, 0f), 1f);

  public void MakeSigns()
  {
    for (int i = 0; i < numSigns; i++)
    {
      GameObject sign = MakeSignAtIndex(i);

      sign.transform.Rotate(new Vector3(0, DegressForIndex(i), 0f));
    }
  }

  GameObject MakeSignAtIndex(int index)
  {
    GameObject result = null;

    if (index >= 0 && index < numSigns)
    {
      result = new GameObject("Sign");

      // parent it all together so we can move it to stay z distance from camera, and so it gets deleted properly
      result.transform.parent = GetSignAnchor().transform;

      float radius = RadiusForIndex(index);

      GameObject ringObject;
      GameObject lineObject;
      GameObject sphereObject;
      GameObject signObject;

      signObject = CreateSign(index, radius);
      signObject.transform.parent = result.transform;

      // first sign doesn't need distance ring
      if (index > 0)
      {
        lineObject = CreateLine(radius, signObject);
        lineObject.transform.parent = result.transform;

        ringObject = CreateRing(radius);
        ringObject.transform.parent = result.transform;

        sphereObject = CreateSphere(radius);
        sphereObject.transform.parent = result.transform;
      }

      signs.Add(result);
    }

    return result;
  }

  GameObject CreateLine(float radius, GameObject sign)
  {
    Bounds signBounds = sign.GetComponentInChildren<Renderer>().bounds;
    float lineLength = signBounds.min.y;
    const float minHeight = .2f;

    if (lineLength < minHeight)
    {
      Vector3 newPos = sign.transform.localPosition;
      newPos.y = signBounds.extents.y + minHeight;

      sign.transform.position = newPos;

      lineLength = minHeight;
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
    Material signMat;
    Vector3 scale = new Vector3(1f, 1f, 1f);

    switch (index)
    {
      default:
      case 0:
        signMat = introSignMaterial;
        scale = new Vector3(1f, 1f, 1f);
        break;
      case 1:
        signMat = HalfMeterMaterial;
        scale = new Vector3(.5f, .5f, .5f);
        break;
      case 2:
        signMat = OneMeterMaterial;
        break;
      case 3:
        signMat = OneHalfMeterMaterial;
        scale = new Vector3(1.5f, 1.5f, 1.5f);
        break;
      case 4:
        signMat = ThreeMeterMaterial;
        scale = new Vector3(2f, 2f, 2f);
        break;
      case 5:
        signMat = SixMeterMaterial;
        scale = new Vector3(3f, 3f, 3f);
        break;
      case 6:
        signMat = TwelveMeterMaterial;
        scale = new Vector3(4f, 4f, 4f);
        break;
    }

    Vector3 newPosition = new Vector3(0, 1.2f, radius);
    TextureBillboard tbb = TextureBillboard.ShowBillboard(signMat, new Vector3(1, 1, 1), 1, newPosition, transform);
    result = tbb.gameObject;

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
        default:
        case 0:
          result = 1.5f;  // intro sign
          break;
        case 1:
          result = .5f;
          break;
        case 2:
          result = 1f;
          break;
        case 3:
          result = 1.5f;
          break;
        case 4:
          result = 3f;
          break;
        case 5:
          result = 6f;
          break;
        case 6:
          result = 12f;
          break;
      }
    }

    return result;
  }

  void Update()
  {
    GameObject anchor = GetSignAnchor();

    // maintain distance to camera without attaching
    if (anchor != null)
    {
      Vector3 newPosition = new Vector3(0, 0, Camera.main.transform.position.z);

      anchor.transform.localPosition = newPosition;
    }
  }
}
