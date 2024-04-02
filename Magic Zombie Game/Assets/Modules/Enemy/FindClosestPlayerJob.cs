using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct FindClosestPlayerJob : IJobFor
{
	[ReadOnly]
	public NativeArray<Vector3> enemyPositions;
	
	[ReadOnly]
	public NativeArray<Vector3> playerPositions;
	
	[ReadOnly]
	public NativeArray<int> playerIds;
	
	public NativeArray<Vector3> resultPositions;
	
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