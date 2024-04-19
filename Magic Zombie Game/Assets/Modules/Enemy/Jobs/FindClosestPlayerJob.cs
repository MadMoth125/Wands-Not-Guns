using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Enemy.Jobs
{
	/// <summary>
	/// DOTS Job that finds the closest player to each enemy.
	/// Requires the positions of all enemies and id/positions of all players.
	/// 
	/// IDs are the ideal output so players can be identified in the registry
	/// with the most up-to-date position rather than multiple frames old positional data.
	/// </summary>
	[BurstCompile]
	public struct FindClosestPlayerJob : IJobFor
	{
		/// <summary>
		/// The positions of all currently active enemies.
		/// </summary>
		[ReadOnly]
		private NativeArray<Vector3> _enemyPositions;
	
		/// <summary>
		/// The IDs/positions of all currently active players.
		/// </summary>
		[ReadOnly]
		private NativeArray<PlayerData> _playerData;
	
		/// <summary>
		/// The resulting IDs/positions of the closest player to each enemy.
		/// </summary>
		public NativeArray<PlayerData> resultData;

		#if true
		
		// fixed code: now we only loop through each player and compare it against the current index enemy.
		public void Execute(int index)
		{
			// save outside of loop for more re-usability
			PlayerData bestPlayer = new PlayerData(-1, Vector3.zero);

			bestPlayer.SetData(-1, Vector3.zero);
			float bestDistance = float.MaxValue;

			foreach (var dat in _playerData)
			{
				Vector3 distVec = dat.Position - _enemyPositions[index];
				float dist = distVec.sqrMagnitude;
				
				if (dist < bestDistance)
				{
					bestPlayer = dat;
					bestDistance = dist;
				}
			}
			
			resultData[index] = bestPlayer;
		}
		
		#else
		
		// problematic code: the outer loop is not necessary because the job already has an iteration index.
		public void Execute(int index)
		{
			// save outside of loop for more re-usability
			PlayerData bestPlayer = new PlayerData(-1, Vector3.zero);

			foreach (var enemyPos in _enemyPositions)
			{
				bestPlayer.SetData(-1, Vector3.zero);
				float bestDistance = float.MaxValue;

				foreach (var dat in _playerData)
				{
					Vector3 distVec = dat.Position - enemyPos;
					float dist = distVec.sqrMagnitude;
				
					if (dist < bestDistance)
					{
						bestPlayer = dat;
						bestDistance = dist;
					}
				}
			
				resultData[index] = bestPlayer;
			}
		}
		
		#endif
		
		public void SetData(in NativeArray<Vector3> enemyPositionArray, in NativeArray<PlayerData> playerDataArray, in NativeArray<PlayerData> resultArray)
		{
			_enemyPositions = enemyPositionArray;
			_playerData = playerDataArray;
			this.resultData = resultArray;
		}
	}
}