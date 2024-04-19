using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

public class JobManager : MonoBehaviour
{
	public int playerCount = 4;
	public int enemyCount = 50;
	public int minBatchCount = 10;
	
	private JobHandle _runningJobHandle;
	
	private NativeArray<Vector3> _playerPositions;
	private NativeArray<Vector3> _enemyPositions;
	private NativeArray<Vector3> _resultPositions;
	
	private int _frameCount = 0;
	private int _waitTime = 4;
	
	#region Unity Methods

	private void Update()
	{
		var frameMod = _frameCount % _waitTime;
		
		if (frameMod == 0)
		{
			_playerPositions = new NativeArray<Vector3>(playerCount, Allocator.TempJob);
			_enemyPositions = new NativeArray<Vector3>(enemyCount, Allocator.TempJob);
			_resultPositions = new NativeArray<Vector3>(enemyCount, Allocator.TempJob);

			for (int i = 0; i < _playerPositions.Length; i++)
			{
				_playerPositions[i] = GetRandomPosition();
			}

			for (int i = 0; i < _enemyPositions.Length; i++)
			{
				_enemyPositions[i] = GetRandomPosition();
			}

			ExampleJob job = new ExampleJob
			{
				playerPositions = _playerPositions,
				enemyPositions = _enemyPositions,
				closestPlayerPositions = _resultPositions,
			};
			
			_runningJobHandle = job.ScheduleParallel(enemyCount, minBatchCount, new JobHandle());
		}
		else if (frameMod == _waitTime - 1)
		{
			CleanupJobs();
		}
		
		_frameCount++;
	}

	private void OnDisable()
	{
		CleanupJobs();
	}

	#endregion

	private void CleanupJobs()
	{
		_runningJobHandle.Complete();
		if (!_runningJobHandle.IsCompleted) return;
		
		if (_playerPositions.IsCreated)
		{
			_playerPositions.Dispose();
		}
		
		if (_enemyPositions.IsCreated)
		{
			_enemyPositions.Dispose();
		}
		
		if (_resultPositions.IsCreated)
		{
			_resultPositions.Dispose();
		}
	}
	
	private Vector3 GetRandomPosition()
	{
		return new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
	}
}