using System;
using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.PhysicsExtension;
using UnityEngine;
using Physics = RotaryHeart.Lib.PhysicsExtension.Physics;

public class LargeProjectile : ProjectileHandler
{
	public float launchVelocity = 10f;
	public float selfDestructTime = 5f;
	public LayerMask layers = ~0;
	
	public Rigidbody projectilePrefab;
	
	private Action<HitContext> _onHitComplete;
	// private readonly List<Rigidbody> _projectiles = new List<Rigidbody>();
	private Dictionary<Rigidbody, Coroutine> _lifetimes = new();
	
	public override void FireProjectile(Action<HitContext> onHitComplete)
	{
		_onHitComplete = onHitComplete;
		var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
		projectile.velocity = transform.forward * launchVelocity;
		_lifetimes.Add(projectile, StartCoroutine(DestroyProjectile(projectile)));
	}

	private void Update()
	{
		var rbs = new List<Rigidbody>(_lifetimes.Keys);
		foreach (var r in rbs)
		{
			var colliders= Physics.OverlapSphere(r.position, 1.25f, layers, 
				PreviewCondition.Editor,
				Time.deltaTime,
				Color.green,
				Color.red);

			if (colliders.Length <= 0) continue;
			
			StopCoroutine(_lifetimes[r]);
			_lifetimes.Remove(r);
			
			foreach (var c in colliders)
			{
				_onHitComplete?.Invoke(new HitContext
				{
					target = c.gameObject,
					distance = Vector3.Distance(r.position, c.transform.position)
				});
			}
			
			Destroy(r.gameObject);
		}
	}

	private IEnumerator DestroyProjectile(Rigidbody projectile)
	{
		yield return new WaitForSeconds(selfDestructTime);
		_lifetimes.Remove(projectile);
		Destroy(projectile.gameObject);
	}
}