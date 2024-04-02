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

namespace Enemy.Jobs
{
	public class EnemyTargetManager : MonoBehaviour
	{
		[Required]
		[SerializeField]
		private EnemyRegistryAsset enemyRegistry;
	
		[Required]
		[SerializeField]
		private PlayerRegistryAsset playerRegistry;
	
		[DisableInPlayMode]
		[SerializeField]
		private int minBatchCount = 10;
	
		[Tooltip("The number of frames to wait before completing the search job.\n" +
		         "The allocation type used requires that the job finishes within 4 frames, so this value should be no higher than 4.")]
		[DisableInPlayMode]
		[PropertyRange(1, 4)]
		[SerializeField]
		private int resultWaitTime = 4; // fix this not working with values under 4
	
		private NativeArray<Vector3> _playerPositions;
		private NativeArray<int> _playerIds;
		private NativeArray<Vector3> _enemyPositions;
		private NativeArray<Vector3> _resultPositions; // consider removing this, we don't use the result positions
		private NativeArray<int> _resultIds;
		private JobHandle _runningJobHandle;
	
		private EnemyComponent[] _cachedEnemyReferences;
		private (int[] id, Transform[] body) _cachedPlayerData;
	
		private int _frameCount = 0;
	
		#region Unity Methods

		private void Update()
		{
			int frameMod = _frameCount % resultWaitTime;
		
			if (frameMod == 0)
			{
				CacheRegistryValues();
			
				if (enemyRegistry.Count == 0 || playerRegistry.Count == 0) return;

				InitializeNativeArrays();
		
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
			else if (frameMod == resultWaitTime - 1)
			{
				_runningJobHandle.Complete();

				for (int i = 0; i < _cachedEnemyReferences.Length; i++)
				{
					var target = playerRegistry.GetValue(_resultIds[i]);
					if (target == null || !target.gameObject.activeSelf) continue;
					_cachedEnemyReferences[i].PathfindingComponent.SetTargetPosition(target.position);
				}
			
				if (!_runningJobHandle.IsCompleted) return;
			
				DisposeNativeArrays();
			}
		
			_frameCount++;
		}

		private void OnDisable()
		{
			_runningJobHandle.Complete();
			if (!_runningJobHandle.IsCompleted) return;
		
			DisposeNativeArrays();
		}

		#endregion

		private void CacheRegistryValues()
		{
			_cachedEnemyReferences = enemyRegistry.GetValuesArray();
			_cachedPlayerData = (playerRegistry.GetKeysArray(), playerRegistry.GetValuesArray());
		}
		
		private void InitializeNativeArrays()
		{
			_playerPositions = new NativeArray<Vector3>(_cachedPlayerData.id.Length, Allocator.TempJob);
			_playerIds = new NativeArray<int>(_cachedPlayerData.id.Length, Allocator.TempJob);
			_enemyPositions = new NativeArray<Vector3>(_cachedEnemyReferences.Length, Allocator.TempJob);
			_resultPositions = new NativeArray<Vector3>(_cachedEnemyReferences.Length, Allocator.TempJob);
			_resultIds = new NativeArray<int>(_cachedEnemyReferences.Length, Allocator.TempJob);
		
			// fill the native arrays with the cached data
			for (int i = 0; i < _cachedEnemyReferences.Length; i++)
			{
				_enemyPositions[i] = _cachedEnemyReferences[i].transform.position;
			}
		
			for (int i = 0; i < _cachedPlayerData.id.Length; i++)
			{
				_playerPositions[i] = _cachedPlayerData.body[i].position;
				_playerIds[i] = _cachedPlayerData.id[i];
			}
		}
		
		private void DisposeNativeArrays()
		{
			_playerPositions.Dispose();
			_playerIds.Dispose();
			_enemyPositions.Dispose();
			_resultPositions.Dispose();
			_resultIds.Dispose();
		}
	}
}