using UnityEngine;

namespace KinematicCharacterController
{
	/// <summary>
	/// Represents the entire state of a PhysicsMover that is pertinent for simulation.
	/// Use this to save state or revert to past state
	/// </summary>
	[System.Serializable]
	public struct PhysicsMoverState
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Velocity;
		public Vector3 AngularVelocity;
	}
}