using NUnit.Framework;

namespace GameTesting
{
	/// <summary>
	/// Testing class for making sure the correct health values are returned for enemies.
	/// Testing values are pulled from https://callofduty.fandom.com/wiki/Zombies_(Treyarch)#Mechanics
	/// </summary>
	public class EnemyHealthTests
	{
		private const double STARTING_HEALTH = 150;
		private const double FLAT_INCREASE_AMOUNT = 100;
		private const double PERCENTAGE_INCREASE_AMOUNT = 0.1;
		
		[Test]
		public void RoundOneHealth()
		{
			Assert.AreEqual(150d, Core.GameplayParams.GetMaxHealthDouble(1, 
				STARTING_HEALTH, 
				FLAT_INCREASE_AMOUNT, 
				PERCENTAGE_INCREASE_AMOUNT));
		}
		
		[Test]
		public void RoundTwoHealth()
		{
			Assert.AreEqual(250d, Core.GameplayParams.GetMaxHealthDouble(2, 
				STARTING_HEALTH, 
				FLAT_INCREASE_AMOUNT, 
				PERCENTAGE_INCREASE_AMOUNT));
		}
		
		[Test]
		public void RoundEightHealth()
		{
			Assert.AreEqual(850d, Core.GameplayParams.GetMaxHealthDouble(8, 
				STARTING_HEALTH, 
				FLAT_INCREASE_AMOUNT, 
				PERCENTAGE_INCREASE_AMOUNT));
		}
		
		[Test]
		public void RoundNineHealth()
		{
			Assert.AreEqual(950d, Core.GameplayParams.GetMaxHealthDouble(9, 
				STARTING_HEALTH, 
				FLAT_INCREASE_AMOUNT, 
				PERCENTAGE_INCREASE_AMOUNT));
		}
		
		[Test]
		public void RoundTenHealth()
		{
			Assert.AreEqual(1045d, Core.GameplayParams.GetMaxHealthDouble(10, 
				STARTING_HEALTH, 
				FLAT_INCREASE_AMOUNT, 
				PERCENTAGE_INCREASE_AMOUNT));
		}
		
		[Test]
		public void RoundFifteenHealth()
		{
			Assert.AreEqual(1683d, Core.GameplayParams.GetMaxHealthDouble(15, 
				STARTING_HEALTH, 
				FLAT_INCREASE_AMOUNT, 
				PERCENTAGE_INCREASE_AMOUNT));
		}
		
		[Test]
		public void RoundThirtyHealth()
		{
			Assert.AreEqual(7030d, Core.GameplayParams.GetMaxHealthDouble(30, 
				STARTING_HEALTH, 
				FLAT_INCREASE_AMOUNT, 
				PERCENTAGE_INCREASE_AMOUNT));
		}
		
		[Test]
		public void RoundFiftyHealth()
		{
			Assert.AreEqual(47_296d, Core.GameplayParams.GetMaxHealthDouble(50, 
				STARTING_HEALTH, 
				FLAT_INCREASE_AMOUNT, 
				PERCENTAGE_INCREASE_AMOUNT));
		}
		
		[Test]
		public void RoundOneHundredHealth()
		{
			Assert.AreEqual(5_552_109, Core.GameplayParams.GetMaxHealthDouble(100, 
				STARTING_HEALTH, 
				FLAT_INCREASE_AMOUNT, 
				PERCENTAGE_INCREASE_AMOUNT));
		}
	}
}