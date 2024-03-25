using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyCustomControls
{
	[CreateAssetMenu(fileName = "PlayerControlsAsset", menuName = "Input/Player Controls", order = 0)]
	public class PlayerGameControls : ControlsAssetBase
	{
		public event Action<InputAction.CallbackContext> OnMoveCallback
		{
			add
			{
				Controls.Game.Move.performed += value;
				Controls.Game.Move.canceled += value;
			}
			remove
			{
				Controls.Game.Move.performed -= value;
				Controls.Game.Move.canceled -= value;
			}
		}
		
		public event Action<InputAction.CallbackContext> OnMoveCursorCallback
		{
			add
			{
				Controls.Game.MoveCursor.performed += value;
				Controls.Game.MoveCursor.canceled += value;
			}
			remove
			{
				Controls.Game.MoveCursor.performed -= value;
				Controls.Game.MoveCursor.canceled -= value;
			}
		}
		
		public event Action<InputAction.CallbackContext> OnUseAbilityCallback
		{
			add
			{
				Controls.Game.UseAbility.performed += value;
				Controls.Game.UseAbility.canceled += value;
			}
			remove
			{
				Controls.Game.UseAbility.performed -= value;
				Controls.Game.UseAbility.canceled -= value;
			}
		}
		
		public event Action<InputAction.CallbackContext> OnInteractCallback
		{
			add
			{
				Controls.Game.Interact.performed += value;
				Controls.Game.Interact.canceled += value;
			}
			remove
			{
				Controls.Game.Interact.performed -= value;
				Controls.Game.Interact.canceled -= value;
			}
		}
	}
}