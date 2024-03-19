using UnityEngine;
using Obvious.Soap;

namespace Core.Data
{
	[CreateAssetMenu(fileName = "EnemyMaxHealthVariable", menuName = "Soap/CustomScriptableVariables/" + nameof(EnemyMaxHealthVariable))]
	public class EnemyMaxHealthVariable : ScriptableVariable<EnemyMaxHealth>
	{
		[Header("Round Value Reference")]
		[SerializeField]
		private IntVariable roundValue;
		
		/// <summary>
		/// Gets the max health for the enemy.
		/// </summary>
		/// <param name="round">The round to get the max health for.</param>
		/// <returns>The max health for the given round.</returns>
		public double GetMaxHealth(int round)
		{
			return GameplayParams.GetMaxHealthDouble(round, _value.StartingHealth, _value.FlatIncreaseAmount, _value.PercentageIncreaseAmount);
		}
		
		/// <summary>
		/// Gets the max health for the enemy.
		/// Uses a reference to the round value.
		/// </summary>
		/// <returns>The max health for the given round.</returns>
		public double GetMaxHealth()
		{
			return GetMaxHealth(roundValue.Value);
		}
	}
}