using UnityEngine;

namespace Core.HealthSystem
{
	public interface IHealable
	{
		/// <summary>
		/// Heals the target by the specified amount.
		/// </summary>
		/// <param name="amount">The health to regain.</param>
		public void Heal(float amount);
	
		/// <summary>
		/// Fully heals the target to its maximum health.
		/// </summary>
		public void HealComplete();
	}
}