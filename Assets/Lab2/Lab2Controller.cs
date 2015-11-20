using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lab2Controller : MonoBehaviour
{
  public Material signMaterial;
  public Texture introSignTexture;
  public Texture HalfMeterTexture;
  public Texture OneMeterTexture;
  public Texture OneHalfMeterTexture;
  public Texture ThreeMeterTexture;
  public Texture SixMeterTexture;
  public Texture TwelveMeterTexture;

  public Material goodMaterial;
  public Material warningMaterial;

  GameObject signAnchor_lazy;
  List<SignWithRing> signs_private;

  const int numSigns = 7;
  int index = 0;

  void Start()
  {
    Next();
  }

  // Update is called once per frame
  void Update()
  {
    if (Utilities.UserClicked())
    {
      Next();
    }
  }

  void LateUpdate()
  {
    MoveAnchorProperDistanceFromCamera();
  }

  void HideAllSigns()
  {
    for (int i = 0; i < numSigns; i++)
    {
      SignWithRing sign = SignAtIndex(i);

      sign.Show(false);
    }
  }

  void Next()
  {
    HideAllSigns();

    SignWithRing sign = SignAtIndex(index);

    if (sign != null)
    {
      sign.Show(true);

      // update distance loop
      float degress = DegressForIndex(index % numSigns);
      iTween.RotateTo(GetSignAnchor(), new Vector3(0f, -degress, 0f), 1f);

      index++;
    }
    else // failed, so show them all
    {
      index = 0;
    }
  }

  SignWithRing SignAtIndex(int index)
  {
    if (signs_private == null)
    {
      signs_private = new List<SignWithRing>();

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

        SignWithRing sign = SignWithRing.Make(radius, signMaterial, SignTextureForIndex(i), ringMat, ScaleForIndex(i), i != 0);
        signs_private.Add(sign);

        result = sign.gameObject;

        result.transform.Rotate(new Vector3(0, DegressForIndex(i), 0f));

        result.transform.parent = GetSignAnchor().transform;
      }
    }

    if (index >= 0 && index < signs_private.Count)
    {
      return signs_private[index];
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

  float DegressForIndex(int index)
  {
    return ((float)index / (float)numSigns) * 360f;
  }

  //    iTween.RotateBy(sign, new Vector3(0f, degress, 0f), 1f);
  Texture SignTextureForIndex(int index)
  {
    Texture result;

    switch (index)
    {
      default:
      case 0:
        result = introSignTexture;
        break;
      case 1:
        result = HalfMeterTexture;
        break;
      case 2:
        result = OneMeterTexture;
        break;
      case 3:
        result = OneHalfMeterTexture;
        break;
      case 4:
        result = ThreeMeterTexture;
        break;
      case 5:
        result = SixMeterTexture;
        break;
      case 6:
        result = TwelveMeterTexture;
        break;
    }

    return result;
  }

  float ScaleForIndex(int index)
  {
    float scale = 1;

    switch (index)
    {
      default:
      case 0:
        break;
      case 1:
        scale = .5f;
        break;
      case 2:
        break;
      case 3:
        scale = 1.5f;
        break;
      case 4:
        scale = 2f;
        break;
      case 5:
        scale = 3f;
        break;
      case 6:
        scale = 6f;
        break;
    }

    return scale;
  }

  float RadiusForIndex(int index)
  {
    float result = -1f;

    if (index < numSigns)
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

  void MoveAnchorProperDistanceFromCamera()
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
