using System;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
	/// <summary>
	/// Static class containing parameters for gameplay calculations and formulas.
	/// </summary>
	public static class GameplayParams
	{
		// duh...
		private const int MINIMUM_ROUND = 1;
	
		// use this if the enemy uses 'float' for health type
		private const int MAXIMUM_ROUND_ENEMY_HEALTH = 106;
	
		// use this if enemy uses 'double' for health type
		private const int MAXIMUM_ROUND_ENEMY_HEALTH_EXTENDED = 339; 

		// spawn interval should clamp calculations at this round
		private const int MAXIMUM_ROUND_SPAWN_INTERVAL = 64;
	
		// spawn counts used for the first 11 rounds, after which the spawn count is calculated using a formula
		private static readonly int[] definedSpawnCounts = { 6, 8, 13, 18, 24, 27, 28, 28, 29, 33, 34 };

		#region Enemy Max Health

		public static float GetMaxHealthFloat(int round, float startingHealth, float flatIncreaseAmount, float percentageIncreaseAmount)
		{
			// clamp the round to the appropriate minimum and maximum values
			round = Math.Clamp(round, MINIMUM_ROUND, MAXIMUM_ROUND_ENEMY_HEALTH);
		
			// convert the double value to a float
			return (float)GetMaxHealthDouble(round, startingHealth, flatIncreaseAmount, percentageIncreaseAmount);
		}
	
		public static double GetMaxHealthDouble(int round, double startingHealth, double flatIncreaseAmount, double percentageIncreaseAmount)
		{
			// clamp the round to the appropriate minimum and maximum values
			round = Math.Clamp(round, MINIMUM_ROUND, MAXIMUM_ROUND_ENEMY_HEALTH_EXTENDED);
			
			if (round < 10)
			{
				return CalculateFlatHealthIncrease(round, startingHealth, flatIncreaseAmount);
			}
		
			// 10th and beyond
			double roundNineHealth = CalculateFlatHealthIncrease(9, startingHealth, flatIncreaseAmount);
			return CalculatePercentageHealthIncrease(round, roundNineHealth, percentageIncreaseAmount);
		}
	
		private static double CalculateFlatHealthIncrease(int round, double startingHealth, double flatIncreaseAmount)
		{
			return startingHealth + flatIncreaseAmount * (round - 1);
		}
		
		private static double CalculatePercentageHealthIncrease(int round, double roundNineHealth, double percentageIncreaseAmount)
		{
			for (int i = 0; i < round - 9; i++)
			{
				roundNineHealth *= (1 + percentageIncreaseAmount);
			}

			return Math.Round(roundNineHealth);
		}

		#endregion

		#region Enemy Spawn Counts

		/// <summary>
		/// Determine the number of enemies to spawn for a given round and number of players.
		/// </summary>
		/// <param name="round">The current round.</param>
		/// <param name="players">The number of players in the game.</param>
		/// <returns>The number of enemies to spawn.</returns>
		public static int GetSpawnCount(int round, int players)
		{
			if (definedSpawnCounts.Length > round - 1)
			{
				// we do a modulo operation to ensure that the spawn count
				// is not out of range, even if the returned value is 'incorrect' for the given round.
				return definedSpawnCounts[round - 1 % definedSpawnCounts.Length];
			}
			
			switch (players)
			{
				case 1:
					return (int)Math.Ceiling(OnePlayerSpawnCount(round));
				case 2:
					return (int)Math.Ceiling(TwoPlayerSpawnCount(round));
				case 3:
					return (int)Math.Ceiling(ThreePlayerSpawnCount(round));
				case 4:
					return (int)Math.Ceiling(FourPlayerSpawnCount(round));
				default:
				{
					if (players < 1) return 0;
					return (int)Math.Ceiling(FourPlayerSpawnCount(round));
				}
			}
		}

		// All numbers pulled directly from a Fandom wiki page:
		// https://callofduty.fandom.com/wiki/Zombies_(Treyarch)
		
		private static double OnePlayerSpawnCount(int round) => 0.000058 * Math.Pow(round, 3) +
		                                                        0.074032 * Math.Pow(round, 2) +
		                                                        0.718119 * round + 14.738699;

		private static double TwoPlayerSpawnCount(int round) => 0.000054 * Math.Pow(round, 3) +
		                                                        0.169717 * Math.Pow(round, 2) +
		                                                        0.541627 * round + 15.917041;

		private static double ThreePlayerSpawnCount(int round) => 0.000169 * Math.Pow(round, 3) +
		                                                          0.238079 * Math.Pow(round, 2) +
		                                                          1.307276 * round + 21.291046;

		private static double FourPlayerSpawnCount(int round) => 0.000225 * Math.Pow(round, 3) +
		                                                         0.314314 * Math.Pow(round, 2) +
		                                                         1.835712 * round + 27.596132;

		#endregion

		#region Enemy Spawn Intervals

		/// <summary>
		/// Determine the spawn interval for a given round.
		/// </summary>
		/// <param name="round">The current round.</param>
		/// <param name="maxInterval">The maximum interval between enemy spawns.</param>
		/// <param name="minInterval">The minimum interval between enemy spawns.</param>
		/// <param name="reductionRate">The percentage rate at which the interval is reduced.</param>
		/// <param name="roundValues">Should the interval be rounded to the nearest hundredth?</param>
		/// <returns>A spawn interval corresponding to the given parameters.</returns>
		public static float GetSpawnInterval(int round, float maxInterval, float minInterval, float reductionRate, bool roundValues)
		{
			round = Math.Clamp(round, MINIMUM_ROUND, MAXIMUM_ROUND_SPAWN_INTERVAL);
			float interval = maxInterval;

			if (round < MAXIMUM_ROUND_SPAWN_INTERVAL)
			{
				for (int i = 0; i < round; i++)
				{
					// If the round is 1, set the interval to the maximum interval and break
					if (round <= 1)
					{
						interval = maxInterval;
						break;
					}
					
					// Skip the first round to prevent a double reduction between rounds 1 and 2
					if (i == 0) continue;
					
					// If the round is greater than 1, reduce the interval by the reduction rate
					if (roundValues)
					{
						interval = (float)Math.Round(interval * 100 * (1 - reductionRate)) / 100;
					}
					else
					{
						interval *= (1 - reductionRate);
					}
					
					// If the interval is less than or equal to the minimum interval,
					// set it to the minimum interval and break
					if (interval <= minInterval)
					{
						interval = minInterval;
						break;
					}
				}
			}
			else
			{
				interval = minInterval;
			}

			return interval;
		}

		#endregion
	}
}