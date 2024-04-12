using System;
using Core.HealthSystem;
using RotaryHeart.Lib.PhysicsExtension;
using UnityEngine;
using PhysicsEx = RotaryHeart.Lib.PhysicsExtension.Physics;

namespace Weapons.Projectiles
{
	public class HitscanProjectile : ProjectileHandler
	{
		public float maxDistance = 100f;
		public LayerMask layers = ~0;

		public override void FireProjectile(Action<HitContext> onHitComplete)
		{
			if (PhysicsEx.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, maxDistance, layers,
				    PreviewCondition.Editor,
				    1f, 
				    Color.green,
				    Color.red))
			{
				// if (hitInfo.collider.GetComponent<IDamageable>() == null) return;
				onHitComplete?.Invoke(new HitContext
				{
					distance = hitInfo.distance,
					target = hitInfo.collider.gameObject
				});
			}
		}
	}
}