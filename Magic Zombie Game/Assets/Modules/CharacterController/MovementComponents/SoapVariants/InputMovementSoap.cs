using System;
using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using ScriptExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Controller.SoapVer
{
	/// <summary>
	/// Controls the player's movement on the ground and in-air.
	/// Copy of the InputMovement class, but instead uses Soap variables
	/// for configuration instead of instance variables.
	/// </summary>
	[Serializable]
	public class InputMovementSoap : MovementComponent
	{
		public Transform forwardOverride;
		public FloatVariable moveSpeed;
		public FloatVariable moveSharpness;
		public FloatVariable airMoveSpeed;
		public FloatVariable airAccelerationSpeed;
		
		private Vector3 _moveDirection = Vector3.zero;

		public override void Enable()
		{
			base.Enable();
			Controls.OnMoveCallback += OnMoveListener;
		}

		public override void Disable()
		{
			base.Disable();
			Controls.OnMoveCallback -= OnMoveListener;
		}

		public override void UpdateVelocity(ref Vector3 currentVelocity, in float deltaTime)
		{
			if (MotorValid && Motor.GroundingStatus.IsStableOnGround)
			{
				HandleGroundMovement(ref currentVelocity, deltaTime);
			}
			else
			{
				HandleAirMovement(ref currentVelocity, deltaTime);
			}
		}

		public override void UpdateRotation(ref Quaternion currentRotation, in float deltaTime)
		{
		
		}

		#region Movement

		private void HandleGroundMovement(ref Vector3 currentVelocity, in float deltaTime)
		{
			float currentVelocityMagnitude = currentVelocity.magnitude;
			Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

			// Reorient source velocity on current ground slope
			// (this is because we don't want our smoothing to cause any velocity losses in slope changes)
			currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

			// Calculate target velocity
			Vector3 inputRight = Vector3.Cross(_moveDirection, Motor.CharacterUp);
			Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveDirection.magnitude;
			Vector3 targetMovementVelocity = reorientedInput * moveSpeed;

			// Smooth movement Velocity
			currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-moveSharpness * deltaTime));
		}
	
		private void HandleAirMovement(ref Vector3 currentVelocity, in float deltaTime)
		{
			if (_moveDirection.sqrMagnitude > 0f)
			{
				Vector3 addedVelocity = _moveDirection * (airAccelerationSpeed * deltaTime);
				Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);
					
				// Limit air velocity from inputs
				if (currentVelocityOnInputsPlane.magnitude < airMoveSpeed)
				{
					// clamp addedVel to make total vel not exceed max vel on inputs plane
					Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, airMoveSpeed);
					addedVelocity = newTotal - currentVelocityOnInputsPlane;
				}
				else
				{
					// Make sure added vel doesn't go in the direction of the already-exceeding velocity
					if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
					{
						addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
					}
				}

				// Prevent climbing on un-stable slopes with air movement
				if (Motor.GroundingStatus.FoundAnyGround)
				{
					if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
					{
						Vector3 perpendicularObstructionNormal = Vector3.Cross(
							Vector3.Cross(
								Motor.CharacterUp, 
								Motor.GroundingStatus.GroundNormal),
							Motor.CharacterUp).normalized;
					
						addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpendicularObstructionNormal);
					}
				}

				// Apply added velocity
				currentVelocity += addedVelocity;
			}
		}

		#endregion
	
		private void OnMoveListener(InputAction.CallbackContext ctx)
		{
			var tempMoveDir = ctx.ReadValue<Vector2>().SwizzleXZ();
		
			if (forwardOverride != null)
			{
				tempMoveDir = forwardOverride.forward * tempMoveDir.z + forwardOverride.right * tempMoveDir.x;
			}
			else
			{
				tempMoveDir = Motor.CharacterForward * tempMoveDir.z + Motor.CharacterRight * tempMoveDir.x;
			}

			_moveDirection = tempMoveDir;
		}
	}
}