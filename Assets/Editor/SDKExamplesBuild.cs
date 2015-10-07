/************************************************************************************

Filename    :   BlockSplosionBuild.cs
Content     :   Build scripts for Blocksplosion
Created     :   May 8, 2014
Authors     :   Alex Howland

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Use of this software is subject to the terms of the Oculus LLC license
agreement provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

************************************************************************************/
using UnityEngine;
using UnityEditor;

using System.IO;

//-------------------------------------------------------------------------------------
// ***** OculusBuildApp
//
// OculusBuildApp allows us to build other Oculus apps from the command line. 
//
partial class OculusBuildApp
{
	// [MenuItem ("TestMenu/BuildAllSampleAPKs")]
	static void BuildAllSampleAPKs() 
	{
		DirectoryInfo dirInfo = new DirectoryInfo("Assets/Scenes");
		FileInfo[] filesInfo = dirInfo.GetFiles("*.unity", SearchOption.AllDirectories);
		foreach (FileInfo f in filesInfo) {
            if (f.FullName.ToLower().Contains("startup_sample.unity")) continue;
			string[] scenes = { "Assets/Scenes/Startup_Sample.unity", f.FullName };
			char[] fileSplit = { '.' };
			string binaryName = f.Name.Split(fileSplit)[0];
#if UNITY_ANDROID
			BuildPipeline.BuildPlayer(scenes, binaryName + ".apk", BuildTarget.Android, BuildOptions.None);
#elif UNITY_STANDALONE_WIN
			BuildPipeline.BuildPlayer(scenes, "./" + binaryName + "32.exe", BuildTarget.StandaloneWindows, BuildOptions.None);
			BuildPipeline.BuildPlayer(scenes, "./" + binaryName + "64.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
#endif
		}
	}

	// Scene transition sample requires the FirstPerson_sample scene
	static void BuildSceneTransitionExampleAPK()
	{
		string[] scenes = { "Assets/Scenes/Startup_Sample.unity","Assets/Scenes/FirstPerson_Sample.unity" };
		BuildPipeline.BuildPlayer(scenes, "SceneTransition_Example.apk", BuildTarget.Android, BuildOptions.None);
	}

	// [MenuItem ("TestMenu/SetPCTarget")]
	static void SetPCTarget()
	{
		if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows) {
			EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.StandaloneWindows);
		}
	}
}
