using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Enemy.Jobs
{
	/// <summary>
	/// Simple job that finds the closest player to each enemy.
	/// </summary>
	[BurstCompile]
	public struct FindClosestPlayerJob : IJobFor
	{
		/// <summary>
		/// The positions of all currently active enemies.
		/// </summary>
		[ReadOnly]
		public NativeArray<Vector3> enemyPositions;
	
		/// <summary>
		/// The positions of all currently active players.
		/// </summary>
		[ReadOnly]
		public NativeArray<Vector3> playerPositions;
	
		/// <summary>
		/// The instance IDs of all currently active players.
		/// </summary>
		[ReadOnly]
		public NativeArray<int> playerIds;
	
		/// <summary>
		/// The resulting positions of the closest player to each enemy.
		/// </summary>
		public NativeArray<Vector3> resultPositions;
	
		/// <summary>
		/// The resulting instance IDs of the closest player to each enemy.
		/// </summary>
		public NativeArray<int> resultIds;
	
		public void Execute(int index)
		{
			foreach (var enemyPos in enemyPositions)
			{
				int bestPlayerId = -1;
				Vector3 bestPosition = Vector3.zero;
				float bestDistance = float.MaxValue;

				for (int i = 0; i < playerIds.Length; i++)
				{
					Vector3 distVec = playerPositions[i] - enemyPos;
					float dist = distVec.sqrMagnitude;
				
					if (dist < bestDistance)
					{
						bestPlayerId = playerIds[i];
						bestPosition = playerPositions[i];
						bestDistance = dist;
					}
				}
			
				resultPositions[index] = bestPosition;
				resultIds[index] = bestPlayerId;
			}
		}
	}
}