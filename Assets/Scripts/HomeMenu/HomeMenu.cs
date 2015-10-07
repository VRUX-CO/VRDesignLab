/************************************************************************************

Filename    :   HomeMenu.cs
Content     :   An example of the required home/dashboard/back button menu
Created     :   June 30, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Collections;

public enum HomeCommand
{
	None = -1,
	NewGame = 0,
	Continue = 1,
	Quit = 2,		// return to the Oculus home/dashboard app
}

public class HomeMenu : MonoBehaviour
{

	public OVRCameraRig			cameraController = null;
	public float				distanceFromViewer = 3.0f;
	public float				doubleTapDelay = 0.25f;
	public float				longPressDelay = 0.75f;
	public string				backButtonName = "Fire2";	
	public string				selectButtonName = "Fire1";
	public string				menuShowAnim = "Menu_Show";
	public string				menuHideAnim = "Menu_Hide";
	public string				menuIdleAnim = "Menu_Idle";
	public AudioClip			menuShowSound = null;
	public AudioClip			menuHideSound = null;
	public AudioClip			menuHighlightSound = null;
	public AudioClip			menuClickSound = null;

	private AudioSource			audioEmitter = null;
	private Renderer[]			renderers = new Renderer[0];
	private HomeButton[]		buttons = new HomeButton[0];
	private string				highLightPrefix = "_HL";
	private string				selectPrefix = "_Select";
	private HomeButton			activeButton = null;
	private Animation			animator = null;
	private bool				isVisible = false;
	private bool				isShowingOrHiding = false;
	//private bool				homeButtonPressed = false;
	private float				homeButtonDownTime = 0.0f;
	private HomeCommand			selectedCommand = HomeCommand.None;

	/// <summary>
	/// Initialization
	/// </summary>
	void Awake()
	{
		if (cameraController == null)
		{
			Debug.LogError("ERROR: Missing camera controller reference on " + name);
			enabled = false;
			return;
		}
		// gather up all the renderers ( even the deactivated ones )
		renderers = GetComponentsInChildren<Renderer>(true);
		// gather up all the buttons
		buttons = GetComponentsInChildren<HomeButton>(true);
		// set up the animations
		animator = GetComponent<Animation>();
		// set up the audio source
		audioEmitter = GetComponent<AudioSource>();
		// all idle and highlight anims go on layer 0 so that other anims can override them
		foreach (AnimationState state in animator)
		{
			if (state.name.ToLower().Contains(selectPrefix))
			{
				state.layer = 1;
			}
			else
			{
				state.layer = 0;
			}
		}
		// make show and hide layered over everything
		animator[ menuShowAnim ].layer = 2;
		animator[ menuHideAnim ].layer = 2;
		// hide the menu to start
		ShowRenderers( false );
	}

	/// <summary>
	/// Shows and hides the menu
	/// </summary>
	float Show(bool show, bool immediate = false)
	{
		if ((show && isVisible) || (!show && !isVisible))
		{
			if (show)
			{
				// refresh any children
				BroadcastMessage("OnRefresh", SendMessageOptions.DontRequireReceiver);
			}
			return 0.0f;
		}
		float delaySecs = 0.0f;
		if (show)
		{
			// orient and position in front of the player's view
			Vector3 offset = (cameraController.centerEyeAnchor.forward * distanceFromViewer);
			offset.y = (transform.position.y - cameraController.centerEyeAnchor.position.y);
			transform.position = cameraController.centerEyeAnchor.position + offset;
			Vector3 dirToCamera = (cameraController.centerEyeAnchor.position - transform.position);
			dirToCamera.y = 0.0f;
			transform.forward = dirToCamera.normalized;

			// refresh any children
			BroadcastMessage("OnRefresh", SendMessageOptions.DontRequireReceiver);
			// show the menu elements and play the animation
			ShowRenderers(true);
			delaySecs = PlayAnim((immediate) ? menuIdleAnim : menuShowAnim);
		}
		else
		{
			// hide the menu after the hide anim finishes
			delaySecs = (immediate) ? 0.0f : PlayAnim(menuHideAnim);
		}
		if (!immediate)
		{
			PlaySound(show ? menuShowSound : menuHideSound);
		}
		isVisible = show;
		// reset the menu state
		activeButton = null;
		//homeButtonPressed = false;
		homeButtonDownTime = 0.0f;
		// don't allow Show/Hide until this anim is done
		isShowingOrHiding = true;
		Invoke("OnMenuAnimFinished", delaySecs);

		return delaySecs;
	}

	/// <summary>
	/// Called when the show or hide anim is finished
	/// </summary>
	void OnMenuAnimFinished()
	{
		if (!isVisible)
		{
			// hide the renderers now that the hide anim is finished
			ShowRenderers(false);
		}
		isShowingOrHiding = false;
	}

	/// <summary>
	/// Shows and hides the menu renderer elements
	/// </summary>
	void ShowRenderers(bool show)
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = show;
		}
	}

	/// <summary>
	/// Plays a menu animation
	/// </summary>
	float PlayAnim(string animName, bool crossFade = false, bool queued = false)
	{
		if (animName == string.Empty)
		{
			return 0.0f;
		}
		//Debug.Log( "PLAYANIM: " + animName + " time: " + Time.time );
		if (crossFade)
		{
			if (queued)
			{
				animator.CrossFadeQueued(animName, 0.1f);
			}
			else
			{
				animator.CrossFade(animName, 0.1f);
			}
		}
		else
		{
			animator.Play(animName);
		}
		return animator[animName].length;
	}

	/// <summary>
	/// Plays a sound
	/// </summary>
	void PlaySound(AudioClip soundClip)
	{
		if (soundClip == null)
		{
			return;
		}
		audioEmitter.clip = soundClip;
		audioEmitter.Play();
	}

	/// <summary>
	/// Processes input and handles menu interaction
	/// as per the Unity integration doc, the back button responds to "mouse 1" button down/up/etc
	/// </summary>
	void Update()
	{
		if (!isVisible)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				CancelInvoke("DelayedShowMenu");
				if (Time.realtimeSinceStartup < (homeButtonDownTime + doubleTapDelay))
				{
					// reset so the menu doesn't pop up after resetting orientation
					homeButtonDownTime = 0f;
					// reset the HMT orientation
					//OVRManager.display.RecenterPose();
				}
				else
				{
					homeButtonDownTime = Time.realtimeSinceStartup;
				}
			}
			else if (Input.GetKey(KeyCode.Escape) && ((Time.realtimeSinceStartup - homeButtonDownTime) >= longPressDelay))
			{
				Debug.Log("[PlatformUI] Showing @ " + Time.time);
				// reset so something else doesn't trigger afterwards
				Input.ResetInputAxes();
				homeButtonDownTime = 0.0f;
				CancelInvoke("DelayedShowMenu");

				OVRManager.PlatformUIGlobalMenu();
			}
			else if (Input.GetKeyUp(KeyCode.Escape))
			{
				float elapsedTime = (Time.realtimeSinceStartup - homeButtonDownTime);
				if (elapsedTime < longPressDelay)
				{
					if (elapsedTime >= doubleTapDelay)
					{
						Show(true);
					}
					else
					{
						Invoke("DelayedShowMenu", (doubleTapDelay - elapsedTime));
					}
				}
			}
		}
		else if (!isShowingOrHiding)
		{
			// menu is visible, check input
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				// back out of the menu
				Show(false);
			}
			else
			{
				// handle user gaze
				Ray ray = new Ray(cameraController.centerEyeAnchor.position, cameraController.centerEyeAnchor.forward);

				// find the active button
				HomeButton lastActiveButton = activeButton;
				activeButton = null;
				RaycastHit hit = new RaycastHit();
				for (int i = 0; i < buttons.Length; i++)
				{
					if (buttons[i].GetComponent<Collider>().Raycast(ray, out hit, 100.0f))
					{
						activeButton = buttons[i];
						if (activeButton != lastActiveButton)
						{
							// play highlight sound and anim
							PlaySound(menuHighlightSound);
							PlayAnim(buttons[i].name + highLightPrefix, true);
						}
						break;
					}
				}
				if ((activeButton == null) && (lastActiveButton != null))
				{
					// return to idle anim (in our case the default anim clip)
					PlayAnim(menuIdleAnim, true);
				}
				if (activeButton != null)
				{
					// check user tap on a button
					if (Input.GetButtonDown(selectButtonName))
					{
						PlaySound(menuClickSound);
						float delaySecs = PlayAnim(activeButton.name + selectPrefix) + 0.05f;
						selectedCommand = activeButton.commandId;
						// activate the menu item after the anim is done playing
						Invoke("OnMenuItemPressed", delaySecs);
					}
				}
			}
		}
	}

	void DelayedShowMenu()
	{
		Show(true);
	}

	/// <summary>
	/// Handle a home menu button press
	/// </summary>
	void OnMenuItemPressed()
	{
		bool immediate = false;
		switch (selectedCommand)
		{
		case HomeCommand.NewGame:
			// TODO
			break;
		case HomeCommand.Continue:
			// TODO
			break;
#if UNITY_ANDROID && !UNITY_EDITOR
		case HomeCommand.Quit:
			OVRManager.instance.ReturnToLauncher();
			break;
#endif
		case HomeCommand.None:
		default:
			Debug.LogError("Unhandled home command: " + selectedCommand);
			break;
		}
		// hide the menu
		Show(false, immediate);
	}

	/// <summary>
	/// Handle a single press to the back button
	/// </summary>
	void OnBackButtonPressed()
	{
		// TODO
		Show(false);
	}

}
