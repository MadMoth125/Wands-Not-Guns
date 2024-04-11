using System;
using Mr_sB.UnityTimer;
using Sirenix.OdinInspector;
using UnityEngine;

public class PooledProjectile : ProjectileHandler
{
	// TODO: Add the OnProjectileHit event asset here.
	// either have the projectile return to pool when it hits something
	// or have it return to pool after a certain amount of time.
	
	[Tooltip("The transform that the projectile will be fired from.")]
	[SerializeField]
	private Transform firePoint;

	[Tooltip("The object pool that the projectile will be taken from.")]
	[SerializeField]
	private ProjectileObjectPool objectPool;
	
	public override void FireProjectile(Action<HitContext> onHitComplete)
	{
		if (firePoint == null)
		{
			Debug.LogError("Fire point is not set.");
			return;
		}

		var projectile = objectPool.GetElement(firePoint);
		Timer.DelayAction(5f, () =>
		{
			objectPool.ReleaseElement(projectile);
		});
	}
}