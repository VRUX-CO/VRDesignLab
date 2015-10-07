/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/
using UnityEngine;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
	public float			delayBeforeLoad = 0.1f;
	public string			sceneToLoad = string.Empty;
	public OVRGamepadController.Button resetButton = OVRGamepadController.Button.X;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		// NOTE: some of the buttons defined in OVRGamepadController.Button are not available on the Android game pad controller
		if (Input.GetButtonDown(OVRGamepadController.ButtonNames[(int)resetButton]))
		{
			StartCoroutine(DelayedSceneLoad());
		}
	}

	/// <summary>
	/// Asynchronously start the main scene load
	/// </summary>
	IEnumerator DelayedSceneLoad()
	{
		// delay one frame to make sure everything has initialized
		yield return 0;

		yield return new WaitForSeconds(delayBeforeLoad);
		
		Debug.Log( "[LoadLevel] " + sceneToLoad + " Time: " + Time.time );
		
		float startTime = Time.realtimeSinceStartup;
#if !UNITY_PRO_LICENSE
		Application.LoadLevel(sceneToLoad);
#else
		//*************************
		// load the scene asynchronously.
		// this will allow the player to 
		// continue looking around in your scene
		//*************************
		AsyncOperation async = Application.LoadLevelAsync(sceneToLoad);
		yield return async;
#endif
		Debug.Log( "[SceneLoad] Completed: " + (Time.realtimeSinceStartup - startTime).ToString("F2") + " sec(s)");
	}
}
