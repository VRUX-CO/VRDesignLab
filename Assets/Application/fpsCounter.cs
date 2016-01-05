using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fpsCounter : MonoBehaviour {

    public int FPS { get; private set; }
    public Text fpsDisplay;

    void Update()
    {
        FPS = (int)(1f / Time.deltaTime);
        fpsDisplay.text = FPS.ToString();
        fpsDisplay.text = Mathf.Clamp(FPS, 0, 99).ToString();
    }

}
