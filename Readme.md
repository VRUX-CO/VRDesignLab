
### An open source VR design project written in [Unity](http://unity3d.com)

#### Runs on Cardboard, DK2, GearVR.

Inspired by Google's Cardboard Design Lab Android app.  This app was designed to be extensible. It's simple to add additional labs and hook them up to the main menu.

This is a VR community owned project. [License](http://choosealicense.com/no-license/): Free to extend, modify and improve.  

Send us your pull requests!

#### How to add a VR Lab

1. Make a new scene
2. Drag in the "App Boot Strap" prefab (Assets/Application/App Boot Strap)
3. Add a menu entry in the AllMenuItems() function inside LevelManager.cs for the new scene. (Assets/Application/Level Manager)
4. Add scene to build settings so it gets included in the build
5. Start inside the VRDL_Start scene to run app.

#### Compiling for Platforms

**Windows/Mac/Linux/DK2:**

1. Open the build settings, Click PC/Mac/Linux and click the appropriate OS type to the right.
2. Build

**Cardboard:**

1. Open the VRDL_Start scene and select the App Boot Strap object in teh heirarchy. Check the checkbox named "Build for Cardboard".
2. Open the build settings, click Android, click Player Settings and uncheck "Virtual Reality Supported" checkbox.
3. Build

**GearVR:**

1. Open the build settings, click Android, click Player Settings and make sure "Virtual Reality Supported" is checked.
2. Build

There's still lots of work to go to complete all the labs.  We need your help! [Email](mailto:steve@vrux.co) me if you have questions or suggestions.

Thanks!

by [VRUX](http://vrux.co)
