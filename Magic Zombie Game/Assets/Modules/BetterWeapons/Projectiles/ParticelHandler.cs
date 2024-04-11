using System;
using System.Collections.Generic;
using System.Linq;
using Core.CustomTickSystem;
using Mr_sB.UnityTimer;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityHFSM;
using Timer = Mr_sB.UnityTimer.Timer;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class ParticelHandler : MonoBehaviour
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
	
	// private GameObject _hitParticleEffectInstance;
	private ParticleSystem _hitEffectParticleSystem;
	
	// private GameObject _flashParticleEffectInstance;
	private ParticleSystem _flashEffectParticleSystem;
	
	// Stuff for parents and local positions of other particle effects.
	// Because they can/will be detached/re-attached to the projectile often.
	private GameObjectStructureData _otherParticleEffectData;
	private float _speedMultiplier = 1f;
	
	private List<DelayTimer> _delayTimers = new List<DelayTimer>();
	private DelayTimer _mainGameObjectTimer;
	
	private SphereCollider _collider;
	private Rigidbody _rigidbody;
	private StateMachine _fsm;
	private Collision _collision;
	private Transform _cachedTransform;
	
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
			_delayTimers.Add(Timer.DelayAction(delay, () => target.SetActive(false)));
		}
		else
		{
			target.SetActive(false);
		}
	}

	public void SetReferencedTransform(Transform reference)
	{
		_cachedTransform = reference;
	}
	
	public void MovementUpdate()
	{
		if (isKinematic)
		{
			transform.position += transform.forward * (speed * _speedMultiplier * Time.fixedDeltaTime);
		}
		else
		{
			if (_rigidbody != null)
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
		
		_collider = GetComponent<SphereCollider>();
		_rigidbody = GetComponent<Rigidbody>();

		_collider.excludeLayers = ignoredLayers.Value;
		_rigidbody.isKinematic = isKinematic;
		_rigidbody.useGravity = useGravity;
		
		_otherParticleEffectData = new GameObjectStructureData(otherParticleEffects);

		if (flashParticleEffect != null)
		{
			var flashInstance = Instantiate(flashParticleEffect, position, rotation);
			flashInstance.SetActive(false);
			_flashEffectParticleSystem = flashInstance.GetComponent<ParticleSystem>().OrNull() 
			                             ?? flashInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
			/*_flashParticleEffectInstance = Instantiate(flashParticleEffect, position, rotation);
			_flashParticleEffectInstance.transform.SetParent(_cachedTransform.OrNull() ?? transform, true);
			_flashParticleEffectInstance.SetActive(false);
			_flashEffectParticleSystem = _flashParticleEffectInstance.GetComponent<ParticleSystem>().OrNull() 
			                             ?? _flashParticleEffectInstance.transform.GetChild(0).GetComponent<ParticleSystem>();*/
		}
		
		if (hitParticleEffect != null)
		{
			var hitInstance = Instantiate(hitParticleEffect, position, rotation);
			hitInstance.SetActive(false);
			_hitEffectParticleSystem = hitInstance.GetComponent<ParticleSystem>().OrNull() 
			                           ?? hitInstance.transform.GetChild(0).GetComponent<ParticleSystem>();
			/*_hitParticleEffectInstance = Instantiate(hitParticleEffect, position, rotation);
			_hitParticleEffectInstance.transform.SetParent(_cachedTransform.OrNull() ?? transform, true);
			_hitParticleEffectInstance.SetActive(false);
			_hitEffectParticleSystem = _hitParticleEffectInstance.GetComponent<ParticleSystem>().OrNull() 
			                           ?? _hitParticleEffectInstance.transform.GetChild(0).GetComponent<ParticleSystem>();*/
		}
	}

	private void OnEnable()
	{
		CleanupState();
		LaunchState();
	}

	private void OnCollisionEnter(Collision other)
	{
		_collision = other;
		projectileHitEvent.Raise(new ProjectileHitData(gameObject, other));
		HitState();
	}

	private void OnDisable()
	{
		// CleanupState();
	}

	#endregion

	#region State Methods

	private void CleanupState(State<string, string> state = null)
	{
		Debug.Log("Cleanup State");
		
		_speedMultiplier = 1f;
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.constraints = RigidbodyConstraints.None;
		_rigidbody.isKinematic = isKinematic;
		_rigidbody.useGravity = useGravity;
		_collider.excludeLayers = ignoredLayers.Value;
		
		StopAllTimers();
	}

	private void LaunchState(State<string, string> state = null)
	{
		Vector3 position = _cachedTransform.OrNull() ? _cachedTransform.position : transform.position;
		Quaternion rotation = _cachedTransform.OrNull() ? _cachedTransform.rotation : transform.rotation;
		
		Debug.Log("Launch State");
		foreach (var other in otherParticleEffects)
		{
			other.SetActive(false);
			other.transform.position = position;
		}
		_otherParticleEffectData.RetrieveGameObjectState(otherParticleEffects);
		
		_flashEffectParticleSystem.transform.SetParent(_cachedTransform.OrNull() ?? transform, true);
		_flashEffectParticleSystem.transform.SetPositionAndRotation(position, rotation);
		_flashEffectParticleSystem.gameObject.SetActive(true);
		
		_hitEffectParticleSystem.gameObject.SetActive(false);
		_hitEffectParticleSystem.transform.SetParent(_cachedTransform.OrNull() ?? transform, true);
		_hitEffectParticleSystem.transform.SetPositionAndRotation(position, rotation);
		
		_mainGameObjectTimer ??= Timer.DelayAction(5f, () =>
		{
			SudoDestroy(gameObject);
		});
	}

	private void HitState(State<string, string> state = null)
	{
		Debug.Log("Hit State");
		
		_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		_speedMultiplier = 0f;
		
		ContactPoint contact = _collision.contacts[0];
		Vector3 pos = contact.point + contact.normal * hitOffset;
		
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

		for (int i = 0; i < otherParticleEffects.Count; i++)
		{
			otherParticleEffects[i].transform.SetParent(null, true);
			SudoDestroy(otherParticleEffects[i], 5f);
		}
		
		if (_mainGameObjectTimer != null)
		{
			_mainGameObjectTimer?.Cancel();
			_mainGameObjectTimer = null;
		}
		
		SudoDestroy(gameObject);
	}

	#endregion
	
	private void StopAllTimers()
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
}