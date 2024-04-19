using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Weapons;
using Weapons.Events;

public class ParticleProjectileHandler : ProjectileHandler
{
	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventCollision projectileHitEvent;
	
	[SerializeField]
	private Transform firePoint;

	[Required]
	[SerializeField]
	private AssetReferenceGameObject projectile;
	
	private HitContext _hitContext;
	private Action<HitContext> _onHitComplete;

	public void Spawn(Transform point)
	{
		Spawn(point.position, point.rotation);
	}

	public void Spawn(Vector3 position, Quaternion rotation)
	{
		projectile.InstantiateAsync(position, rotation);
	}

	public override void FireProjectile(Action<HitContext> onHitComplete)
	{
		_onHitComplete = onHitComplete;
		Spawn(firePoint);
	}

	#region Unity Methods

	private void OnEnable()
	{
		_hitContext = new HitContext(); // reusing the same object to save on resources
		projectileHitEvent.OnRaised += OnProjectileHit;
	}

	private void OnDisable()
	{
		projectileHitEvent.OnRaised -= OnProjectileHit;
	}

	#endregion

	private void OnProjectileHit(ProjectileHitData hitData)
	{
		_hitContext.SetData(
			hitData.hitCollider.gameObject,
			hitData.hitCollider.collider,
			hitData.travelDistance);
		_onHitComplete?.Invoke(_hitContext);
	}
}