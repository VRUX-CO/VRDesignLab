using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignWithRing : MonoBehaviour
{
  Material signMaterial;
  Material ringMaterial;
  TextureBillboard sign;

  GameObject ringObject;
  GameObject lineObject;
  GameObject sphereObject;
  GameObject signObject;
  bool visible = false;

  public static SignWithRing Make(float radius, Material signMat, Texture signTexture, Material ringMat, float scale, bool hasRing)
  {
    GameObject gob = new GameObject("Sign");

    SignWithRing result = gob.AddComponent<SignWithRing>();

    // deep clone the material
    signMat = Instantiate(signMat) as Material;

    signMat.mainTexture = signTexture;

    result.Setup(radius, signMat, ringMat, scale, hasRing);

    return result;
  }

  public void Setup(float radius, Material signMat, Material ringMat, float scale, bool hasRing)
  {
    signMaterial = signMat;
    ringMaterial = ringMat;

    signObject = CreateSign(radius, signMat, scale);
    signObject.transform.parent = transform;

    // first sign doesn't need distance ring
    if (hasRing)
    {
      lineObject = CreateLine(radius, signObject);
      lineObject.transform.parent = transform;

      ringObject = CreateRing(radius);
      ringObject.transform.parent = transform;

      sphereObject = CreateSphere(radius);
      sphereObject.transform.parent = transform;
    }
  }

  GameObject CreateRing(float radius)
  {
    float innerRadius = .01f;
    int nbRadSeg = 80;
    int nbSides = 20;

    GameObject result = new GameObject();

    MeshUtilities.AddTorusMeshFilter(result, radius, innerRadius, nbRadSeg, nbSides);

    MeshRenderer renderer = result.AddComponent<MeshRenderer>();
    renderer.enabled = false;  // start off hidden

    renderer.material = ringMaterial;

    return result;
  }

  GameObject CreateLine(float radius, GameObject sign)
  {
    Bounds signBounds = sign.GetComponentInChildren<Renderer>().bounds;
    float signHeight = signBounds.size.y * .40f;  // image is centered in bounds and bottom half is blank, this is an adustment
    float lineLength = signBounds.center.y - (signHeight / 2);

    const float minHeight = .2f;

    if (lineLength < minHeight)
    {
      Vector3 newPos = sign.transform.localPosition;
      newPos.y = (signHeight / 2) + minHeight;

      sign.transform.position = newPos;

      lineLength = minHeight;
    }

    GameObject result = GameObject.CreatePrimitive(PrimitiveType.Quad);

    MeshRenderer renderer = result.GetComponent<MeshRenderer>();
    renderer.enabled = false;  // start off hidden

    renderer.material = ringMaterial;

    result.transform.localPosition = new Vector3(0, lineLength / 2f, radius);
    result.transform.localScale = new Vector3(.01f, lineLength, 1);

    return result;
  }

  GameObject CreateSphere(float radius)
  {
    GameObject result = GameObject.CreatePrimitive(PrimitiveType.Sphere);

    MeshRenderer renderer = result.GetComponent<MeshRenderer>();
    renderer.enabled = false;  // start off hidden

    renderer.material = ringMaterial;

    result.transform.localPosition = new Vector3(0, 0, radius);
    result.transform.localScale = new Vector3(.05f, .05f, .05f);

    return result;
  }

  GameObject CreateSign(float radius, Material signMat, float scale)
  {
    GameObject result = null;

    Vector3 newPosition = new Vector3(0, 1.2f, radius);
    sign = TextureBillboard.Billboard(signMat, new Vector3(1, 1, 1), 1, newPosition, transform, false);
    result = sign.gameObject;

    result.transform.localScale = new Vector3(scale, scale, scale);

    return result;
  }

  public void Show(bool show)
  {
    // already in sync
    if (visible == show)
      return;

    visible = show;

    if (show)
    {
      sign.Show(0);

      if (ringObject != null)
      {
        ringObject.GetComponent<Renderer>().enabled = true;
        lineObject.GetComponent<Renderer>().enabled = true;
        sphereObject.GetComponent<Renderer>().enabled = true;
      }
    }
    else
    {
      sign.Hide(.25f);

      if (ringObject != null)
      {
        ringObject.GetComponent<Renderer>().enabled = false;
        lineObject.GetComponent<Renderer>().enabled = false;
        sphereObject.GetComponent<Renderer>().enabled = false;
      }
    }
  }
}
