using CustomControls;
using UnityEngine;
using UnityEngine.InputSystem;

public class BetterInputTester : MonoBehaviour
{
	public BetterInputAsset inputAsset;
	public PlayerInput playerInput;
	
	
	#region Unity Methods

	private void Start()
	{
		
	}

	private void Update()
	{
		
	}

	private void OnEnable()
	{
		playerInput.onControlsChanged += OnControlsChanged;
		InputSystem.onActionChange += OnActionChanged;
		inputAsset.EnableInput();
	}

	private void OnDisable()
	{
		playerInput.onControlsChanged -= OnControlsChanged;
		InputSystem.onActionChange -= OnActionChanged;
		inputAsset.DisableInput();
	}

	#endregion

	private void OnControlsChanged(PlayerInput obj)
	{
		
		Debug.Log("Controls changed");
	}

	private void OnActionChanged(object obj, InputActionChange change)
	{
		if (change == InputActionChange.ActionPerformed)
		{
			InputAction receivedInputAction = (InputAction)obj;
			InputDevice lastDevice = receivedInputAction.activeControl.device;
 
			Debug.Log(lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse"));
			//If needed we could check for "XInputControllerWindows" or "DualShock4GamepadHID"
			//Maybe if it Contains "controller" could be xbox layout and "gamepad" sony? More investigation needed
		}
	}
}