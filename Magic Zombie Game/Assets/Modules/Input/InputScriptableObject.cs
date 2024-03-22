using Core;
using Core.CustomDebugger;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputMapping
{
	[CreateAssetMenu(fileName = "InputScriptableObject", menuName = "ScriptableObject/InputScriptableObject")]
	public class InputScriptableObject : ScriptableObject
	{
		public bool Enabled => _isInputEnabled;
	
		public bool UsingKeyboard => !_isGamepadActive;
	
		public bool UsingGamepad => _isGamepadActive;
	
		[SerializeField]
		private LoggerScriptableObject logger;
	
		private PlayerControls _internalPlayerControls;
	
		private bool _isInputEnabled; // true if the input is enabled
		private bool _isGamepadActive; // true if the last input was from a gamepad
		private Vector2 _movementInput; // wasd/gamepad left stick
		private Vector2 _internalCursorPosition; //
		private Vector2 _mousePositionInput; // mouse position
		private Vector2 _gamepadLookDirectionInput; // gamepad right stick

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
			// TickSystem.OnUpdateTick += OnUpdateTickListener;
			_internalPlayerControls.Default.Movement.performed += OnMovementListener;
			_internalPlayerControls.Default.LookPosition.performed += OnLookListener;
		}

		private void UnsubscribeFromEvents()
		{
			// TickSystem.OnUpdateTick -= OnUpdateTickListener;
			_internalPlayerControls.Default.Movement.performed -= OnMovementListener;
			_internalPlayerControls.Default.LookPosition.performed -= OnLookListener;
		}

		#endregion

		#region Event Listeners

		// called every frame from static tick system
		private void OnUpdateTickListener(float delfaTime)
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