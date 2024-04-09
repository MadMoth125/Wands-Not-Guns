using System;
using Core.Utils;
using MyCustomControls;
using Obvious.Soap;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controller
{
	[Serializable]
	public class CharacterRotation : MovementComponent
	{
		public enum LookDirection
		{
			TowardsCursor = 0,
			TowardsMovementDirection = 1,
			[InspectorName("Towards Movement Input (Not implemented)")] 
			TowardsMovementInput = 2,
			TowardsTarget = 3,
			AwayFromTarget = 4,
			Custom = 5,
		}

		public LookDirection lookDirectionType = LookDirection.TowardsMovementInput;
		
		[ShowIf("lookDirectionType", LookDirection.TowardsCursor, Animate = false)]
		[SerializeField]
		public MouseUtilityAsset mouseUtility;

		[ShowIf("lookDirectionType", LookDirection.TowardsTarget, Animate = false)]
		[ShowIf("lookDirectionType", LookDirection.AwayFromTarget, Animate = false)]
		public Transform lookTarget;

		[ShowIf("lookDirectionType", LookDirection.Custom, Animate = false)]
		public FloatVariable lookAngleVariable;
		
		public FloatVariable rotationSharpnessVariable;
		
		private Device _currentDevice = Device.KeyboardMouse;
		
		private Vector2 _cursorScreenPosition = Vector2.zero;
		private Vector3 _cursorWorldPosition = Vector3.zero;
		
		private Vector3 _lookDirection = Vector3.zero;
		
		public override void Enable()
		{
			base.Enable();
			Controls.OnMoveCursorCallback += OnMoveCursorListener;
			Controls.OnDeviceChange += OnDeviceChangedListener;
		}

		public override void Disable()
		{
			base.Disable();
			Controls.OnMoveCursorCallback -= OnMoveCursorListener;
			Controls.OnDeviceChange -= OnDeviceChangedListener;
		}

		public override void UpdateVelocity(ref Vector3 currentVelocity, in float deltaTime)
		{
			// Do nothing
		}

		public override void UpdateRotation(ref Quaternion currentRotation, in float deltaTime)
		{
			float rotationSharpness = rotationSharpnessVariable.OrNull() ?? 0f;
			float lookAngle = lookAngleVariable.OrNull() ?? 0f;
			Vector3 targetPosition = lookTarget.OrNull() ? lookTarget.position : Vector3.zero;
			
			switch (lookDirectionType)
			{
				default:
				case LookDirection.Custom:
					_lookDirection = Quaternion.AngleAxis(lookAngle, Motor.CharacterUp) * Vector3.forward;
					break;
				case LookDirection.TowardsCursor:
					if (!mouseUtility.OrNull()) break;
					_cursorWorldPosition = mouseUtility.GetMouseWorldPosition(_cursorScreenPosition);
					_lookDirection = _cursorWorldPosition.Set(y: Motor.TransientPosition.y) - Motor.TransientPosition;
					break;
				case LookDirection.TowardsTarget:
					if (!lookTarget.OrNull()) break;
					_lookDirection = targetPosition.Set(y: Motor.TransientPosition.y) - Motor.TransientPosition;
					break;
				case LookDirection.AwayFromTarget:
					if (!lookTarget.OrNull()) break;
					_lookDirection = Motor.TransientPosition - targetPosition.Set(y: Motor.TransientPosition.y);
					break;
				case LookDirection.TowardsMovementDirection:
					_lookDirection = Motor.BaseVelocity;
					break;
				case LookDirection.TowardsMovementInput:
					break;
			}
			
			if (_lookDirection.sqrMagnitude > 0f && rotationSharpness > 0f)
			{
				// Smoothly interpolate from current to target look direction
				Vector3 smoothedLookInputDirection = Vector3.Slerp(
					Motor.CharacterForward,
					_lookDirection.normalized, 1 - Mathf.Exp(-rotationSharpness * deltaTime)).normalized;

				// Set the current rotation (which will be used by the KinematicCharacterMotor)
				currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
			}
		}

		private void OnMoveCursorListener(InputAction.CallbackContext ctx)
		{
			switch (_currentDevice)
			{
				default:
				case Device.KeyboardMouse:
					_cursorScreenPosition = MouseUtils.ClampScreenPosition(ctx.ReadValue<Vector2>());
					break;
				case Device.GenericGamepad:
				case Device.PlayStationGamepad:
				case Device.XboxGamepad:
					_cursorScreenPosition = MouseUtils.ClampScreenPosition(ctx.ReadValue<Vector2>() * 15f);
					break;
				case Device.None:
					break;
			}
		}
		
		private void OnDeviceChangedListener(Device device)
		{
			_currentDevice = device;
		}
	}
}