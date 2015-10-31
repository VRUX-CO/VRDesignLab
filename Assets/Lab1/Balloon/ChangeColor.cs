using UnityEngine;
using System.Collections;

public class ChangeColor : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();

        // pick a random color
        Color newColor = new Color(Random.value, Random.value, Random.value, .9f);
        // apply it on current object's material
        rend.material.color = newColor;

        //rend.material.color = new Color(1, 0, 0, .5f);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
