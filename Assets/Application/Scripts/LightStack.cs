using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightStack : MonoBehaviour
{
    private static List<Light> lights = new List<Light>();

    private new Light light;

    protected virtual void Awake()
    {
        light = GetComponent(typeof(Light)) as Light;
    }

    protected virtual void OnDestroy()
    {
        lights.Remove(light);
        RefreshStack();
    }

    protected virtual void Start()
    {
        lights.Add(light);
        RefreshStack();
    }

    private static void RefreshStack()
    {
        for (int i = 0; i < lights.Count; ++i)
        {
            lights[i].enabled = (i == lights.Count - 1);
        }
    }
}