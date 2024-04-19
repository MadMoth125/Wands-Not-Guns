using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Core.HealthSystem
{
	/// <summary>
	/// Class that listens to the events of a <see cref="HealthComponent"/>
	/// and invokes UnityEvents for each event.
	/// </summary>
	[RequireComponent(typeof(HealthComponent))]
	public class HealthComponentEvents : MonoBehaviour
	{
		#region Events
		
		[UnityEventsCategory]
		public UnityEvent<HealthChangedArgs> OnHealthChanged = new();

		[UnityEventsCategory]
		public UnityEvent<HealthChangedArgs> OnMaxHealthChanged = new();

		[UnityEventsCategory]
		public UnityEvent<HealthChangedArgs> OnDamaged = new();

		[UnityEventsCategory]
		public UnityEvent<HealthChangedArgs> OnHealed = new();

		[UnityEventsCategory]
		public UnityEvent OnDie = new();
		
		#endregion
		
		private HealthComponent _healthComponent;
		
		#region Unity Methods

		private void OnValidate()
		{
			if (_healthComponent == null)
			{
				_healthComponent = GetComponent<HealthComponent>();
			}
		}

		private void Awake()
		{
			if (_healthComponent == null)
			{
				_healthComponent = GetComponent<HealthComponent>();
			}
		}

		private void OnEnable()
		{
			if (!CanValidateHealthComponent()) return;
			
			_healthComponent.OnHealthChanged += OnHealthChanged.Invoke;
			_healthComponent.OnMaxHealthChanged += OnMaxHealthChanged.Invoke;
			_healthComponent.OnDamaged += OnDamaged.Invoke;
			_healthComponent.OnHealed += OnHealed.Invoke;
			_healthComponent.OnDie += OnDie.Invoke;
		}

		private void OnDisable()
		{
			if (!CanValidateHealthComponent()) return;
			
			_healthComponent.OnHealthChanged -= OnHealthChanged.Invoke;
			_healthComponent.OnMaxHealthChanged -= OnMaxHealthChanged.Invoke;
			_healthComponent.OnDamaged -= OnDamaged.Invoke;
			_healthComponent.OnHealed -= OnHealed.Invoke;
			_healthComponent.OnDie -= OnDie.Invoke;
		}

		#endregion
		
		private bool CanValidateHealthComponent()
		{
			if (_healthComponent == null)
			{
				_healthComponent = GetComponent<HealthComponent>();
			}
			
			return _healthComponent != null;
		}
	}
}