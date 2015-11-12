
# VR Design Lab project written in [Unity](http://unity3d.com)

## Runs on Cardboard, DK2, GearVR.

Inspired by Google's <a href="https://play.google.com/store/apps/details?id=com.google.vr.cardboard.apps.designlab&hl=en" target="_blank">Cardboard Design Lab</a> Android app.  VRUX Design Lab was designed to showcase the dos and donts of VR with instructional labs inside of VR. 
It was designed to be extensible so anyone with a new idea can add a new lab. It's simple to add additional labs and hook them up to the main menu.

This is a VR community owned project.  Open sourced. [No License](http://choosealicense.com/no-license/).  

Send us your pull requests!

## How to add a VR Lab

1. Make a new scene
2. Drag in the "App Boot Strap" prefab (Assets/Application/App Boot Strap)
3. Add a menu entry in the AllMenuItems() function inside LevelManager.cs for the new scene. (Assets/Application/Level Manager)
4. Add scene to build settings so it gets included in the build
5. Start inside the VRDL_Start scene to run app.

## Compiling for Platforms

**Windows/Mac/Linux/DK2:**

1. Open the build settings, Click PC/Mac/Linux and click the appropriate OS type to the right.
1. For DK2, click Player Settings and make sure "Virtual Reality Supported" is checked.
2. Build

**Cardboard:**

1. Open the VRDL_Start scene and select the App Boot Strap object in teh heirarchy. Check the checkbox named "Build for Cardboard".
2. Open the build settings, click Android, click Player Settings and uncheck "Virtual Reality Supported" checkbox.
3. If your building for GearVR and Cardboard, make sure to have unique Product names and Bundle Idenifiers so you can run both on your phone.  See build settings/player settings/other under the phone tab.
4. Plug in phone and Build & Run to 

**GearVR:**

1. Open the build settings, click Android, click Player Settings and make sure "Virtual Reality Supported" is checked.
2. Get your [Oculus Signature File](https://developer.oculus.com/osig/) and put it in Project/Assets/Plugins/Android/assets/
3. If your building for GearVR and Cardboard, make sure to have unique Product names and Bundle Idenifiers so you can run both versions on your phone.  See build settings/player settings/other under the phone tab.
4. Plug in phone and Build & Run

## Engage and Contribute

There's still lots of work to go to complete all the labs.  
We need your help! 
[Email](mailto:steve@vrux.co) me if you have questions or suggestions.

**Contributors:**

- Steve Gehrman
- Xin Chung
- (your name)

Thanks!

by [VRUX](http://vrux.co)