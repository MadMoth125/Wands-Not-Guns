using Core.CustomDebugger;
using CustomTickSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace CustomControls
{
	// [CreateAssetMenu(fileName = "InputAsset", menuName = "Input/Input Asset")]
	public class InputAsset : ScriptableObject
	{
		public bool Enabled => _isInputEnabled;
	
		public bool UsingKeyboard => !_isGamepadActive;
	
		public bool UsingGamepad => _currentDevice is Device.GenericGamepad or Device.XboxGamepad or Device.PlayStationGamepad;
	
		[SerializeField]
		private LoggerScriptableObject logger;
	
		private PlayerControls _internalPlayerControls;
	
		private bool _isInputEnabled; // true if the input is enabled
		private bool _isGamepadActive; // true if the last input was from a gamepad
		private Vector2 _movementInput; // wasd/gamepad left stick
		private Vector2 _internalCursorPosition; //
		private Vector2 _mousePositionInput; // mouse position
		private Vector2 _gamepadLookDirectionInput; // gamepad right stick
		private Device _currentDevice;
		
		/// <summary>
		/// Returns the current movement input direction.
		/// Value will always be normalized.
		/// </summary>
		public Vector2 GetMovementInput()
		{
			return _movementInput;
		}

		/// <summary>
		/// Returns the current position of the cursor in a normalized range.
		/// Value will always be clamped between 0 and 1.
		/// </summary>
		public Vector2 GetCursorPosition()
		{
			float clampedX = Mathf.Clamp01(_internalCursorPosition.x / Screen.width);
			float clampedY = Mathf.Clamp01(_internalCursorPosition.y / Screen.height);
			return new Vector2(clampedX, clampedY);
		}
	
		/// <summary>
		/// Returns the current position of the cursor in absolute screen coordinates (e.g. based on the current screen resolution).
		/// Internally, this calls <see cref="GetCursorPosition"/> and multiplies the result by the current screen resolution.
		/// </summary>
		public Vector2 GetAbsoluteCursorPosition()
		{
			return GetCursorPosition() * new Vector2(Screen.width, Screen.height);
		}

		#region Enable/Disable Input

		public void EnableInput()
		{
			logger?.Log("Enabling input", this);
		
			_internalPlayerControls = new PlayerControls();
		
			_internalPlayerControls.Enable();
			_isInputEnabled = true;

			SubscribeToEvents();
		}

		public void DisableInput()
		{
			logger?.Log("Disabling input", this);
		
			UnsubscribeFromEvents();
		
			_internalPlayerControls.Disable();
			_isInputEnabled = false;
		
			_internalPlayerControls.Dispose();
			_internalPlayerControls = null;
		}

		#endregion

		#region Event Subscription

		private void SubscribeToEvents()
		{
			TickSystem.AddListener("Update", OnUpdateTickListener);
			_internalPlayerControls.Default.Movement.performed += OnMovementListener;
			_internalPlayerControls.Default.CursorPosition.performed += OnLookListener;
		}

		private void UnsubscribeFromEvents()
		{
			TickSystem.RemoveListener("Update", OnUpdateTickListener);
			_internalPlayerControls.Default.Movement.performed -= OnMovementListener;
			_internalPlayerControls.Default.CursorPosition.performed -= OnLookListener;
		}

		#endregion

		#region Event Listeners

		// called every frame from static tick system
		private void OnUpdateTickListener()
		{
			if (UsingGamepad)
			{
				_internalCursorPosition += _gamepadLookDirectionInput * (Time.deltaTime * Screen.width);
			}
			else
			{
				_internalCursorPosition = _mousePositionInput;
			}
		
			_internalCursorPosition.x = Mathf.Clamp(_internalCursorPosition.x, 0, Screen.width);
			_internalCursorPosition.y = Mathf.Clamp(_internalCursorPosition.y, 0, Screen.height);
		}
	
		// called when the movement input is performed
		private void OnMovementListener(InputAction.CallbackContext obj)
		{
			_isGamepadActive = obj.control.device is Gamepad;
		
			_movementInput = obj.ReadValue<Vector2>();
			if (_movementInput.magnitude > 1) _movementInput.Normalize();
		}

		// called when the look input is performed
		private void OnLookListener(InputAction.CallbackContext obj)
		{
			_isGamepadActive = obj.control.device is Gamepad;

			Debug.Log(_isGamepadActive);
			var currentScheme = _internalPlayerControls.controlSchemes;
			
			/*var currentScheme = InputControlScheme.FindControlSchemeForDevice(obj.control.device, _internalPlayerControls.controlSchemes);

			if (currentScheme != null)
			{
				Debug.Log($"Current scheme: {currentScheme.Value.name}");
			}
			else
			{
				Debug.LogError("No scheme found");
			}*/

			if (UsingGamepad)
			{
				_gamepadLookDirectionInput = obj.ReadValue<Vector2>();
			}
			else
			{
				_mousePositionInput = obj.ReadValue<Vector2>();
			}
		}

		#endregion
	}
}