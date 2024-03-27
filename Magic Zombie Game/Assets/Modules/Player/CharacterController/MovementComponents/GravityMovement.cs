using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Controller
{
	/// <summary>
	/// Controls the vertical movement of the player to simulate gravity.
	/// </summary>
	[Serializable]
	public class GravityMovement : MovementComponent
	{
		public float gravityStrength = 30f;
	
		public float drag = 0.1f;

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