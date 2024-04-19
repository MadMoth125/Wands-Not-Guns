using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
	/// <summary>
	/// Contains data about a hit target.
	/// </summary>
	public struct HitContext
	{
		public HitContext(GameObject target, Collider collider, float travelDistance)
		{
			this.hitTarget = target;
			this.hitCollider = collider;
			this.travelDistance = travelDistance;
		}
		
		public void SetData(GameObject target, Collider collider, float travelDistance)
		{
			this.hitTarget = target;
			this.hitCollider = collider;
			this.travelDistance = travelDistance;
		}
		
		public GameObject hitTarget;
		public Collider hitCollider;
		public float travelDistance;
	}
}