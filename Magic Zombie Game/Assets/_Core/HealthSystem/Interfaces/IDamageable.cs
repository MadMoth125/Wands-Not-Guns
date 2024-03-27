using UnityEngine;

namespace Core.HealthSystem
{
	public interface IDamageable
	{
		/// <summary>
		/// Damages the target by the specified amount.
		/// </summary>
		/// <param name="amount">The damage to apply.</param>
		public void Damage(float amount);
	
		/// <summary>
		/// Kills the target, setting its health to 0.
		/// </summary>
		public void Kill();
	}
}