using System;
using UnityEngine;

public abstract class ProjectileHandler : MonoBehaviour
{
	public struct HitContext
	{
		public GameObject target;
		public float distance;
	}
	
	public abstract void FireProjectile(Action<HitContext> onHitComplete);
}