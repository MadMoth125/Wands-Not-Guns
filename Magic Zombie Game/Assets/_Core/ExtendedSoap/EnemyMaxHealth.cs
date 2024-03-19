using UnityEngine;

namespace Core.Data
{
	[System.Serializable]
	public class EnemyMaxHealth
	{
		public double StartingHealth => startingHealth;
	
		public double FlatIncreaseAmount => flatIncreaseAmount;
	
		public double PercentageIncreaseAmount => percentageIncreaseAmount;
	
		[SerializeField]
		private double startingHealth = 150d;
	
		[SerializeField]
		private double flatIncreaseAmount = 100d;
	
		[SerializeField]
		private double percentageIncreaseAmount = 0.1d;
	}
}