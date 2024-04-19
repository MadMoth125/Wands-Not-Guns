using System;
using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using UnityEngine;

namespace Player.Controller
{
	/// <summary>
	/// Controls the vertical movement of the player to simulate gravity.
	/// Copy of the GravityMovement class, but instead uses Soap variables
	/// for configuration instead of instance variables.
	/// </summary>
	[Serializable]
	public class GravityMovement : MovementComponentBase
	{
		public FloatVariable gravityStrength;
		public FloatVariable drag;
	
		public override void UpdateVelocity(ref Vector3 currentVelocity, in float deltaTime)
		{
			if (!MotorValid || Motor.GroundingStatus.IsStableOnGround) return;
		
			currentVelocity += new Vector3(0f, -gravityStrength, 0f) * deltaTime;
			currentVelocity *= 1 / (1f + drag * deltaTime);
		}

		public override void UpdateRotation(ref Quaternion currentRotation, in float deltaTime)
		{
		
		}
	}
}