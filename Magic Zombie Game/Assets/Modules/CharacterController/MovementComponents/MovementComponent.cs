using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using MyCustomControls;
using UnityEngine;

namespace Player.Controller
{
	/// <summary>
	/// Simple base class for movement components.
	/// Holds references to a <see cref="KinematicCharacterMotor"/> and <see cref="GameControlsAsset"/>
	/// received from the "owning" <see cref="MovementController"/>.
	/// </summary>
	public abstract class MovementComponent : IMovementAbility
	{
		protected GameControlsAsset Controls { get; private set; }

		protected KinematicCharacterMotor Motor { get; private set; }

		/// <summary>
		/// Shorthand for checking if the motor is valid. (e.g. value != null)
		/// </summary>
		protected bool MotorValid => Motor != null;
	
		public virtual void Enable()
		{
			
		}

		public virtual void Disable()
		{
			
		}

		public abstract void UpdateVelocity(ref Vector3 currentVelocity, in float deltaTime);

		public abstract void UpdateRotation(ref Quaternion currentRotation, in float deltaTime);

		public void SetReferences(GameControlsAsset controls, KinematicCharacterMotor motor)
		{
			Controls = controls;
			Motor = motor;
		}
	}
}