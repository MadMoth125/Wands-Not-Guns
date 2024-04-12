using System;
using System.Collections.Generic;
using Mr_sB.UnityTimer;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Events;

namespace Weapons.Projectiles
{
	[Obsolete]
	[RequireComponent(typeof(SphereCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class ProjectileParticleHandler : MonoBehaviour
	{
		[ScriptableEventCategory]
		[Required]
		[SerializeField]
		private ScriptableEventCollision projectileHitEvent;
	
		[Tooltip("The layers to ignore when checking for collisions.")]
		[Required]
		public LayerMaskVariable ignoredLayers;
	
		[Tooltip("The particle effect to spawn when the projectile hits a target.")]
		public GameObject hitParticleEffect;
	
		[Tooltip("The particle effect to spawn when the projectile is fired.")]
		public GameObject flashParticleEffect;
	
		[Tooltip("Other particle effects that are already attached to the projectile, but act separately.")]
		public List<GameObject> otherParticleEffects = new List<GameObject>();
	
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
	
		private GameObject _hitParticleEffectInstance;
		private ParticleSystem _hitEffectParticleSystem;
	
		private GameObject _flashParticleEffectInstance;
		private ParticleSystem _flashEffectParticleSystem;
	
		// Stuff for parents and local positions of other particle effects.
		// Because they can/will be detached/re-attached to the projectile often.
		private Transform[] _otherParticleEffectParents;
		private Vector3[] _otherParticleEffectLocalPositions;
		private float _speedMultiplier = 1f;
	
		private List<DelayTimer> _delayTimers = new List<DelayTimer>();
		private DelayTimer _mainGameObjectTimer;
	
		private SphereCollider _collider;
		private Rigidbody _rigidbody;

		public void SudoInstantiate(GameObject target, Vector3 position, Quaternion rotation)
		{
			target.transform.position = position;
			target.transform.rotation = rotation;
			target.SetActive(true);
		}
	
		public void SudoInstantiateLocal(GameObject target, Vector3 position, Quaternion rotation)
		{
			target.transform.localPosition = position;
			target.transform.localRotation = rotation;
			target.SetActive(true);
		}
	
		public void SudoDestroy(GameObject target, float delay = 0f)
		{
			delay = Mathf.Clamp(delay, 0f, float.MaxValue);
			if (delay > 0)
			{
				_delayTimers.Add(Timer.DelayAction(delay, () => target.SetActive(false)));
			}
			else
			{
				target.SetActive(false);
			}
		}
	
		public void StopAllTimers()
		{
			if (_mainGameObjectTimer != null)
			{
				_mainGameObjectTimer.Cancel();
				_mainGameObjectTimer = null;
			}
		
			if (_delayTimers == null || _delayTimers.Count == 0) return;
		
			foreach (var timer in _delayTimers)
			{
				timer.Cancel();
			}
		
			_delayTimers.Clear();
		}

		public void MergeAllObjects()
		{
			if (IsFlashParticleEffectValid())
			{
				_flashParticleEffectInstance.transform.SetParent(transform);
			}

			if (IsHitParticleEffectValid())
			{
				_hitParticleEffectInstance.transform.SetParent(transform);
			}
		
			if (IsOtherParticleEffectsValid())
			{
				for (int i = 0; i < otherParticleEffects.Count; i++)
				{
					otherParticleEffects[i].transform.SetParent(_otherParticleEffectParents[i], true);
					otherParticleEffects[i].transform.position = _otherParticleEffectParents[i].position;
				}
			}
		}
	
		public void SeparateAllObjects()
		{
			if (IsFlashParticleEffectValid())
			{
				_flashParticleEffectInstance.transform.SetParent(null);
			}

			if (IsHitParticleEffectValid())
			{
				_hitParticleEffectInstance.transform.SetParent(null);
				_hitParticleEffectInstance.SetActive(false);
			}
		
			if (IsOtherParticleEffectsValid())
			{
				for (int i = 0; i < otherParticleEffects.Count; i++)
				{
					otherParticleEffects[i].transform.SetParent(_otherParticleEffectParents[i], true);
					otherParticleEffects[i].transform.position = _otherParticleEffectParents[i].position;
					otherParticleEffects[i].SetActive(false);
				}
			}
		}
	
		#region Unity Methods

		private void Awake()
		{
			_collider = GetComponent<SphereCollider>();
			_rigidbody = GetComponent<Rigidbody>();

			if (hitParticleEffect != null)
			{
				_hitParticleEffectInstance = Instantiate(hitParticleEffect, transform.position, transform.rotation);
				_hitParticleEffectInstance.SetActive(false);
				_hitEffectParticleSystem = _hitParticleEffectInstance.GetComponent<ParticleSystem>().OrNull() 
				                           ?? _hitParticleEffectInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
			}

			if (flashParticleEffect != null)
			{
				_flashParticleEffectInstance = Instantiate(flashParticleEffect, transform.position, transform.rotation);
				_flashParticleEffectInstance.transform.SetParent(transform);
				_flashParticleEffectInstance.SetActive(false);
				_flashEffectParticleSystem = _flashParticleEffectInstance.GetComponent<ParticleSystem>().OrNull() 
				                             ?? _flashParticleEffectInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
			}
		
			if (IsOtherParticleEffectsValid())
			{
				_otherParticleEffectParents = new Transform[otherParticleEffects.Count];
				_otherParticleEffectLocalPositions = new Vector3[otherParticleEffects.Count];
				for (int i = 0; i < otherParticleEffects.Count; i++)
				{
					_otherParticleEffectParents[i] = otherParticleEffects[i].transform.parent;
					_otherParticleEffectLocalPositions[i] = otherParticleEffects[i].transform.position;
					// otherParticleEffects[i].SetActive(false);
				}
			}
		}

		private void FixedUpdate()
		{
			if (isKinematic)
			{
				transform.position += transform.forward * (speed * _speedMultiplier * Time.fixedDeltaTime);
			}
			else
			{
				_rigidbody.velocity = transform.forward * (speed * _speedMultiplier);
			}
		}

		private void OnEnable()
		{
			StopAllTimers();

			_speedMultiplier = 1f;
			_collider.excludeLayers = ignoredLayers.Value;
			_rigidbody.isKinematic = isKinematic;
			if (!_rigidbody.isKinematic)
			{
				_rigidbody.constraints = RigidbodyConstraints.None;
				_rigidbody.useGravity = useGravity;
			}

		
			if (transform.childCount != 0)
			{
				foreach (Transform child in transform)
				{
					child.gameObject.SetActive(true);
				}
			}
		
			ResetOtherParticleEffects();
			SpawnFlashParticleEffect();
		
			// Hides the projectile after a certain amount of time.
			// This will fire if the projectile never collides with anything.
			_mainGameObjectTimer = Timer.DelayAction(5f, () =>
			{
				gameObject.SetActive(false);
				_mainGameObjectTimer = null;
			});
		}

		private void OnDestroy()
		{
			_mainGameObjectTimer?.Cancel();
			StopAllTimers();
			Destroy(_hitParticleEffectInstance);
			Destroy(_flashParticleEffectInstance);
			for (int i = 0; i < otherParticleEffects.Count; i++)
			{
				Destroy(otherParticleEffects[i]);
			}
		}

		private void OnCollisionEnter(Collision other)
		{
			_speedMultiplier = 0f;
			if (!_rigidbody.isKinematic) _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		
			SpawnHitParticleEffect(other);
			EndOtherParticleSystemsLifetimes();
		
			// if the initial lifetime timer is still active, cancel it.
			if (_mainGameObjectTimer != null)
			{
				_mainGameObjectTimer.Cancel();
				_mainGameObjectTimer = null;
			}
		
			gameObject.SetActive(false);
		}

		#endregion

		#region Particle Effect Reference Validation

		private bool IsHitParticleEffectValid()
		{
			return _hitParticleEffectInstance != null && _hitEffectParticleSystem != null;
		}
	
		private bool IsFlashParticleEffectValid()
		{
			return _flashParticleEffectInstance != null && _flashEffectParticleSystem != null;
		}
	
		private bool IsOtherParticleEffectsValid()
		{
			return otherParticleEffects is { Count: > 0 };
		}

		#endregion
	
		private void SpawnFlashParticleEffect()
		{
			if (IsFlashParticleEffectValid())
			{
				Vector3 position = transform.position;
				Quaternion rotation = transform.rotation;
				SudoInstantiate(_flashParticleEffectInstance, position, rotation);
				SudoDestroy(_flashParticleEffectInstance, _flashEffectParticleSystem.main.duration);
			}
		}

		private void SpawnHitParticleEffect(Collision other)
		{
			ContactPoint contact = other.contacts[0];
			Vector3 collisionPosition = contact.point + contact.normal * hitOffset;
		
			if (IsHitParticleEffectValid())
			{
				if (useFirePointRotation)
				{
					SudoInstantiate(
						_hitParticleEffectInstance,
						collisionPosition,
						gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0));
				}
				else if (rotationOffset != Vector3.zero)
				{
					SudoInstantiate(
						_hitParticleEffectInstance,
						collisionPosition,
						Quaternion.Euler(rotationOffset));
				}
				else
				{
					SudoInstantiate(
						_hitParticleEffectInstance,
						collisionPosition,
						Quaternion.LookRotation(contact.point + contact.normal));
				}
			
				SudoDestroy(_hitParticleEffectInstance, _hitEffectParticleSystem.main.duration);
			}
		}
	
		private void EndOtherParticleSystemsLifetimes()
		{
			if (IsOtherParticleEffectsValid())
			{
				for (int i = 0; i < otherParticleEffects.Count; i++)
				{
					otherParticleEffects[i].transform.SetParent(null, true);
					SudoDestroy(otherParticleEffects[i], 1f);
				}
			}
		}
	
		private void ResetOtherParticleEffects()
		{
			if (IsOtherParticleEffectsValid())
			{
				for (int i = 0; i < otherParticleEffects.Count; i++)
				{
					otherParticleEffects[i].transform.SetParent(_otherParticleEffectParents[i], true);
					otherParticleEffects[i].transform.position = _otherParticleEffectParents[i].position;
					otherParticleEffects[i].SetActive(true);
				}
			}
		}
	}
}