/************************************************************************************

Filename    :   SaveStateSample.cs
Content     :   An example of a saving the state of objects when exiting the app.
Created     :   September 16, 2014
Authors     :   Jim Dosé

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Collections;

public class SaveStateSample : MonoBehaviour {
	public static SaveStateSample instance = null;

	public Material[] Materials;

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		RestoreState();
	}

	void OnApplicationQuit() {
		SaveState();
	}

	void OnApplicationPause( bool pauseState ) {
		if ( pauseState == true )
		{
			SaveState();
		}
	}

	void SaveState()
	{
		Debug.Log( "Saving state..." );
		ToggleColor[] objects = FindObjectsOfType( typeof( ToggleColor ) ) as ToggleColor[];
		foreach( ToggleColor obj in objects )
		{
			obj.SaveState();
		}
		
		PlayerPrefs.SetInt( "HasSaveState", 1 );
		PlayerPrefs.Save();
	}

	void RestoreState()
	{
		if ( PlayerPrefs.GetInt( "HasSaveState" ) != 0 )
		{
			Debug.Log( "Restoring state..." );
			ToggleColor[] objects = FindObjectsOfType( typeof( ToggleColor ) ) as ToggleColor[];
			foreach( ToggleColor obj in objects )
			{
				obj.RestoreState();
			}
		}
		else
		{
			Debug.Log( "No saved state." );
		}
	}
}
