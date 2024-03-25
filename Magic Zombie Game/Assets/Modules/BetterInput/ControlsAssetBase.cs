using System;
using System.Collections;
using System.Collections.Generic;
using CustomControls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyCustomControls
{
	public class ControlsAssetBase : ScriptableObject
	{
		public event Action<Device> OnDeviceChange;
		
		public Device CurrentDevice => _currentDevice;
		
		protected MyPlayerControls Controls => _internalControls ??= new MyPlayerControls();
		
		private MyPlayerControls _internalControls;
		private Device _currentDevice;
		private Device _newDevice;

		#region Static Methods

		public static Device GetDeviceType(InputDevice device)
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

		#endregion

		#region Public Methods

		public virtual void EnableControls()
		{
			Controls.Enable();
			SubscribeToEvents();
		}

		public virtual void DisableControls()
		{
			Controls?.Disable();
			UnsubscribeFromEvents();
		}

		#endregion

		#region Protected Methods

		protected virtual void SubscribeToEvents()
		{
			InputSystem.onActionChange += CheckInputDeviceChanged;
		}

		protected virtual void UnsubscribeFromEvents()
		{
			InputSystem.onActionChange -= CheckInputDeviceChanged;
		}

		#endregion

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

			_newDevice = GetDeviceType(lastInputDevice);
			if (_currentDevice == _newDevice) return;
			
			_currentDevice = _newDevice;
			OnDeviceChange?.Invoke(_currentDevice);
		}
	}
}