using System;
using UnityEngine;

[Serializable]
public struct ProjectileHitData
{
	public ProjectileHitData(GameObject projectile, Collision hitCollider)
	{
		this.projectile = projectile;
		this.hitCollider = hitCollider;
	}
	
	public void SetData(GameObject projectile, Collision hitCollider)
	{
		this.projectile = projectile;
		this.hitCollider = hitCollider;
	}
	
	[HideInInspector]
	public GameObject projectile;
	
	[HideInInspector]
	public Collision hitCollider;
}