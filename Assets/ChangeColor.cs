using UnityEngine;
using System.Collections;

public class ChangeColor : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = new Color(1, 0, 0, .5f);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
