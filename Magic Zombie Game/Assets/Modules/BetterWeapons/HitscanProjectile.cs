using System;
using UnityEngine;
using PhysicsEx = RotaryHeart.Lib.PhysicsExtension;

public class HitscanProjectile : ProjectileHandler
{
	public float maxDistance = 100f;
	public LayerMask layers = ~0;

	public override void FireProjectile(Action<HitContext> onHitComplete)
	{
		if (PhysicsEx.Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, maxDistance, layers,
			    PhysicsEx.PreviewCondition.Editor,
			    1f, 
			    Color.green,
			    Color.red))
		{
			onHitComplete?.Invoke(new HitContext
				{
					distance = hitInfo.distance,
					target = hitInfo.collider.gameObject
				});
		}
	}
}