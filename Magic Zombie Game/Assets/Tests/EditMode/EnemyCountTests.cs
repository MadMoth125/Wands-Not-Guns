using NUnit.Framework;

namespace GameTesting
{
	/// <summary>
	/// Testing class for making sure the correct number of enemies will spawn on the correct round.
	/// Testing values are pulled from https://callofduty.fandom.com/wiki/Zombies_(Treyarch)#Mechanics
	/// </summary>
	public class EnemyCountTests
	{
		[Test]
		public void RoundTenCount()
		{
			Assert.AreEqual(33, Core.GameplayParams.GetSpawnCount(10, 1));
		}
		
		[Test]
		public void RoundTwentyCount()
		{
			Assert.AreEqual(60, Core.GameplayParams.GetSpawnCount(20, 1));
		}
		
		[Test]
		public void RoundThirtyCount()
		{
			Assert.AreEqual(105, Core.GameplayParams.GetSpawnCount(30, 1));
		}
		
		[Test]
		public void RoundFiftyCount()
		{
			Assert.AreEqual(243, Core.GameplayParams.GetSpawnCount(50, 1));
		}
		
		[Test]
		public void RoundSeventyFiveCount()
		{
			Assert.AreEqual(510, Core.GameplayParams.GetSpawnCount(75, 1));
		}
		
		[Test]
		public void RoundOneHundredCount()
		{
			Assert.AreEqual(885, Core.GameplayParams.GetSpawnCount(100, 1));
		}
	}
}