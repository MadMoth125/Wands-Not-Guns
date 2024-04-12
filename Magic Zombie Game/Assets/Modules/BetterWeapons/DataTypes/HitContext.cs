using UnityEngine;

namespace Weapons
{
	/// <summary>
	/// Contains data about a hit target.
	/// </summary>
	public struct HitContext
	{
		public HitContext(GameObject target, float distance)
		{
			this.target = target;
			this.distance = distance;
		}
		
		public void SetData(GameObject target, float distance)
		{
			this.target = target;
			this.distance = distance;
		}
		
		public GameObject target;
		public float distance;
	}
}