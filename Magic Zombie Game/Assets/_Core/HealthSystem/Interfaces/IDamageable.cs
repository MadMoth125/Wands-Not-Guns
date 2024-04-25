using UnityEngine;

namespace Core.HealthSystem
{
	public interface IDamageable
	{
		/// <summary>
		/// Damages the target by the specified amount.
		/// </summary>
		/// <param name="amount">The damage to apply.</param>
		/// <param name="source">The source of the damage.</param>
		public void Damage(float amount, Object source);
	
		/// <summary>
		/// Kills the target, setting its health to 0.
		/// </summary>
		/// <param name="source">The source of the damage.</param>
		public void Kill(Object source);
	}
}