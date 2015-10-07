/************************************************************************************

Filename    :   MultiCameraSample.cs
Content     :   An example of how to set and use multiple camera controllers
Created     :   July 24, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;

public class MultiCameraSample : MonoBehaviour {

	public OVRCameraRig[]	cameraControllers = new OVRCameraRig[0];
	public int						currentController = 0;

	/// <summary>
	/// Initialize
	/// </summary>
	void Start() {
		UpdateCameraControllers();
	}
	
	/// <summary>
	/// Set the current camera controller
	/// </summary>
	void UpdateCameraControllers() {
		for ( int i = 0; i < cameraControllers.Length; i++ ) {
			if ( cameraControllers[i] == null ) {
				continue;
			}
			cameraControllers[i].gameObject.SetActive( i == currentController );
		}
	}

	/// <summary>
	/// Check input and switch between camera controllers
	/// These samples use the default Unity Input Mappings with the addition of "Right Shoulder" and "Left Shoulder"
	/// </summary>
	void Update() {
		if ( Input.GetButtonDown( "Right Shoulder" ) ) {
			//*************************
			// switch to the next camera
			//*************************
			if ( ++currentController == cameraControllers.Length ) {
				currentController = 0;
			}
			UpdateCameraControllers();
		} else if ( Input.GetButtonDown( "Left Shoulder" ) ) {
			//*************************
			// switch to the previous camera
			//*************************
			if ( --currentController < 0 ) {
				currentController = cameraControllers.Length - 1;
			}
			UpdateCameraControllers();
		}
	}
}
