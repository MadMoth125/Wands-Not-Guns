using KinematicCharacterController;
using MyCustomControls;
using UnityEngine;

namespace Player.Controller
{
	/// <summary>
	/// Interface for implementing necessary methods for any movement ability.
	/// </summary>
	public interface IMovementAbility
	{
		public void Enable();

		public void Disable();

		/// <summary>
		/// Returns the value affecting the player's velocity.
		/// </summary>
		public void UpdateVelocity(ref Vector3 currentVelocity, in float deltaTime);

		/// <summary>
		/// Returns the value affecting the player's rotation.
		/// </summary>
		public void UpdateRotation(ref Quaternion currentRotation, in float deltaTime);

		/// <summary>
		/// Sets the references for the movement ability.
		/// </summary>
		/// <param name="controls">The controls asset.</param>
		/// <param name="motor">The kinematic character motor.</param>
		public void SetReferences(GameControlsAsset controls, KinematicCharacterMotor motor);
	}
}