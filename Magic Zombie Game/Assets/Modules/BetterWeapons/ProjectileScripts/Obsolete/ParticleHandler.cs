using System;
using System.Collections.Generic;
using Core.ObjectPool;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons.Events;

namespace Weapons.Projectiles
{
	/// <summary>
	/// A copy of the 'HS_ProjectileMover' script from the 'AAA Projectiles' pack, but
	/// uses the ObjectPool system for projectile management.
	///
	///	Instead on instantiating and destroying projectiles, this script handles the enabling and disabling of particles
	/// to simulate the same effect. This is done to reduce the overhead of instantiating and destroying objects.
	/// 
	/// Works only with the structure of Hovl Studio's 'AAA Projectiles' prefab/particle systems.
	/// Examine the prefab to understand the structure of the particle systems. 
	/// </summary>
	[Obsolete]
	[RequireComponent(typeof(ParticleTimerHandler))]
	[DisallowMultipleComponent]
	public class ParticleHandler : MonoBehaviour, IPoolableObject
	{
		#region Fields

		[ScriptableEventCategory]
		[Required]
		[SerializeField]
		private ScriptableEventCollision projectileHitEvent;
	
		[Title("Projectile Components")]
		[Tooltip("The layers to ignore when checking for collisions.")]
		[Required]
		public LayerMaskVariable ignoredLayers;
	
		[Tooltip("The particle effect to spawn when the projectile hits a target.")]
		public GameObject hitParticleEffect;
	
		[Tooltip("The particle effect to spawn when the projectile is fired.")]
		public GameObject flashParticleEffect;
	
		[Tooltip("Other particle effects that are already attached to the projectile, but act separately.")]
		public List<GameObject> otherParticleEffects = new List<GameObject>();
	
		[Title("Parameters")]
		[Tooltip("The speed at which the projectile will move.")]
		public float speed = 15f;
	
		[Tooltip("The offset from the hit point where the hit particle effect will spawn.")]
		public float hitOffset = 0f;
	
		public bool useFirePointRotation = false;
	
		[DisableIf(nameof(useFirePointRotation))]
		[Tooltip("Only used when <b>useFirePointRotation</b> is disabled.")]
		public Vector3 rotationOffset = new Vector3(0, 0, 0);
	
		[Tooltip("Disable if the projectile needs to be affected by physics.")]
		public bool isKinematic = false;
	
		[DisableIf(nameof(isKinematic))]
		[Tooltip("Enable gravity for the projectile.\n" +
		         "Only works if <b>isKinematic</b> is disabled.")]
		public bool useGravity = false;

		#endregion
	
		private ParticleSystem _hitEffectParticleSystem;
		private ParticleSystem _flashEffectParticleSystem;
		// Stuff for parents and local positions of other particle effects.
		// Because they can/will be detached/re-attached to the projectile often.
		private GameObjectStructureData _otherParticleEffectData;
		private ProjectileHitData _cachedProjectileData;
	
		private float _speedMultiplier = 1f;
		private float _startTime;
		private Transform _cachedTransform;
	
		private Collider _collider; // collision shape for projectile
		private Rigidbody _rigidbody;
		private ParticleTimerHandler _timerHandler; // handles timers
		private Collision _collision; // result from hit

		#region Sudo Create/Destroy Methods

		public void SudoInstantiate(GameObject target, Vector3 position, Quaternion rotation, Transform parent)
		{
			target.transform.SetParent(parent, true);
			target.transform.position = position;
			target.transform.rotation = rotation;
			target.SetActive(true);
		}
	
		public void SudoDestroy(GameObject target, float delay = 0f)
		{
			delay = Mathf.Clamp(delay, 0f, float.MaxValue);
			if (delay > 0)
			{
				_timerHandler.DelayAction(delay, DisableTarget);
				return;
			}
		
			DisableTarget();

			return;

			void DisableTarget()
			{
				target.SetActive(false);
			}
		}

		#endregion

		public void OverrideTransform(Transform reference)
		{
			_cachedTransform = reference;
		}

		public void HandleRelease(Action onComplete)
		{
			_timerHandler.CheckForAllTimersComplete(1f, onComplete);
		}
	
		public void MovementUpdate()
		{
			if (isKinematic)
			{
				transform.position += transform.forward * (speed * _speedMultiplier * Time.fixedDeltaTime);
			}
			else
			{
				if (_rigidbody != null && _cachedTransform != null)
				{
					_rigidbody.velocity = _cachedTransform.forward * (speed * _speedMultiplier);
				}
			}
		}
	
		#region Unity Methods

		private void Awake()
		{
			Vector3 position = _cachedTransform.OrNull() ? _cachedTransform.position : transform.position;
			Quaternion rotation = _cachedTransform.OrNull() ? _cachedTransform.rotation : transform.rotation;
		
			_collider = GetComponent<Collider>();
			_rigidbody = this.GetOrAddComponent<Rigidbody>();
			_timerHandler = this.GetOrAddComponent<ParticleTimerHandler>();

			if (_collider != null)
			{
				_collider.excludeLayers = ignoredLayers.Value;
			}
		
			if (_rigidbody != null)
			{
				_rigidbody.isKinematic = isKinematic;
				_rigidbody.useGravity = useGravity;
			}
		
			_otherParticleEffectData = new GameObjectStructureData(otherParticleEffects);
			_cachedProjectileData = new ProjectileHitData();
		
			if (flashParticleEffect != null)
			{
				var flashInstance = Instantiate(flashParticleEffect, position, rotation);
				flashInstance.SetActive(false);
				_flashEffectParticleSystem = flashInstance.GetComponent<ParticleSystem>().OrNull() 
				                             ?? flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
			}
		
			if (hitParticleEffect != null)
			{
				var hitInstance = Instantiate(hitParticleEffect, position, rotation);
				hitInstance.SetActive(false);
				_hitEffectParticleSystem = hitInstance.GetComponent<ParticleSystem>().OrNull() 
				                           ?? hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
			}
		}

		private void OnEnable()
		{
			_cachedProjectileData
				.SetStartPosition(_cachedTransform.OrNull() ? _cachedTransform.position : transform.position)
				.SetTravelTime(Time.time);
		
			CleanupState();
			LaunchState();
		}

		private void OnCollisionEnter(Collision other)
		{
			_collision = other;
			HitState();
		
			_cachedProjectileData
				.SetEndPosition(other.GetContact(0).point)
				.SetTravelDistance(Vector3.Distance(_cachedProjectileData.startPosition, _cachedProjectileData.endPosition))
				.SetTravelTime(Time.time - _cachedProjectileData.travelTime);
		
			projectileHitEvent.Raise(_cachedProjectileData);
		}

		#endregion

		#region State Methods

		private void CleanupState()
		{
			_speedMultiplier = 1f;
		
			_collision = null;
		
			if (_rigidbody != null)
			{
				_rigidbody.velocity = Vector3.zero;
				_rigidbody.constraints = RigidbodyConstraints.None;
				_rigidbody.isKinematic = isKinematic;
				_rigidbody.useGravity = useGravity;
			}

			if (_collider != null)
			{
				_collider.excludeLayers = ignoredLayers.Value;
			}
		
			_timerHandler.OrNull()?.CancelAllTimers();
		}

		private void LaunchState()
		{
			Vector3 position = _cachedTransform.OrNull() ? _cachedTransform.position : transform.position;
			Quaternion rotation = _cachedTransform.OrNull() ? _cachedTransform.rotation : transform.rotation;

			if (otherParticleEffects.IsValid())
			{
				for (int i = 0; i < otherParticleEffects.Count; i++)
				{
					otherParticleEffects[i].SetActive(false);
					otherParticleEffects[i].transform.position = position;
				}
			}

			_otherParticleEffectData.RetrieveGameObjectState(otherParticleEffects);

			if (_flashEffectParticleSystem != null)
			{
				_flashEffectParticleSystem.transform.SetParent(_cachedTransform.OrNull() ?? transform, true);
				_flashEffectParticleSystem.transform.SetPositionAndRotation(position, rotation);
				_flashEffectParticleSystem.gameObject.SetActive(true);
			}

			if (_hitEffectParticleSystem != null)
			{
				_hitEffectParticleSystem.gameObject.SetActive(false);
				_hitEffectParticleSystem.transform.SetParent(_cachedTransform.OrNull() ?? transform, true);
				_hitEffectParticleSystem.transform.SetPositionAndRotation(position, rotation);
			}
		
			_timerHandler.OrNull()?.StartMainParticleTimer(4f, () => gameObject.SetActive(false));
		}

		private void HitState()
		{
			if (_rigidbody != null)
			{
				_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				_speedMultiplier = 0f;
			}

			ContactPoint contact = default;
			Vector3 pos = default;
			if (_collision != null)
			{
				contact = _collision.GetContact(0);
				pos = contact.point + contact.normal * hitOffset;
			}
		
			if (_hitEffectParticleSystem != null && _cachedTransform != null)
			{
				if (useFirePointRotation)
				{
					SudoInstantiate(_hitEffectParticleSystem.gameObject, pos, _cachedTransform.rotation * Quaternion.Euler(0, 180f, 0), null);
				}
				else if (rotationOffset != Vector3.zero)
				{
					SudoInstantiate(_hitEffectParticleSystem.gameObject, pos, Quaternion.Euler(rotationOffset), null);
				}
				else
				{
					SudoInstantiate(_hitEffectParticleSystem.gameObject, pos, Quaternion.LookRotation(contact.point + contact.normal), null);
				}

				SudoDestroy(_hitEffectParticleSystem.gameObject, _hitEffectParticleSystem.main.duration);
			}

			if (otherParticleEffects.IsValid())
			{
				for (int i = 0; i < otherParticleEffects.Count; i++)
				{
					otherParticleEffects[i].transform.SetParent(null, true);
					SudoDestroy(otherParticleEffects[i], 4f);
				}
			}
		
			_timerHandler.OrNull()?.StopMainParticleTimer();
			SudoDestroy(gameObject);
		}

		#endregion

		public void OnGet()
		{
		
		}

		public void OnRelease()
		{
			// re-attach the particle effects to the projectile to clean-up the hierarchy
			if (_flashEffectParticleSystem != null)
			{
				_flashEffectParticleSystem.transform.SetParent(transform, true);
			}
		
			if (_hitEffectParticleSystem != null)
			{
				_hitEffectParticleSystem.transform.SetParent(transform, true);
			}
		
			if (otherParticleEffects.IsValid())
			{
				_otherParticleEffectData.RetrieveGameObjectState(otherParticleEffects);
			}
		}
	}
}