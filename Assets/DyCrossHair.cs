using UnityEngine;
using System.Collections;

public class DyCrossHair : MonoBehaviour
{


    // Use this for initialization

    public enum preset { none, shotgunPreset, crysisPreset }
    public preset crosshairPreset = preset.none;

    public bool showCrosshair = true;
    public Texture2D verticalTexture;
    public Texture2D horizontalTexture;

    //Size of boxes
    public float cLength = 10.0f;
    public float cWidth = 3.0f;

    //Spreed setup
    public float minSpread = 45.0f;
    public float maxSpread = 250.0f;
    public float spreadPerSecond = 150.0f;

    //Rotation
    public float rotAngle = 0.0f;
    public float rotSpeed = 0.0f;

    [HideInInspector]
    public Texture2D temp;
    [HideInInspector]
    public float spread;

    void Start()
    {
        crosshairPreset = preset.none;
    }

    void Update()
    {
        //Used just for test (weapon script should change spread).
        if (Input.GetKey(KeyCode.K)) spread += spreadPerSecond * Time.deltaTime;
        else spread -= spreadPerSecond * 2 * Time.deltaTime;

        //Rotation
        rotAngle += rotSpeed * Time.deltaTime;
    }

    void OnGUI()
    {
        if (showCrosshair && verticalTexture && horizontalTexture)
        {
            GUIStyle verticalT = new GUIStyle();
            GUIStyle horizontalT = new GUIStyle();
            verticalT.normal.background = verticalTexture;
            horizontalT.normal.background = horizontalTexture;
            spread = Mathf.Clamp(spread, minSpread, maxSpread);
            Vector2 pivot = new Vector2(Screen.width / 2, Screen.height / 2);

            if (crosshairPreset == preset.crysisPreset)
            {

                GUI.Box(new Rect((Screen.width - 2) / 2, (Screen.height - spread) / 2 - 14, 2, 14), temp, horizontalT);
                GUIUtility.RotateAroundPivot(45, pivot);
                GUI.Box(new Rect((Screen.width + spread) / 2, (Screen.height - 2) / 2, 14, 2), temp, verticalT);
                GUIUtility.RotateAroundPivot(0, pivot);
                GUI.Box(new Rect((Screen.width - 2) / 2, (Screen.height + spread) / 2, 2, 14), temp, horizontalT);
            }

            if (crosshairPreset == preset.shotgunPreset)
            {

                GUIUtility.RotateAroundPivot(45, pivot);

                //Horizontal
                GUI.Box(new Rect((Screen.width - 14) / 2, (Screen.height - spread) / 2 - 3, 14, 3), temp, horizontalT);
                GUI.Box(new Rect((Screen.width - 14) / 2, (Screen.height + spread) / 2, 14, 3), temp, horizontalT);
                //Vertical
                GUI.Box(new Rect((Screen.width - spread) / 2 - 3, (Screen.height - 14) / 2, 3, 14), temp, verticalT);
                GUI.Box(new Rect((Screen.width + spread) / 2, (Screen.height - 14) / 2, 3, 14), temp, verticalT);
            }

            if (crosshairPreset == preset.none)
            {

                GUIUtility.RotateAroundPivot(rotAngle % 360, pivot);

                //Horizontal
                GUI.Box(new Rect((Screen.width - cWidth) / 2, (Screen.height - spread) / 2 - cLength, cWidth, cLength), temp, horizontalT);
                GUI.Box(new Rect((Screen.width - cWidth) / 2, (Screen.height + spread) / 2, cWidth, cLength), temp, horizontalT);
                //Vertical
                GUI.Box(new Rect((Screen.width - spread) / 2 - cLength, (Screen.height - cWidth) / 2, cLength, cWidth), temp, verticalT);
                GUI.Box(new Rect((Screen.width + spread) / 2, (Screen.height - cWidth) / 2, cLength, cWidth), temp, verticalT);
            }
        }
    }
}