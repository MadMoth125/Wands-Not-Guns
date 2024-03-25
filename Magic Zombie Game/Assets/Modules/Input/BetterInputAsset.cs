using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace CustomControls
{
	[CreateAssetMenu(fileName = "BetterInputAsset", menuName = "ScriptableObject/BetterInputAsset")]
	public class BetterInputAsset : ScriptableObject
	{
		private PlayerControls _playerControls;
		
		public void EnableInput()
		{
			_playerControls ??= new PlayerControls();
			_playerControls.Enable();
			SubscribeToEvents();
		}
		
		public void DisableInput()
		{
			_playerControls?.Disable();
			UnsubscribeFromEvents();
		}
		
		private void SubscribeToEvents()
		{
			_playerControls.Default.Movement.performed += OnMovementEvent;
			_playerControls.Default.Movement.canceled += OnMovementEvent;
			
			_playerControls.Default.CursorPosition.performed += OnCursorPositionEvent;
			_playerControls.Default.CursorPosition.canceled += OnCursorPositionEvent;
			
			InputSystem.onDeviceChange += OnDeviceChange;
			// InputUser.onChange += OnControlsChanged;
		}

		private void UnsubscribeFromEvents()
		{
			_playerControls.Default.Movement.performed -= OnMovementEvent;
			_playerControls.Default.Movement.canceled -= OnMovementEvent;
			
			_playerControls.Default.CursorPosition.performed -= OnCursorPositionEvent;
			_playerControls.Default.CursorPosition.canceled -= OnCursorPositionEvent;
			
			InputSystem.onDeviceChange -= OnDeviceChange;
			// InputUser.onChange -= OnControlsChanged;
		}

		private void OnMovementEvent(InputAction.CallbackContext ctx)
		{
			Debug.LogWarning($"Movement event: {ctx.ReadValue<Vector2>()}");
		}

		private void OnCursorPositionEvent(InputAction.CallbackContext ctx)
		{
			// Debug.LogWarning($"Cursor position event: {ctx.ReadValue<Vector2>()}");
		}

		public void OnControlsChanged()
		{
			// Debug.LogWarning("Controls changed");
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			return;
			/*if (change == InputDeviceChange.Added)
			{
				Debug.Log("Device added: " + device.displayName);
			}
			else if (change == InputDeviceChange.Removed)
			{
				Debug.Log("Device removed: " + device.displayName);
			}*/

			// Debug.Log($"Device change: {device.description.manufacturer}");
			// Debug.Log($"Device change: {arg2}");
		}
		
	}
}