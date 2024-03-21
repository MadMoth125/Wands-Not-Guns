using System;
using InputMapping;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputParser : MonoBehaviour
{
	public struct GamepadParams
	{
		public Vector2 delta;
		public Vector2 direction;
	}
	public Vector2 MovementInput { get; private set; }
	
	public Vector2 CursorPositionRaw { get; private set; }
	
	public Vector2 CursorPositionNormalized { get; private set; }
	
	public bool GamepadActive { get; private set; }
	
	private PlayerControls _playerControls;
	
	private Vector2 _lastLookPosition; // mouse
	private Vector2 _lastLookDelta; // gamepad
	private Vector2 _internalCursorPosition;
	
	#region Unity Methods

	private void Update()
	{
		if (GamepadActive)
		{
			_internalCursorPosition += _lastLookDelta * (Time.deltaTime * Screen.width);
		}
		else
		{
			_internalCursorPosition = _lastLookPosition;
		}
		
		_internalCursorPosition.x = Mathf.Clamp(_internalCursorPosition.x, 0, Screen.width);
		_internalCursorPosition.y = Mathf.Clamp(_internalCursorPosition.y, 0, Screen.height);
		
		CursorPositionNormalized = new Vector2(Mathf.Clamp01(_internalCursorPosition.x / Screen.width),
			Mathf.Clamp01(_internalCursorPosition.y / Screen.height));
			
		CursorPositionRaw = CursorPositionNormalized * new Vector2(Screen.width, Screen.height);
	}

	private void OnEnable()
	{
		_playerControls = new PlayerControls();
		_playerControls.Enable();

		_playerControls.Default.Movement.performed += OnMovement;
		_playerControls.Default.Movement.canceled += OnMovement;
		
		_playerControls.Default.LookPosition.performed += OnLookPosition;
		_playerControls.Default.LookPosition.canceled += OnLookPosition;
	}

	private void OnDisable()
	{
		_playerControls.Default.Movement.performed -= OnMovement;
		_playerControls.Default.Movement.canceled -= OnMovement;
		
		_playerControls.Default.LookPosition.performed -= OnLookPosition;
		_playerControls.Default.LookPosition.canceled -= OnLookPosition;
		
		_playerControls.Disable();
		_playerControls.Dispose();
	}

	#endregion
	
	private void OnMovement(InputAction.CallbackContext obj)
	{
		GamepadActive = obj.control.device is Gamepad;
		
		MovementInput = obj.ReadValue<Vector2>();
		Debug.Log($"Movement: {obj.ReadValue<Vector2>()}");
	}
	
	private void OnLookPosition(InputAction.CallbackContext obj)
	{
		GamepadActive = obj.control.device is Gamepad;

		if (GamepadActive)
		{
			_lastLookDelta = obj.ReadValue<Vector2>();
		}
		else
		{
			_lastLookPosition = obj.ReadValue<Vector2>();
		}
		
		Debug.Log($"Look Position: {obj.ReadValue<Vector2>()}");
	}
}