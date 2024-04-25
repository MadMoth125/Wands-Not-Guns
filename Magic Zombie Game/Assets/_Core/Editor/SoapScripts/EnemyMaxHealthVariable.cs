using UnityEngine;
using Obvious.Soap;

namespace Core.Data
{
	[CreateAssetMenu(fileName = nameof(EnemyMaxHealthVariable), menuName = "Game/Data/" + nameof(EnemyMaxHealthVariable))]
	public class EnemyMaxHealthVariable : ScriptableVariable<EnemyMaxHealthParameters>
	{
		/// <summary>
		/// Gets the max health for the enemy.
		/// </summary>
		/// <param name="round">The round to get the max health for.</param>
		/// <returns>The max health for the given round.</returns>
		public double GetMaxHealth(int round)
		{
			return GameFunction.GetMaxHealthDouble(round, _value.StartingHealth, _value.FlatIncreaseAmount, _value.PercentageIncreaseAmount);
		}
		
		/// <summary>
		/// Gets the max health for the enemy.
		/// Uses a reference to the round value.
		/// </summary>
		/// <returns>The max health for the given round.</returns>
		public double GetMaxHealth()
		{
			return GetMaxHealth(_value.RoundValue);
		}
	}
}