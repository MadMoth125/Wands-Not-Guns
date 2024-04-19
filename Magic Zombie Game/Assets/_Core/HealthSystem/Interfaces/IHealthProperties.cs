using UnityEngine;

namespace Core.HealthSystem
{
	public interface IHealthProperties : IDamageable, IHealable
	{
		/// <summary>
		/// The Object that owns this <see cref="IHealthProperties"/>.
		/// </summary>
		/// <returns>The Object owner.</returns>
		public Object GetOwner();
		
		/// <summary>
		/// Sets the Object that owns this <see cref="IHealthProperties"/>.
		/// </summary>
		/// <param name="owner">The Object owner.</param>
		public void SetOwner(Object owner);
	
		/// <summary>
		/// The current health of the target.
		/// </summary>
		public float GetHealth();
	
		/// <summary>
		/// Sets the current health of the target.
		/// Clamped between 0 and the maximum health.
		/// </summary>
		public void SetHealth(float health);
	
		/// <summary>
		/// The maximum health of the target.
		/// </summary>
		public float GetMaxHealth();
	
		/// <summary>
		/// Sets the maximum health of the target.
		/// </summary>
		/// <param name="healthMax">The new maximum health.</param>
		/// <param name="fullHealth">Should the health be set to the new maximum health.</param>
		public void SetMaxHealth(float healthMax, bool fullHealth);
	
		/// <summary>
		/// The current health of the target as a normalized value between 0 and 1.
		/// </summary>
		public float GetHealthNormalized();
	
		/// <summary>
		/// Whether the target is dead.
		/// </summary>
		public bool IsDead();
	
		/// <summary>
		/// When the target's health reaches 0.
		/// </summary>
		public void Die();
	}
}