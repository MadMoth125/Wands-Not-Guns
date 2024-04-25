using System;
using CodeMonkey.HealthSystemCM;
using Core.Owning;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.HealthSystem
{
	/// <summary>
	/// A custom health component that wraps the <see cref="HealthSystem"/> class into a MonoBehaviour.
	/// Adds additional data and events to the underlying health system for more robust functionality.
	/// </summary>
	public class HealthComponent : MonoBehaviour, IHealthProperties, IOwnable<GameObject>
	{
		#region Events

		public event Action<HealthChangedArgs> OnHealthChanged;
		public event Action<HealthChangedArgs> OnMaxHealthChanged;
		public event Action<HealthChangedArgs, Object> OnDamaged;
		public event Action<HealthChangedArgs, Object> OnHealed;
		public event Action OnDie;

		#endregion

		#region Properties

		/// <summary>
		/// The current health of the <see cref="HealthComponent"/>.
		/// Get/Set works the same as calling <see cref="GetHealth"/> and <see cref="SetHealth"/> respectively.
		/// </summary>
		public float Health
		{
			get => GetHealth();
			set => SetHealth(value);
		}

		/// <summary>
		/// The maximum health of the <see cref="HealthComponent"/>.
		/// Get works the same as calling <see cref="GetMaxHealth"/>. However, Set will not refill the health to full.
		/// </summary>
		public float MaxHealth
		{
			get => GetMaxHealth();
			set => SetMaxHealth(value, false);
		}

		/// <summary>
		/// The current health of the <see cref="HealthComponent"/> as a normalized value (0-1).
		/// Get works the same as calling <see cref="GetHealthNormalized"/>.
		/// </summary>
		public float HealthNormalized => GetHealthNormalized();
		
		/// <summary>
		/// Whether the <see cref="HealthComponent"/> is dead.
		/// Get works the same as calling <see cref="IsDead"/>.
		/// </summary>
		public bool Dead => IsDead();

		#endregion

		[Tooltip("Maximum Health amount")]
		[SerializeField]
		private float healthAmountMax = 150f;

		[Tooltip("Starting Health amount, leave at 0 to start at full health.")]
		[SerializeField]
		private float startingHealthAmount = 0f;
	
		private CodeMonkey.HealthSystemCM.HealthSystem _internalHealthSystem;
		private GameObject _owner;
		private float _previousHealth;
		private float _previousMaxHealth;
		private Object _lastDamageSource;
		private Object _lastHealSource;
		private HealthChangedArgs _cachedHealthChangedArgs;

		#region Static Methods

		/// <summary>
		/// Finds the <see cref="IHealthProperties"/> component on the target GameObject or its children.
		/// </summary>
		/// <param name="target">The GameObject to search for the <see cref="IHealthProperties"/> component.</param>
		/// <param name="healthComponent">The found <see cref="IHealthProperties"/> component.</param>
		/// <returns>If the <see cref="IHealthProperties"/> component was found.</returns>
		public static bool FindHealthProperties(GameObject target, out IHealthProperties healthComponent)
		{
			healthComponent = null;
			healthComponent = target.GetComponent<IHealthProperties>() ?? target.GetComponentInChildren<IHealthProperties>();
			return healthComponent != null;
		}
		
		/// <summary>
		/// Finds the <see cref="HealthComponent"/> component on the target GameObject or its children.
		/// </summary>
		/// <param name="target">The GameObject to search for the <see cref="HealthComponent"/> component.</param>
		/// <param name="healthComponent">The found <see cref="HealthComponent"/> component.</param>
		/// <returns>If the <see cref="HealthComponent"/> component was found.</returns>
		public static bool FindHealthProperties(GameObject target, out HealthComponent healthComponent)
		{
			healthComponent = null;
			healthComponent = target.GetComponent<HealthComponent>() ?? target.GetComponentInChildren<HealthComponent>();
			return healthComponent != null;
		}

		#endregion

		#region Owning Methods

		public GameObject GetOwner()
		{
			return _owner;
		}

		public void SetOwner(GameObject owner)
		{
			_owner = owner;
		}

		#endregion
		
		#region Getters and Setters

		public virtual float GetHealth() => _internalHealthSystem.GetHealth();

		public virtual float GetMaxHealth() => _internalHealthSystem.GetHealthMax();

		public virtual void SetHealth(float health)
		{
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.SetHealth(health);
			OnHealthChangedInvoke(_internalHealthSystem.GetHealth(), _previousHealth);
			
			if (_internalHealthSystem.IsDead())
			{
				OnDieInvoke();
			}
		}

		public virtual void SetMaxHealth(float healthMax, bool fullHealth)
		{
			_previousMaxHealth = _internalHealthSystem.GetHealthMax();
			_internalHealthSystem.SetHealthMax(healthMax, fullHealth);
			OnMaxHealthChangedInvoke(_internalHealthSystem.GetHealthMax(), _previousMaxHealth);
		}

		public virtual float GetHealthNormalized() => _internalHealthSystem.GetHealthNormalized();

		public virtual bool IsDead() => _internalHealthSystem.IsDead();

		public Object GetLastDamageSource() => _lastDamageSource;
		
		public Object GetLastHealSource() => _lastHealSource;
		
		#endregion

		#region Public Methods
		
		public virtual void Damage(float damageAmount, Object source)
		{
			if (_internalHealthSystem.IsDead()) return;
			
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.Damage(damageAmount);
			OnDamagedInvoke(_internalHealthSystem.GetHealth(), _previousHealth, source);
			OnHealthChangedInvoke(_internalHealthSystem.GetHealth(), _previousHealth);
			
			if (_internalHealthSystem.IsDead())
			{
				OnDieInvoke();
			}
		}

		public void Kill(Object source)
		{
			if (_internalHealthSystem.IsDead()) return;
			
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.Damage(_internalHealthSystem.GetHealth());
			OnDamagedInvoke(_internalHealthSystem.GetHealth(), _previousHealth, source);
			OnHealthChangedInvoke(_internalHealthSystem.GetHealth(), _previousHealth);
			
			if (_internalHealthSystem.IsDead())
			{
				OnDieInvoke();
			}
		}

		public virtual void Heal(float healAmount, Object source)
		{
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.Heal(healAmount);
			OnHealedInvoke(_internalHealthSystem.GetHealth(), _previousHealth, source);
		}

		public virtual void HealComplete(Object source)
		{
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.HealComplete();
			OnHealedInvoke(_internalHealthSystem.GetHealth(), _previousHealth, source);
		}

		public virtual void Die()
		{
			_internalHealthSystem.Die();
			OnDieInvoke();
		}

		#endregion
		
		#region Unity Methods

		private void Awake()
		{
			_internalHealthSystem = new CodeMonkey.HealthSystemCM.HealthSystem(healthAmountMax);
		
			if (startingHealthAmount != 0) 
			{
				_internalHealthSystem.SetHealth(startingHealthAmount);
			}

			AfterAwake();
		}

		/// <summary>
		/// Called after the <see cref="Awake"/> method.
		/// </summary>
		protected virtual void AfterAwake()
		{
			
		}
		
		#endregion

		#region Callbacks

		/// <summary>
		/// Method is called when <see cref="OnHealthChanged"/> is invoked.
		/// </summary>
		/// <param name="args">The <see cref="HealthChangedArgs"/> args.</param>
		protected virtual void OnHealthChangedCallback(HealthChangedArgs args)
		{
			
		}
		
		/// <summary>
		/// Method is called when <see cref="OnMaxHealthChanged"/> is invoked.
		/// </summary>
		/// <param name="args">The <see cref="HealthChangedArgs"/> args.</param>
		protected virtual void OnMaxHealthChangedCallback(HealthChangedArgs args)
		{
			
		}
		
		/// <summary>
		/// Method is called when <see cref="OnHealed"/> is invoked.
		/// </summary>
		/// <param name="args">The <see cref="HealthChangedArgs"/> args.</param>
		/// <param name="source">The source of the healing.</param>
		protected virtual void OnHealedCallback(HealthChangedArgs args, Object source)
		{
			
		}
		
		/// <summary>
		/// Method is called when <see cref="OnDamaged"/> is invoked.
		/// </summary>
		/// <param name="args">The <see cref="HealthChangedArgs"/> args.</param>
		/// <param name="source">The source of the damage.</param>
		protected virtual void OnDamagedCallback(HealthChangedArgs args, Object source)
		{
			
		}
		
		/// <summary>
		/// Method is called when <see cref="OnDie"/> is invoked.
		/// </summary>
		protected virtual void OnDieCallback()
		{
			
		}

		#endregion
		
		#region Event Invokers

		private void OnHealthChangedInvoke(float current, float previous)
		{
			OnHealthChangedCallback(HealthChangedArgs.Make(current, previous, ref _cachedHealthChangedArgs));
			OnHealthChanged?.Invoke(HealthChangedArgs.Make(current, previous, ref _cachedHealthChangedArgs));
		}

		private void OnMaxHealthChangedInvoke(float current, float previous)
		{
			OnMaxHealthChangedCallback(HealthChangedArgs.Make(current, previous, ref _cachedHealthChangedArgs));
			OnMaxHealthChanged?.Invoke(HealthChangedArgs.Make(current, previous, ref _cachedHealthChangedArgs));
		}

		private void OnHealedInvoke(float current, float previous, Object source)
		{
			_lastHealSource = source;
			OnHealedCallback(HealthChangedArgs.Make(current, previous, ref _cachedHealthChangedArgs), source);
			OnHealed?.Invoke(HealthChangedArgs.Make(current, previous, ref _cachedHealthChangedArgs), source);
		}

		private void OnDamagedInvoke(float current, float previous, Object source)
		{
			_lastDamageSource = source;
			OnDamagedCallback(HealthChangedArgs.Make(current, previous, ref _cachedHealthChangedArgs), source);
			OnDamaged?.Invoke(HealthChangedArgs.Make(current, previous, ref _cachedHealthChangedArgs), source);
		}

		private void OnDieInvoke()
		{
			OnDieCallback();
			OnDie?.Invoke();
		}

		#endregion
	}
}