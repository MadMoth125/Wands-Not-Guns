using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct ExampleJob : IJobFor
{
	[ReadOnly]
	public NativeArray<Vector3> enemyPositions;
	
	[ReadOnly]
	public NativeArray<Vector3> playerPositions;
	
	public NativeArray<Vector3> closestPlayerPositions;
	
	public void Execute(int index)
	{
		foreach (var enemyPos in enemyPositions)
		{
			Vector3 closestPosition = Vector3.zero;
			float closestDistance = float.MaxValue;
			
			foreach (var playerPos in playerPositions)
			{
				Vector3 positionDifference = playerPos - enemyPos;
				float distance = positionDifference.sqrMagnitude;

				if (distance < closestDistance)
				{
					closestPosition = playerPos;
					closestDistance = distance;
				}
			}
			
			closestPlayerPositions[index] = closestPosition;
		}
	}
}