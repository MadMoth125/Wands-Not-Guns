using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.HealthSystem
{
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(Rigidbody))]
	public class KillZone : MonoBehaviour
	{
		private BoxCollider _collider;
		private Rigidbody _rigidbody;
		
		#region Unity Methods

		private void Awake()
		{
			_collider = GetComponent<BoxCollider>();
			_rigidbody = GetComponent<Rigidbody>();
		}

		private void Update()
		{
		
		}

		private void OnEnable()
		{
		
		}

		private void OnDisable()
		{
		
		}

		private void OnTriggerEnter(Collider other)
		{
			Debug.Log($"Entered {other.gameObject.name}");
			if (!other.gameObject.CompareTag("Enemy"))
			{
				Debug.Log($"Not an enemy: {other.gameObject.name}");
				return;
			}
			
			var health = other.gameObject.GetComponent<IDamageable>();
			if (health == null)
			{
				Debug.LogWarning($"No IDamageable component found on {other.gameObject.name}");
				return;
			}
			
			health.Kill();
			Debug.Log($"Killed {other.gameObject.name}");
		}
		
		private void OnTriggerExit(Collider other)
		{
			
		}

		#endregion
	}
}