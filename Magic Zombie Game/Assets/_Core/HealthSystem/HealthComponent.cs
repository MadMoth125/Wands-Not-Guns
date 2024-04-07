using System;
using CodeMonkey.HealthSystemCM;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.HealthSystem
{
	/// <summary>
	/// A custom health component that wraps the <see cref="HealthSystem"/> class into a MonoBehaviour.
	/// Adds additional data and events to the underlying health system for more robust functionality.
	/// </summary>
	public class HealthComponent : MonoBehaviour, IHealthProperties
	{
		#region Events

		public event Action<HealthChangedArgs> OnHealthChanged;
		
		public event Action<HealthChangedArgs> OnMaxHealthChanged;
		
		public event Action<HealthChangedArgs> OnDamaged;
		
		public event Action<HealthChangedArgs> OnHealed;
		
		public event Action OnDie;

		#endregion

		#region Properties

		/// <summary>
		/// The GameObject that owns this <see cref="HealthComponent"/>.
		/// Get/Set works the same as calling <see cref="GetOwningGameObject"/> and <see cref="SetOwner"/> respectively.
		/// </summary>
		public Object Owner
		{
			get => GetOwner();
			set => SetOwner(value);
		}

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
		private Object _owner;
		private float _previousHealth;
		private float _previousMaxHealth;

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
		
		#region Getters and Setters
		
		public Object GetOwner() => _owner;

		public void SetOwner(Object owner) => _owner = owner;

		public virtual float GetHealth() => _internalHealthSystem.GetHealth();

		public virtual float GetMaxHealth() => _internalHealthSystem.GetHealthMax();

		public virtual void SetHealth(float health)
		{
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.SetHealth(health);
		}

		public virtual void SetMaxHealth(float healthMax, bool fullHealth)
		{
			_previousMaxHealth = _internalHealthSystem.GetHealthMax();
			_internalHealthSystem.SetHealthMax(healthMax, fullHealth);
		}

		public virtual float GetHealthNormalized() => _internalHealthSystem.GetHealthNormalized();

		public virtual bool IsDead() => _internalHealthSystem.IsDead();

		#endregion

		#region Public Methods
		
		public virtual void Damage(float damageAmount)
		{
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.Damage(damageAmount);
		}

		public void Kill()
		{
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.SetHealth(0f);
		}

		public virtual void Heal(float healAmount)
		{
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.Heal(healAmount);
		}

		public virtual void HealComplete()
		{
			_previousHealth = _internalHealthSystem.GetHealth();
			_internalHealthSystem.HealComplete();
		}

		public virtual void Die() => _internalHealthSystem.Die();

		#endregion
		
		#region Unity Methods

		private void Awake()
		{
			_internalHealthSystem = new CodeMonkey.HealthSystemCM.HealthSystem(healthAmountMax);
		
			if (startingHealthAmount != 0) 
			{
				_internalHealthSystem.SetHealth(startingHealthAmount);
			}
			
			// SetOwner(gameObject);
			
			AfterAwake();
		}

		/// <summary>
		/// Called after the <see cref="Awake"/> method.
		/// </summary>
		protected virtual void AfterAwake()
		{
			
		}

		private void OnEnable()
		{
			_internalHealthSystem.OnHealthChanged += HealthChanged;
			OnHealthChanged += HealthChangedInternal;
			
			_internalHealthSystem.OnHealthMaxChanged += MaxHealthChanged;
			OnMaxHealthChanged += MaxHealthChangedInternal;
			
			_internalHealthSystem.OnHealed += Healed;
			OnHealed += HealedInternal;
			
			_internalHealthSystem.OnDamaged += Damaged;
			OnDamaged += DamagedInternal;
			
			_internalHealthSystem.OnDead += Died;
			OnDie += DiedInternal;
			
			AfterEnable();
		}

		/// <summary>
		/// Called after the <see cref="OnEnable"/> method.
		/// </summary>
		protected virtual void AfterEnable()
		{
			
		}

		private void OnDisable()
		{
			_internalHealthSystem.OnHealthChanged -= HealthChanged;
			OnHealthChanged -= HealthChangedInternal;
			
			_internalHealthSystem.OnHealthMaxChanged -= MaxHealthChanged;
			OnMaxHealthChanged -= MaxHealthChangedInternal;
			
			_internalHealthSystem.OnHealed -= Healed;
			OnHealed -= HealedInternal;
			
			_internalHealthSystem.OnDamaged -= Damaged;
			OnDamaged -= DamagedInternal;
			
			_internalHealthSystem.OnDead -= Died;
			OnDie -= DiedInternal;
			
			AfterDisable();
		}

		/// <summary>
		/// Called after the <see cref="OnDisable"/> method.
		/// </summary>
		protected virtual void AfterDisable()
		{
			
		}
		#endregion

		/// <summary>
		/// Method is called when <see cref="OnHealthChanged"/> is invoked.
		/// </summary>
		/// <param name="args">The <see cref="HealthChangedArgs"/> args.</param>
		protected virtual void HealthChangedInternal(HealthChangedArgs args)
		{
			
		}
		
		/// <summary>
		/// Method is called when <see cref="OnMaxHealthChanged"/> is invoked.
		/// </summary>
		/// <param name="args">The <see cref="HealthChangedArgs"/> args.</param>
		protected virtual void MaxHealthChangedInternal(HealthChangedArgs args)
		{
			
		}
		
		/// <summary>
		/// Method is called when <see cref="OnHealed"/> is invoked.
		/// </summary>
		/// <param name="args">The <see cref="HealthChangedArgs"/> args.</param>
		protected virtual void HealedInternal(HealthChangedArgs args)
		{
			
		}
		
		/// <summary>
		/// Method is called when <see cref="OnDamaged"/> is invoked.
		/// </summary>
		/// <param name="args">The <see cref="HealthChangedArgs"/> args.</param>
		protected virtual void DamagedInternal(HealthChangedArgs args)
		{
			
		}
		
		/// <summary>
		/// Method is called when <see cref="OnDie"/> is invoked.
		/// </summary>
		protected virtual void DiedInternal()
		{
			
		}
		
		#region Event Handlers

		private void HealthChanged(object sender, EventArgs e) =>
			OnHealthChanged?.Invoke(new HealthChangedArgs(
					_internalHealthSystem.GetHealth(),
					_previousHealth));

		private void MaxHealthChanged(object sender, EventArgs e) =>
			OnMaxHealthChanged?.Invoke(new HealthChangedArgs(
					_internalHealthSystem.GetHealthMax(),
					_previousMaxHealth));

		private void Healed(object sender, EventArgs e) =>
			OnHealed?.Invoke(new HealthChangedArgs(
					_internalHealthSystem.GetHealthMax(),
					_previousMaxHealth));

		private void Damaged(object sender, EventArgs e) =>
			OnDamaged?.Invoke(new HealthChangedArgs(
					_internalHealthSystem.GetHealth(),
					_previousHealth));

		private void Died(object sender, EventArgs e) => OnDie?.Invoke();

		#endregion
	}
}