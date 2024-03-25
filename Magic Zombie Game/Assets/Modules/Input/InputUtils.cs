using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

namespace CustomControls
{
	public static class InputUtils
	{
		public static Device GetDeviceFromContext(InputAction.CallbackContext ctx)
		{
			switch (ctx.control.device)
			{
				case Keyboard:
				case Mouse:
					return Device.KeyboardMouse;
				case XInputController:
					return Device.XboxGamepad;
				case DualShockGamepad:
					return Device.PlayStationGamepad;
				case Gamepad:
					return Device.GenericGamepad;
				default:
					return Device.None;
			}
		}
	}
}