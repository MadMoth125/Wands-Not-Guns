using System;
using System.Collections;
using System.Collections.Generic;
using CustomControls;
using MyCustomControls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameInput
{
	// [CreateAssetMenu(fileName = "PlayerInput", menuName = "ScriptableObject/Input Asset")]
	[Obsolete]
	public class PlayerInputAsset : ScriptableObject
	{
		#region Events

		public event Action<InputAction.CallbackContext> OnMoveCallback
		{
			add
			{
				_playerInputAsset.Game.Move.performed += value;
				_playerInputAsset.Game.Move.canceled += value;
			}
			remove
			{
				_playerInputAsset.Game.Move.performed -= value;
				_playerInputAsset.Game.Move.canceled -= value;
			}
		}


		public event Action<InputAction.CallbackContext> OnMoveCursorCallback
		{
			add
			{
				_playerInputAsset.Game.MoveCursor.performed += value;
				_playerInputAsset.Game.MoveCursor.canceled += value;
			}
			remove
			{
				_playerInputAsset.Game.MoveCursor.performed -= value;
				_playerInputAsset.Game.MoveCursor.canceled -= value;
			}
		}


		public event Action<InputAction.CallbackContext> OnUseAbilityCallback
		{
			add
			{
				_playerInputAsset.Game.UseAbility.performed += value;
				_playerInputAsset.Game.UseAbility.canceled += value;
			}
			remove
			{
				_playerInputAsset.Game.UseAbility.performed -= value;
				_playerInputAsset.Game.UseAbility.canceled -= value;
			}
		}


		public event Action<InputAction.CallbackContext> OnInteractCallback
		{
			add
			{
				_playerInputAsset.Game.Interact.performed += value;
				_playerInputAsset.Game.Interact.canceled += value;
			}
			remove
			{
				_playerInputAsset.Game.Interact.performed -= value;
				_playerInputAsset.Game.Interact.canceled -= value;
			}
		}
		
		public event Action<Device> OnDeviceChange;

		#endregion

		private MyPlayerControls _playerInputAsset;

		private Device _lastDevice;
		
		#region Enable/Disable Input

		public void EnableInput(bool forceEnableAll)
		{
			_playerInputAsset ??= new();
			_playerInputAsset.Enable();
			_playerInputAsset.Game.Enable();
			if (forceEnableAll)
			{
				_playerInputAsset.Game.Move.Enable();
				_playerInputAsset.Game.MoveCursor.Enable();
				_playerInputAsset.Game.UseAbility.Enable();
				_playerInputAsset.Game.Interact.Enable();
			}
			SubscribeToEvents();
		}
		
		public void DisableInput()
		{
			_playerInputAsset.Disable();
			UnsubscribeFromEvents();
		}

		#endregion

		#region Input Action States

		/// <summary>
		/// Enables or disables the move input action.
		/// </summary>
		/// <param name="enabled">The state to set the input action to.</param>
		public void SetMoveInputState(bool enabled)
		{
			if (enabled)
			{
				_playerInputAsset.Game.Move.Enable();
			}
			else
			{
				_playerInputAsset.Game.Move.Disable();
			}
		}
		
		/// <summary>
		/// Enables or disables the move cursor input action.
		/// </summary>
		/// <param name="enabled">The state to set the input action to.</param>
		public void SetMoveCursorInputState(bool enabled)
		{
			if (enabled)
			{
				_playerInputAsset.Game.MoveCursor.Enable();
			}
			else
			{
				_playerInputAsset.Game.MoveCursor.Disable();
			}
		}
		
		/// <summary>
		/// Enables or disables the use ability input action.
		/// </summary>
		/// <param name="enabled">The state to set the input action to.</param>
		public void SetUseAbilityInputState(bool enabled)
		{
			if (enabled)
			{
				_playerInputAsset.Game.UseAbility.Enable();
			}
			else
			{
				_playerInputAsset.Game.UseAbility.Disable();
			}
		}
		
		/// <summary>
		/// Enables or disables the interact input action.
		/// </summary>
		/// <param name="enabled">The state to set the input action to.</param>
		public void SetInteractInputState(bool enabled)
		{
			if (enabled)
			{
				_playerInputAsset.Game.Interact.Enable();
			}
			else
			{
				_playerInputAsset.Game.Interact.Disable();
			}
		}

		#endregion
		
		private void SubscribeToEvents()
		{
			InputSystem.onActionChange += CheckInputDeviceChanged;
		}

		private void UnsubscribeFromEvents()
		{
			InputSystem.onActionChange -= CheckInputDeviceChanged;
		}

		/// <summary>
		/// Handles detecting input device changes from the <see cref="InputSystem.onActionChange"/> event.
		/// Only triggers on <see cref="InputActionChange.ActionPerformed"/> and if the current input
		/// came from a different device than the last one.
		/// Invokes the <see cref="OnDeviceChange"/> event when the given conditions are met.
		/// </summary>
		private void CheckInputDeviceChanged(object actionObject, InputActionChange actionChange)
		{
			if (actionChange != InputActionChange.ActionPerformed) return;
			if (actionObject is not InputAction action) return;

			var lastInputDevice = action.activeControl?.device;
			if (lastInputDevice == null) return;

			var lastDevice = GetDeviceType(lastInputDevice);
			if (_lastDevice == lastDevice) return;
			
			_lastDevice = lastDevice;
			OnDeviceChange?.Invoke(_lastDevice);
		}

		/// <summary>
		/// Determines the hardware device type based on the given <paramref name="device"/>.
		/// </summary>
		/// <param name="device">The input device to determine the type of.</param>
		/// <returns>The simplified, enum, device type.</returns>
		private static Device GetDeviceType(InputDevice device)
		{
			const string keyboard_name = "Keyboard";
			const string mouse_name = "Mouse";
			const string xbox_gamepad_name = "XInputControllerWindows";
			const string playstation_gamepad_name = "DualShock4GamepadHID";
			
			string[] words = device.name.Split('/');
			
			// index 0 will always be the device name
			switch (words[0])
			{
				case keyboard_name:
				case mouse_name:
					return Device.KeyboardMouse;
				
				case xbox_gamepad_name:
					return Device.XboxGamepad;
				
				case playstation_gamepad_name:
					return Device.PlayStationGamepad;
				
				case "Gamepad":
					return Device.GenericGamepad;
				
				default:
					return Device.None;
			}
		}
	}
}