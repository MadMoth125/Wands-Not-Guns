using UnityEngine;
using Obvious.Soap;

namespace Core.Data
{
	[CreateAssetMenu(fileName = "scriptable_variable_" + nameof(EnemyMaxHealth), menuName = "Soap/CustomScriptableVariables/"+ nameof(EnemyMaxHealth), order = -999)]
	public class EnemyMaxHealthVariable : ScriptableVariable<EnemyMaxHealth>
	{
		public double GetMaxHealth(int round)
		{
			return GameplayParams.GetMaxHealthDouble(round, _value.StartingHealth, _value.FlatIncreaseAmount, _value.PercentageIncreaseAmount);
		}
	}
}