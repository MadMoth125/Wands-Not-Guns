using System;
using UnityEngine;

namespace Weapons
{
	/// <summary>
	/// Contains collision data from a projectile and other partially relevant data.
	/// Uses a builder pattern to allow for daisy-chain 'construction', avoiding the need for multiple constructors.
	/// Also useful for reusing the same struct instance for multiple purposes. 
	/// </summary>
	[Serializable] // Only marked as serializable for Soap scriptable events
	public struct ProjectileHitData
	{
		#region Constructors

		public ProjectileHitData(GameObject projectile, Collision hitCollider, Vector3 startPosition, Vector3 endPosition, float travelDistance, float travelTime)
		{
			this.projectile = projectile;
			this.hitCollider = hitCollider;
			this.startPosition = startPosition;
			this.endPosition = endPosition;
			this.travelDistance = travelDistance;
			this.travelTime = travelTime;
		}

		#endregion

		#region Builder Methods

		/// <summary>
		/// Sets the projectile that hit the target.
		/// </summary>
		public ProjectileHitData SetProjectile(GameObject projectile)
		{
			this.projectile = projectile;
			return this;
		}
		
		/// <summary>
		/// Sets the collider that was hit by the projectile.
		/// </summary>
		public ProjectileHitData SetHitCollider(Collision hitCollider)
		{
			this.hitCollider = hitCollider;
			return this;
		}
		
		/// <summary>
		/// Sets the start position of the projectile.
		/// </summary>
		public ProjectileHitData SetStartPosition(Vector3 startPosition)
		{
			this.startPosition = startPosition;
			return this;
		}
		
		/// <summary>
		/// Sets the end position/hit point of the projectile.
		/// </summary>
		public ProjectileHitData SetEndPosition(Vector3 endPosition)
		{
			this.endPosition = endPosition;
			return this;
		}
		
		/// <summary>
		/// Sets the distance the projectile traveled.
		/// </summary>
		public ProjectileHitData SetTravelDistance(float travelDistance)
		{
			this.travelDistance = travelDistance;
			return this;
		}
		
		/// <summary>
		/// Sets the time it took for the projectile to travel.
		/// </summary>
		public ProjectileHitData SetTravelTime(float travelTime)
		{
			this.travelTime = travelTime;
			return this;
		}
		
		/// <summary>
		/// Sets all data at once.
		/// </summary>
		public void SetData(GameObject projectile, Collision hitCollider, Vector3 startPosition, Vector3 endPosition, float travelDistance, float travelTime)
		{
			this.projectile = projectile;
			this.hitCollider = hitCollider;
			this.startPosition = startPosition;
			this.endPosition = endPosition;
			this.travelDistance = travelDistance;
			this.travelTime = travelTime;
		}

		#endregion

		#region Fields

		[HideInInspector]
		public GameObject projectile;

		[HideInInspector]
		public Collision hitCollider;

		[HideInInspector]
		public Vector3 startPosition;

		[HideInInspector]
		public Vector3 endPosition;

		[HideInInspector]
		public float travelDistance;

		[HideInInspector]
		public float travelTime;

		#endregion
	}
}