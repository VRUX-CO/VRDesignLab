/************************************************************************************

Filename    :   ToggleColor.cs
Content     :   An example of a saving the state of objects when exiting the app.
Created     :   September 16, 2014
Authors     :   Jim Dosé

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Collections;

public class ToggleColor : MonoBehaviour
{
	public enum Colors
	{
		Red,
		Green,
		Blue
	};

	public Colors ObjectColor = Colors.Red;

	public void RestoreState()
	{
		ObjectColor = (Colors)PlayerPrefs.GetInt(name + "_ObjectColor");
		GetComponent<Renderer>().sharedMaterial = SaveStateSample.instance.Materials[(int)ObjectColor];
	}

	public void SaveState()
	{
		PlayerPrefs.SetInt(name + "_ObjectColor", (int)ObjectColor);
	}

 	public void OnClick()
	{
		int color = (int)ObjectColor;
		color++;
		if (color > (int)Colors.Blue)
		{
			color = (int)Colors.Red;
		}
		
		ObjectColor = (Colors)color;
		GetComponent<Renderer>().sharedMaterial = SaveStateSample.instance.Materials[(int)ObjectColor];
	}
}
