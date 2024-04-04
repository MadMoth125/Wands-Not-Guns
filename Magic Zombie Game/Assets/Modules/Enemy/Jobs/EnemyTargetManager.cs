using Core.CustomDebugger;
using Enemy.Registry;
using Player.Registry;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

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
	
		[Tooltip("The minimum number of enemies to process in a single batch.\n" +
		         "Higher values may improve performance for larger enemy groups.")]
		[DisableInPlayMode]
		[SerializeField]
		private int minBatchCount = 10;
	
		[Tooltip("The number of frames to wait before completing the search job.\n" +
		         "The allocation type used requires that the job finishes within 4 frames, so this value should be no higher than 4.")]
		[DisableInPlayMode]
		[PropertyRange(1, 4)]
		[SerializeField]
		private int resultWaitTime = 4;
		
		[TitleGroup("Debug", "Parameters", Alignment = TitleAlignments.Centered)]
		[SerializeField]
		private LoggerAsset logger;

		[TitleGroup("Debug")]
		[Tooltip("Enable logging of when the job starts and ends.")]
		[SerializeField]
		private bool logJobStartAndEnd = false;
		
		[TitleGroup("Debug")]
		[Tooltip("Enable logging of the native arrays and their state.")]
		[SerializeField]
		private bool logNativeArrays = false;
		
		private NativeArray<Vector3> _enemyPositions;
		private NativeArray<PlayerData> _playerData;
		private NativeArray<PlayerData> _resultData;
		private JobHandle _runningJobHandle;
	
		private EnemyComponent[] _cachedEnemies;
		private (int[] id, Transform[] body) _cachedPlayers;
		
		private int _frameCount = 0;
		private bool _firstIfExecuted;
	
		#region Unity Methods

		private void Update()
		{
			// job loop is handled over multiple frames to ensure the most performant execution.
			int frameMod = _frameCount % resultWaitTime;
		
			if (frameMod == 0 && !_firstIfExecuted)
			{
				HandleJobBegin();
				_firstIfExecuted = true;
				
				if (logJobStartAndEnd) LogWrapper("Job started at frame '{_frameCount}'.", LoggerAsset.LogType.Info);
			}

			// if the job has been started and the we've waited enough frames, end the job
			if (_firstIfExecuted && frameMod == resultWaitTime - 1)
			{
				HandleJobEnd();
				_firstIfExecuted = false;
				
				if (logJobStartAndEnd) LogWrapper("Job ended at frame '{_frameCount}'.", LoggerAsset.LogType.Info);
			}
		
			_frameCount++;
		}

		private void OnEnable()
		{
			_frameCount = 0;
			_firstIfExecuted = false;
		}
		
		private void OnDisable()
		{
			_runningJobHandle.Complete();
			if (!_runningJobHandle.IsCompleted) return;
		
			DisposeNativeArrays();
		}

		#endregion

		/// <summary>
		/// Handles the initialization of the job and the subsequent scheduling of the job.
		/// </summary>
		private void HandleJobBegin()
		{
			CacheRegistryValues();
			
			if (enemyRegistry.Count == 0 || playerRegistry.Count == 0) return;

			InitializeNativeArrays();
		
			FindClosestPlayerJob job = new FindClosestPlayerJob()
			{
				enemyPositions = _enemyPositions,
				playerData = _playerData,
				resultData = _resultData,
			};
			
			_runningJobHandle = job.ScheduleParallel(_cachedEnemies.Length, minBatchCount, new JobHandle());
		}

		/// <summary>
		/// Handles the completion of the job and the subsequent processing of the job results.
		/// </summary>
		private void HandleJobEnd()
		{
			_runningJobHandle.Complete();

			if (playerRegistry != null && playerRegistry.Count != 0 && _resultData.Length != 0)
			{
				for (int i = 0; i < _cachedEnemies.Length; i++)
				{
					var target = playerRegistry.GetValue(_resultData[i].ID);
					if (target == null || !target.gameObject.activeSelf)
					{
						LogWrapper($"Player with ID '{_resultData[i].ID}' not found or inactive.", LoggerAsset.LogType.Warning);
						continue;
					}
					_cachedEnemies[i].PathfindingComponent.SetTargetPosition(target.position);
				}
			}
			
			if (!_runningJobHandle.IsCompleted) return;
			
			DisposeNativeArrays();
		}
		
		/// <summary>
		/// Stores the current values of the enemy and player registries in external arrays.
		/// Useful to prevent constant registry lookups/accesses during the job.
		/// </summary>
		private void CacheRegistryValues()
		{
			_cachedEnemies = enemyRegistry.GetValuesArray();
			_cachedPlayers = (playerRegistry.GetKeysArray(), playerRegistry.GetValuesArray());
		}
		
		/// <summary>
		/// Converts the cached data into native arrays for use in the job.
		/// </summary>
		private void InitializeNativeArrays()
		{
			_enemyPositions = new NativeArray<Vector3>(_cachedEnemies.Length, Allocator.TempJob);
			_playerData = new NativeArray<PlayerData>(_cachedPlayers.id.Length, Allocator.TempJob);
			_resultData = new NativeArray<PlayerData>(_cachedEnemies.Length, Allocator.TempJob);
		
			// fill the native arrays with the cached data
			for (int i = 0; i < _cachedEnemies.Length; i++)
			{
				_enemyPositions[i] = _cachedEnemies[i].transform.position;
			}
			
			if (logNativeArrays) LogWrapper($"Native array '{nameof(_enemyPositions)}' initialized w/ {_cachedEnemies.Length} items.", LoggerAsset.LogType.Info);
		
			for (int i = 0; i < _cachedPlayers.id.Length; i++)
			{
				_playerData[i] = new PlayerData(_cachedPlayers.id[i], _cachedPlayers.body[i].position);
			}
			
			if (logNativeArrays) LogWrapper($"Native array '{nameof(_playerData)}' initialized w/ {_cachedPlayers.id.Length} items.", LoggerAsset.LogType.Info);
		}
		
		/// <summary>
		/// Disposes of the native arrays used in the job.
		/// Only disposes if the arrays have been created.
		/// </summary>
		private void DisposeNativeArrays()
		{
			if (_enemyPositions.IsCreated)
			{
				_enemyPositions.Dispose();
				if (logNativeArrays) LogWrapper($"Native array '{nameof(_enemyPositions)}' disposed.", LoggerAsset.LogType.Info);
			}
			
			if (_playerData.IsCreated)
			{
				_playerData.Dispose();
				if (logNativeArrays) LogWrapper($"Native array '{nameof(_playerData)}' disposed.", LoggerAsset.LogType.Info);
			}
			
			if (_resultData.IsCreated)
			{
				_resultData.Dispose();
				if (logNativeArrays) LogWrapper($"Native array '{nameof(_resultData)}' disposed.", LoggerAsset.LogType.Info);
			}
		}
		
		private void LogWrapper(string message, LoggerAsset.LogType type)
		{
			#if UNITY_EDITOR
			
			if (logger != null)
			{
				logger.Log(message, this, type);
			}
			
			#endif
		}
	}
}