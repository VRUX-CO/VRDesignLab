using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunWithPath : MonoBehaviour
{
  public TrackPiece[] trackPieces;
  List<Transform> points = new List<Transform>();

  int NowPathID = 0;
  public float Speed = 3F;
  public Transform[] Wheels;
  public float TurnPerPoint = 1F;
  float RunProgress = 0F;
  Vector3 LastPos;
  Vector3 NowPos;
  float RotX;
  float RotY;
  bool isEnd = false;
  Vector3 LastRot;
  float NowSpeed = 0F;

    public int StartPathId = 0;

    private AttachCamera attachCamera;
    private Vector3 startPosition;
    private Quaternion startRotation;

    public static event Action PathStarted;

    public static event Action PathEnded;

    protected virtual void Awake()
    {
        attachCamera = GetComponent<AttachCamera>();

        foreach (TrackPiece piece in trackPieces)
        {
            points.AddRange(piece.trackPoints());
        }

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    protected virtual void OnEnable()
    {
        NowPathID = StartPathId;
        GetPost(NowPathID);
        NowSpeed = Speed * 0.2F;

        if (PathStarted != null)
        {
            PathStarted.Invoke();
        }
    }

    protected virtual void OnDisable()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;

        if (attachCamera != null)
        {
            attachCamera.enabled = false;
        }

        isEnd = false;

        if (PathEnded != null)
        {
            PathEnded.Invoke();
        }
    }

  // Update is called once per frame
  void Update()
  {
    if (isEnd)
    {
      return;
    }

    if (NowPathID < points.Count / 3F)
    {
      NowSpeed = Mathf.Lerp(Speed * 0.2F, Speed, Mathf.Clamp01(NowPathID / (points.Count / 3F)));
    }
    if (NowPathID > points.Count / 3F * 2F)
    {
      NowSpeed = Mathf.Lerp(Speed, 0F, Mathf.Clamp01((NowPathID - (points.Count / 3F * 2F)) / (points.Count / 3F)));
    }

    transform.position = Vector3.Lerp(LastPos, NowPos, RunProgress);
    transform.eulerAngles = new Vector3(Mathf.Lerp(LastRot.x, RotX, RunProgress), Mathf.Lerp(LastRot.y, RotY, RunProgress), 0F);
    if (RunProgress < 1F)
    {
      RunProgress += Time.deltaTime * NowSpeed;
    }
    else
    {
      NowPathID += 1;
      GetPost(NowPathID);
    }
    for (int i = 0; i < Wheels.Length; i++)
    {
      Wheels[i].eulerAngles = new Vector3(Wheels[i].eulerAngles.x + Time.deltaTime * NowSpeed * 360F * TurnPerPoint, Wheels[i].eulerAngles.y, Wheels[i].eulerAngles.z);
    }

  }

  private float FormAngles(float OriAngle, float NowAngle)
  {
    float result = OriAngle % 360F;
    if (result - NowAngle > 180F)
    {
      result -= 360F;
    }
    if (NowAngle - result > 180F)
    {
      result += 360F;
    }
    return result;
  }


  void GetPost(int _NowPathID)
  {
    RunProgress = 0F;
    if (_NowPathID <= 0)
    {
      _NowPathID = 0;
    }
    if (_NowPathID > points.Count - 2)
    {
      isEnd = true;
      Debug.Log("End");
      _NowPathID = points.Count - 2;
        enabled = false;
    }
    else
    {
      LastPos = points[_NowPathID].position;
      NowPos = points[_NowPathID + 1].position;
      LastRot = transform.eulerAngles;
      Vector3 _dis = NowPos - LastPos;
      Quaternion _rot = Quaternion.LookRotation(_dis);
      RotX = FormAngles(_rot.eulerAngles.x, transform.eulerAngles.x);
      RotY = FormAngles(_rot.eulerAngles.y, transform.eulerAngles.y);
    }
  }
}
