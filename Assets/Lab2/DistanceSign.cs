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
  List<SignWithRing> signs;

  const int numSigns = 7;

  SignWithRing SignAtIndex(int index)
  {
    if (signs == null)
    {
      signs = new List<SignWithRing>();

      for (int i = 0; i < numSigns; i++)
      {
        GameObject result = null;

        // parent it all together so we can move it to stay z distance from camera, and so it gets deleted properly
        float radius = RadiusForIndex(i);

        Material ringMat = goodMaterial;
        if (radius < 1f || radius > 5)
        {
          ringMat = warningMaterial;
        }

        SignWithRing sign = new SignWithRing();
        signs.Add(sign);

        result = sign.Make(radius, SignMatForIndex(i), ringMat, ScaleForIndex(i), i != 0);
        result.transform.Rotate(new Vector3(0, DegressForIndex(i), 0f));

        result.transform.parent = GetSignAnchor().transform;
      }
    }

    if (index >= 0 && index < signs.Count)
    {
      return signs[index];
    }

    return null;
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

  void DeleteAllSigns()
  {
    foreach (SignWithRing sign in signs)
    {
      Destroy(sign.gameObject);
    }

    signs.Clear();
  }

  public bool Show(int index)
  {
    float degress = DegressForIndex(index % numSigns);
    iTween.RotateTo(GetSignAnchor(), new Vector3(0f, -degress, 0f), 1f);

    SignAtIndex(index).Show(true);

    return true;
  }

  float DegressForIndex(int index)
  {
    return ((float)index / (float)numSigns) * 360f;
  }

  //    iTween.RotateBy(sign, new Vector3(0f, degress, 0f), 1f);
  Material SignMatForIndex(int index)
  {
    Material signMat;

    switch (index)
    {
      default:
      case 0:
        signMat = introSignMaterial;
        break;
      case 1:
        signMat = HalfMeterMaterial;
        break;
      case 2:
        signMat = OneMeterMaterial;
        break;
      case 3:
        signMat = OneHalfMeterMaterial;
        break;
      case 4:
        signMat = ThreeMeterMaterial;
        break;
      case 5:
        signMat = SixMeterMaterial;
        break;
      case 6:
        signMat = TwelveMeterMaterial;
        break;
    }

    return signMat;
  }

  Vector3 ScaleForIndex(int index)
  {
    Vector3 scale = new Vector3(1f, 1f, 1f);

    switch (index)
    {
      default:
      case 0:
        break;
      case 1:
        scale = new Vector3(.5f, .5f, .5f);
        break;
      case 2:
        break;
      case 3:
        scale = new Vector3(1.5f, 1.5f, 1.5f);
        break;
      case 4:
        scale = new Vector3(2f, 2f, 2f);
        break;
      case 5:
        scale = new Vector3(3f, 3f, 3f);
        break;
      case 6:
        scale = new Vector3(4f, 4f, 4f);
        break;
    }

    return scale;
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
