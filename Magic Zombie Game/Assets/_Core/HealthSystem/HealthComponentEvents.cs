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
		
		public UnityEvent<HealthChangedArgs> OnHealthChanged;

		public UnityEvent<HealthChangedArgs> OnMaxHealthChanged;

		public UnityEvent<HealthChangedArgs> OnDamaged;

		public UnityEvent<HealthChangedArgs> OnHealed;

		public UnityEvent OnDie;
		
		#endregion
		
		[HideInInspector]
		[SerializeField]
		private HealthComponent healthComponent;
		
		#region Unity Methods

		private void OnValidate()
		{
			if (healthComponent == null)
			{
				healthComponent = GetComponent<HealthComponent>();
			}
		}

		private void Awake()
		{
			if (healthComponent == null)
			{
				healthComponent = GetComponent<HealthComponent>();
			}
		}

		private void OnEnable()
		{
			if (!CanValidateHealthComponent()) return;
			
			healthComponent.OnHealthChanged += OnHealthChanged.Invoke;
			healthComponent.OnMaxHealthChanged += OnMaxHealthChanged.Invoke;
			healthComponent.OnDamaged += OnDamaged.Invoke;
			healthComponent.OnHealed += OnHealed.Invoke;
			healthComponent.OnDie += OnDie.Invoke;
		}

		private void OnDisable()
		{
			if (!CanValidateHealthComponent()) return;
			
			healthComponent.OnHealthChanged -= OnHealthChanged.Invoke;
			healthComponent.OnMaxHealthChanged -= OnMaxHealthChanged.Invoke;
			healthComponent.OnDamaged -= OnDamaged.Invoke;
			healthComponent.OnHealed -= OnHealed.Invoke;
			healthComponent.OnDie -= OnDie.Invoke;
		}

		#endregion
		
		private bool CanValidateHealthComponent()
		{
			if (healthComponent == null)
			{
				healthComponent = GetComponent<HealthComponent>();
			}
			
			return healthComponent != null;
		}
	}
}