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

#if !UNITY_5
#define OVR_LEGACY
#endif

using System;
using System.Runtime.InteropServices;

// Internal C# wrapper for OVRPlugin.

internal static class OVRPlugin
{
	public static System.Version wrapperVersion = new System.Version("0.1.2.0");
	private static System.Version v0110 = new System.Version(0, 1, 1, 0);
	private static System.Version v0120 = new System.Version(0, 1, 2, 0);
	private static System.Version v0500 = new System.Version(0, 5, 0, 0);

	private enum Bool
	{
		False = 0,
		True
	}

	public enum Eye
	{
		None  = -1,
		Left  = 0,
		Right = 1,
		Count = 2
	}

	public enum Tracker
	{
		Default = 0,
		Count,
	}

	public enum Node
	{
		None           = -1,
		LeftEye        = 0,
		RightEye       = 1,
		CenterEye      = 2,
		LeftHand       = 3,
		RightHand      = 4,
		TrackerDefault = 5,
		Count,
	}

	public enum BatteryStatus
	{
		Charging = 0,
		Discharging,
		Full,
		NotCharging,
		Unknown,
	}

	public enum PlatformUI
	{
		GlobalMenu = 0,
		ConfirmQuit,
	}

	private enum Key
	{
		Version = 0,
		ProductName,
		Latency,
		EyeDepth,
		EyeHeight,
		BatteryLevel,
		BatteryTemperature,
		CpuLevel,
		GpuLevel,
		SystemVolume,
		QueueAheadFraction,
		IPD,
		NativeTextureScale,
		VirtualTextureScale,
        Frequency,
    }

	private enum Caps
	{
		SRGB = 0,
		Chromatic,
		FlipInput,
		Rotation,
		HeadModel,
		Position,
		CollectPerf,
		DebugDisplay,
		Monoscopic,
		ShareTexture,
	}

	private enum Status
	{
		Debug = 0,
		HSWVisible,
		PositionSupported,
		PositionTracked,
		PowerSaving,
		Initialized,
		HMDPresent,
		UserPresent,
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2i
	{
		public int x;
		public int y;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2f
	{
		public float x;
		public float y;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3f
	{
		public float x;
		public float y;
		public float z;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Quatf
	{
		public float x;
		public float y;
		public float z;
		public float w;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Posef
	{
		public Quatf Orientation;
		public Vector3f Position;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct InputState
	{
		public uint ConnectedControllers;
		public uint Buttons;
		public uint Touches;
		public uint NearTouches;
		public float LIndexTrigger;
		public float RIndexTrigger;
		public float LHandTrigger;
		public float RHandTrigger;
		public Vector2f LThumbstick;
		public Vector2f RThumbstick;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Sizei
	{
		public int w;
		public int h;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct Frustumf
	{
		public float zNear;
		public float zFar;
		public float fovX;
		public float fovY;
	}

	public static bool srgb
	{
		get { return GetCap(Caps.SRGB); }
		set { SetCap(Caps.SRGB, value); }
	}

	public static bool chromatic
	{
		get { return GetCap(Caps.Chromatic); }
		set { SetCap(Caps.Chromatic, value); }
	}

	public static bool flipInput
	{
		get { return GetCap(Caps.FlipInput); }
		set { SetCap(Caps.FlipInput, value); }
	}

	public static bool rotation
	{
		get { return GetCap(Caps.Rotation); }
		set { SetCap(Caps.Rotation, value); }
	}

	public static bool headModel
	{
		get { return GetCap(Caps.HeadModel); }
		set { SetCap(Caps.HeadModel, value); }
	}

	public static bool position
	{
		get { return GetCap(Caps.Position); }
		set { SetCap(Caps.Position, value); }
	}

	public static bool collectPerf
	{
		get { return GetCap(Caps.CollectPerf); }
		set { SetCap(Caps.CollectPerf, value); }
	}

	public static bool debugDisplay
	{
		get { return GetCap(Caps.DebugDisplay); }
		set { SetCap(Caps.DebugDisplay, value); }
	}

	public static bool monoscopic
	{
		get { return GetCap(Caps.Monoscopic); }
		set { SetCap(Caps.Monoscopic, value); }
	}

	public static bool debug { get { return GetStatus(Status.Debug); } }

	public static bool userPresent { get { return true; /*TODO return GetStatus(Status.UserPresent);*/ } }

	public static bool hmdPresent { get { return GetStatus(Status.HMDPresent); } }

	public static bool hswVisible { get { return GetStatus(Status.HSWVisible); } }

	public static bool positionSupported { get { return GetStatus(Status.PositionSupported); } }

	public static bool positionTracked { get { return GetStatus(Status.PositionTracked); } }

	public static bool powerSaving { get { return GetStatus(Status.PowerSaving); } }

	public static System.Version version
	{
		get {
			if (_version == null)
			{
				try {
					_version = new System.Version(OVRP0100.ovrp_GetString(Key.Version));
				} catch {
					_version = new System.Version(0, 0, 0, 0);
				}
			}

			// Unity 5.1.1f3-p3 have OVRPlugin version "0.5.0", which isn't accurate.
			if (_version == v0500)
				_version = new System.Version(0, 1, 0, 0);

			return _version;
		}
	}
	private static System.Version _version;

	public static string productName { get { return OVRP0100.ovrp_GetString(Key.ProductName); } }

	public static string latency { get { return OVRP0100.ovrp_GetString(Key.Latency); } }

	public static float eyeDepth
	{
		get { return OVRP0100.ovrp_GetFloat(Key.EyeDepth); }
		set { OVRP0100.ovrp_SetFloat(Key.EyeDepth, value); }
	}

	public static float eyeHeight
	{
		get { return OVRP0100.ovrp_GetFloat(Key.EyeHeight); }
		set { OVRP0100.ovrp_SetFloat(Key.EyeHeight, value); }
	}

	public static float batteryLevel
	{
		get { return OVRP0100.ovrp_GetFloat(Key.BatteryLevel); }
		set { OVRP0100.ovrp_SetFloat(Key.BatteryLevel, value); }
	}

	public static float batteryTemperature
	{
		get { return OVRP0100.ovrp_GetFloat(Key.BatteryTemperature); }
		set { OVRP0100.ovrp_SetFloat(Key.BatteryTemperature, value); }
	}

	public static int cpuLevel
	{
		get { return (int)OVRP0100.ovrp_GetFloat(Key.CpuLevel); }
		set { OVRP0100.ovrp_SetFloat(Key.CpuLevel, (float)value); }
	}

	public static int gpuLevel
	{
		get { return (int)OVRP0100.ovrp_GetFloat(Key.GpuLevel); }
		set { OVRP0100.ovrp_SetFloat(Key.GpuLevel, (float)value); }
	}

	public static float systemVolume
	{
		get { return OVRP0100.ovrp_GetFloat(Key.SystemVolume); }
		set { OVRP0100.ovrp_SetFloat(Key.SystemVolume, value); }
	}

	public static float queueAheadFraction
	{
		get { return OVRP0100.ovrp_GetFloat(Key.QueueAheadFraction); }
		set { OVRP0100.ovrp_SetFloat(Key.QueueAheadFraction, value); }
	}

	public static float ipd
	{
		get { return OVRP0100.ovrp_GetFloat(Key.IPD); }
		set { OVRP0100.ovrp_SetFloat(Key.IPD, value); }
	}

#if OVR_LEGACY
	public static bool initialized { get { return GetStatus(Status.Initialized); } }

	public static bool shareTexture
	{
		get { return GetCap(Caps.ShareTexture); }
		set { SetCap(Caps.ShareTexture, value); }
	}

	public static float nativeTextureScale
	{
		get { return OVRP0100.ovrp_GetFloat(Key.NativeTextureScale); }
		set { OVRP0100.ovrp_SetFloat(Key.NativeTextureScale, value); }
	}

	public static float virtualTextureScale
	{
		get { return OVRP0100.ovrp_GetFloat(Key.VirtualTextureScale); }
		set { OVRP0100.ovrp_SetFloat(Key.VirtualTextureScale, value); }
	}
#endif

	public static BatteryStatus batteryStatus
	{
		get { return OVRP0100.ovrp_GetBatteryStatus(); }
	}

	public static Posef GetEyeVelocity(Eye eyeId) { return OVRP0100.ovrp_GetEyeVelocity(eyeId); }
	public static Posef GetEyeAcceleration(Eye eyeId) { return OVRP0100.ovrp_GetEyeAcceleration(eyeId); }
	public static Frustumf GetEyeFrustum(Eye eyeId) { return OVRP0100.ovrp_GetEyeFrustum(eyeId); }
	public static Sizei GetEyeTextureSize(Eye eyeId) { return OVRP0100.ovrp_GetEyeTextureSize(eyeId); }
	public static Posef GetTrackerPose(Tracker trackerId) { return OVRP0100.ovrp_GetTrackerPose(trackerId); }
	public static Frustumf GetTrackerFrustum(Tracker trackerId) { return OVRP0100.ovrp_GetTrackerFrustum(trackerId); }
	public static bool DismissHSW() { return OVRP0100.ovrp_DismissHSW() == Bool.True; }
	public static bool ShowUI(PlatformUI ui) { return OVRP0100.ovrp_ShowUI(ui) == Bool.True; }
	public static bool SetOverlayQuad(bool onTop, bool headLocked, IntPtr texture, IntPtr device, Posef pose, Vector3f scale)
	{
		if (version >= v0110)
			return OVRP0110.ovrp_SetOverlayQuad2(ToBool(onTop), ToBool(headLocked), texture, device, pose, scale) == Bool.True;
		else
			return OVRP0100.ovrp_SetOverlayQuad(ToBool(onTop), texture, device, pose, scale) == Bool.True;
	}

	public static Posef GetNodePose(Node nodeId)
	{
		if (version >= v0120)
			return OVRP0120.ovrp_GetNodePose(nodeId);
		else
			return new Posef();
	}

	public static InputState GetInputState(uint controllerMask)
	{
		if (version >= v0120)
			return OVRP0120.ovrp_GetInputState(controllerMask);
		else
			return new InputState();
	}

	public static bool SetControllerVibration(uint controllerMask, float frequency, float amplitude)
	{
		if (version >= v0120)
			return OVRP0120.ovrp_SetControllerVibration(controllerMask, frequency, amplitude) == Bool.True;
		else
			return false;
	}

#if OVR_LEGACY
	public static bool Update(int frameIndex) { return OVRP0100.ovrp_Update(frameIndex) == Bool.True; }
	public static IntPtr GetNativePointer() { return OVRP0100.ovrp_GetNativePointer(); }
	public static Posef GetEyePose(Eye eyeId) { return OVRP0100.ovrp_GetEyePose(eyeId); }
	public static bool RecenterPose() { return OVRP0100.ovrp_RecenterPose() == Bool.True; }
#endif

	private static bool GetStatus(Status bit)
	{
		if (version >= v0120)
			return OVRP0120.ovrp_GetStatus2((uint)(1 << (int)bit)) != 0;
		else
			return (OVRP0100.ovrp_GetStatus() & (uint)(1 << (int)bit)) != 0;
	}

	private static bool GetCap(Caps cap)
	{
		return ((int)OVRP0100.ovrp_GetCaps() & (1 << (int)cap)) != 0;
	}

	private static void SetCap(Caps cap, bool value)
	{
		if (GetCap(cap) == value)
			return;

		int caps = (int)OVRP0100.ovrp_GetCaps();
		if (value)
			caps |= (1 << (int)cap);
		else
			caps &= ~(1 << (int)cap);

		OVRP0100.ovrp_SetCaps((Caps)caps);
	}

	private static Bool ToBool(bool b)
	{
		return (b) ? OVRPlugin.Bool.True : OVRPlugin.Bool.False;
	}

		private const string pluginName = "OVRPlugin";

	private static class OVRP0100
	{
		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetEyeVelocity(Eye eyeId);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetEyeAcceleration(Eye eyeId);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Frustumf ovrp_GetEyeFrustum(Eye eyeId);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Sizei ovrp_GetEyeTextureSize(Eye eyeId);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetTrackerPose(Tracker trackerId);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Frustumf ovrp_GetTrackerFrustum(Tracker trackerId);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_DismissHSW();

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Caps ovrp_GetCaps();

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetCaps(Caps caps);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovrp_GetStatus();

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovrp_GetFloat(Key key);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetFloat(Key key, float value);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern BatteryStatus ovrp_GetBatteryStatus();

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetOverlayQuad(Bool onTop, IntPtr texture, IntPtr device, Posef pose, Vector3f scale);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_ShowUI(PlatformUI ui);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrp_GetString")]
		private static extern IntPtr _ovrp_GetString(Key key);
		public static string ovrp_GetString(Key key) { return Marshal.PtrToStringAnsi(_ovrp_GetString(key)); }

#if OVR_LEGACY
		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_PreInitialize();

		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_Initialize(RenderAPIType apiType, IntPtr platformArgs);

		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_Shutdown();

		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_SetupDistortionWindow();

		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_DestroyDistortionWindow();

		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_RecreateEyeTexture(Eye eyeId, int stage, void* device, int height, int width, int samples, Bool isSRGB, void* result);
		
		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_SetEyeTexture(Eye eyeId, IntPtr texture);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_Update(int frameIndex);

		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_BeginFrame(int frameIndex);

		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_EndEye(Eye eye);

		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern Bool ovrp_EndFrame(int frameIndex);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovrp_GetNativePointer();
		
		//[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern int ovrp_GetBufferCount();
		
		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetEyePose(Eye eyeId);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_RecenterPose();
#endif
	}

	private static class OVRP0110
	{
		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetOverlayQuad2(Bool onTop, Bool headLocked, IntPtr texture, IntPtr device, Posef pose, Vector3f scale);
	}

	private static class OVRP0120
	{
		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovrp_GetStatus2(uint query);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Posef ovrp_GetNodePose(Node nodeId);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern InputState ovrp_GetInputState(uint controllerMask);

		[DllImport(pluginName, CallingConvention = CallingConvention.Cdecl)]
		public static extern Bool ovrp_SetControllerVibration(uint controllerMask, float frequency, float amplitude);
	}
}
