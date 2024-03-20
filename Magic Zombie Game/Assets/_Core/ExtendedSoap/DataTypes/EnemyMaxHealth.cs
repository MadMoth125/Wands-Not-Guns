using UnityEngine;
using Obvious.Soap;

namespace Core.Data
{
	[System.Serializable]
	public class EnemyMaxHealth
	{
		public double StartingHealth => startingHealth;
	
		public double FlatIncreaseAmount => flatIncreaseAmount;
	
		public double PercentageIncreaseAmount => percentageIncreaseAmount;
	
		public int RoundValue => roundValue != null ? roundValue.Value : 1;
		
		[SerializeField]
		private double startingHealth = 150d;
	
		[SerializeField]
		private double flatIncreaseAmount = 100d;
	
		[SerializeField]
		private double percentageIncreaseAmount = 0.1d;
		
		[SerializeField]
		private IntVariable roundValue;
	}
}