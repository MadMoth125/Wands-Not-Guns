using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
		public UnityEvent<HealthChangedArgs> onHealthChanged = new();

		[UnityEventsCategory]
		public UnityEvent<HealthChangedArgs> onMaxHealthChanged = new();

		[UnityEventsCategory]
		public UnityEvent<HealthChangedArgs, Object> onDamaged = new();

		[UnityEventsCategory]
		public UnityEvent<HealthChangedArgs, Object> onHealed = new();

		[UnityEventsCategory]
		public UnityEvent onDie = new();
		
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
			
			_healthComponent.OnHealthChanged += onHealthChanged.Invoke;
			_healthComponent.OnMaxHealthChanged += onMaxHealthChanged.Invoke;
			_healthComponent.OnDamaged += onDamaged.Invoke;
			_healthComponent.OnHealed += onHealed.Invoke;
			_healthComponent.OnDie += onDie.Invoke;
		}

		private void OnDisable()
		{
			if (!CanValidateHealthComponent()) return;
			
			_healthComponent.OnHealthChanged -= onHealthChanged.Invoke;
			_healthComponent.OnMaxHealthChanged -= onMaxHealthChanged.Invoke;
			_healthComponent.OnDamaged -= onDamaged.Invoke;
			_healthComponent.OnHealed -= onHealed.Invoke;
			_healthComponent.OnDie -= onDie.Invoke;
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