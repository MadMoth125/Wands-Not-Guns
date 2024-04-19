using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;
using Weapons.Events;

[RequireComponent(typeof(Collider))]
public class DetectProjectileHit : MonoBehaviour
{
	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventCollision projectileHitEvent;
	
	[Required]
	[SerializeField]
	private LayerMaskVariable ignoredLayerMasks;
	
	private Collider _collider;
	private ProjectileHitData _hitData;
	
	#region Unity Methods

	private void Awake()
	{
		_collider = GetComponent<Collider>();
		if (_collider != null && ignoredLayerMasks != null)
		{
			_collider.excludeLayers = ignoredLayerMasks.Value;
		}
	}

	private void Start()
	{
		_hitData = new ProjectileHitData()
			.SetStartPosition(transform.position)
			.SetTravelTime(Time.time);
	}

	private void OnCollisionEnter(Collision other)
	{
		_hitData
			.SetHitCollider(other)
			.SetEndPosition(transform.position)
			.SetTravelDistance(Vector3.Distance(_hitData.startPosition, _hitData.endPosition))
			.SetTravelTime(Time.time - _hitData.travelTime);
		
		projectileHitEvent.Raise(_hitData);
	}

	#endregion
}