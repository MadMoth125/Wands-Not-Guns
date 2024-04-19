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
		public void RoundOneCount()
		{
			Assert.AreEqual(6, Core.GameFunction.GetSpawnCount(1, 1));
		}
		
		[Test]
		public void RoundTwoCount()
		{
			Assert.AreEqual(8, Core.GameFunction.GetSpawnCount(2, 1));
		}
		
		[Test]
		public void RoundThreeCount()
		{
			Assert.AreEqual(13, Core.GameFunction.GetSpawnCount(3, 1));
		}
		
		[Test]
		public void RoundFourCount()
		{
			Assert.AreEqual(18, Core.GameFunction.GetSpawnCount(4, 1));
		}
		
		[Test]
		public void RoundFiveCount()
		{
			Assert.AreEqual(24, Core.GameFunction.GetSpawnCount(5, 1));
		}
		
		[Test]
		public void RoundSixCount()
		{
			Assert.AreEqual(27, Core.GameFunction.GetSpawnCount(6, 1));
		}
		
		[Test]
		public void RoundSevenCount()
		{
			Assert.AreEqual(28, Core.GameFunction.GetSpawnCount(7, 1));
		}
		
		[Test]
		public void RoundEightCount()
		{
			Assert.AreEqual(28, Core.GameFunction.GetSpawnCount(8, 1));
		}
		
		[Test]
		public void RoundNineCount()
		{
			Assert.AreEqual(29, Core.GameFunction.GetSpawnCount(9, 1));
		}

		[Test]
		public void RoundTenCount()
		{
			Assert.AreEqual(33, Core.GameFunction.GetSpawnCount(10, 1));
		}

		[Test]
		public void RoundElevenCount()
		{
			Assert.AreEqual(34, Core.GameFunction.GetSpawnCount(11, 1));
		}

		[Test]
		public void RoundTwentyCount()
		{
			Assert.AreEqual(60, Core.GameFunction.GetSpawnCount(20, 1));
		}
		
		[Test]
		public void RoundThirtyCount()
		{
			Assert.AreEqual(105, Core.GameFunction.GetSpawnCount(30, 1));
		}
		
		[Test]
		public void RoundFiftyCount()
		{
			Assert.AreEqual(243, Core.GameFunction.GetSpawnCount(50, 1));
		}
		
		[Test]
		public void RoundSeventyFiveCount()
		{
			Assert.AreEqual(510, Core.GameFunction.GetSpawnCount(75, 1));
		}
		
		[Test]
		public void RoundOneHundredCount()
		{
			Assert.AreEqual(885, Core.GameFunction.GetSpawnCount(100, 1));
		}
	}
}