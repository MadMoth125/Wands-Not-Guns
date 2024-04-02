using System;
using System.Collections.Generic;
using System.Linq;
using Enemy.Registry;
using Player.Registry;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyTargetManager : MonoBehaviour
{
	[Required]
	public EnemyRegistryAsset enemyRegistry;
	
	[Required]
	public PlayerRegistryAsset playerRegistry;
	
	[DisableInPlayMode]
	public int minBatchCount = 10;
	
	private NativeArray<Vector3> _playerPositions;
	private NativeArray<int> _playerIds;
	private NativeArray<Vector3> _enemyPositions;
	private NativeArray<Vector3> _resultPositions;
	private NativeArray<int> _resultIds;
	private JobHandle _runningJobHandle;
	
	private EnemyComponent[] _cachedEnemyReferences;
	private (int[] id, Transform[] body) _cachedPlayerData;
	
	private int _frameCount = 0;
	private int _waitTime = 4;
	
	#region Unity Methods

	private void Update()
	{
		int frameMod = _frameCount % _waitTime;
		
		if (frameMod == 0)
		{
			_cachedEnemyReferences = enemyRegistry.GetValuesArray();
			_cachedPlayerData = (playerRegistry.GetKeysArray(), playerRegistry.GetValuesArray());
			
			if (enemyRegistry.Count == 0 || playerRegistry.Count == 0) return;

			_playerPositions = new NativeArray<Vector3>(_cachedPlayerData.id.Length, Allocator.TempJob);
			_playerIds = new NativeArray<int>(_cachedPlayerData.id.Length, Allocator.TempJob);
			_enemyPositions = new NativeArray<Vector3>(_cachedEnemyReferences.Length, Allocator.TempJob);
			_resultPositions = new NativeArray<Vector3>(_cachedEnemyReferences.Length, Allocator.TempJob);
			_resultIds = new NativeArray<int>(_cachedEnemyReferences.Length, Allocator.TempJob);
			
			for (int i = 0; i < _cachedEnemyReferences.Length; i++)
			{
				_enemyPositions[i] = _cachedEnemyReferences[i].transform.position;
			}
			
			for (int i = 0; i < _cachedPlayerData.id.Length; i++)
			{
				_playerPositions[i] = _cachedPlayerData.body[i].position;
				_playerIds[i] = _cachedPlayerData.id[i];
			}
		
			FindClosestPlayerJob job = new FindClosestPlayerJob()
			{
				playerPositions = _playerPositions,
				playerIds = _playerIds,
				enemyPositions = _enemyPositions,
				resultPositions = _resultPositions,
				resultIds = _resultIds,
			};
			
			_runningJobHandle = job.ScheduleParallel(_cachedEnemyReferences.Length, minBatchCount, new JobHandle());
		}
		else if (frameMod == _waitTime - 1)
		{
			_runningJobHandle.Complete();

			Debug.Log("job handle completed");
			
			for (int i = 0; i < _cachedEnemyReferences.Length; i++)
			{
				var target = playerRegistry.GetValue(_resultIds[i]);
				if (target == null) continue;
				_cachedEnemyReferences[i].PathfindingComponent.SetTargetPosition(target.position);
			}
			
			if (!_runningJobHandle.IsCompleted) return;
			Debug.Log("job items disposed");
			_playerPositions.Dispose();
			_playerIds.Dispose();
			_enemyPositions.Dispose();
			_resultPositions.Dispose();
			_resultIds.Dispose();
		}
		
		_frameCount++;
	}

	private void OnDisable()
	{
		_runningJobHandle.Complete();
		if (!_runningJobHandle.IsCompleted) return;
		
		_playerPositions.Dispose();
		_playerIds.Dispose();
		_enemyPositions.Dispose();
		_resultPositions.Dispose();
		_resultIds.Dispose();
	}

	#endregion
}