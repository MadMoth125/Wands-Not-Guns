using System;
using System.Linq;
using Core.CustomDebugger;
using Core.CustomTickSystem;
using Enemy.Registry;
using Player.Registry;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy.Jobs
{
	public class EnemyTargetManager : MonoBehaviour
	{
		#region Fields

		[RegistryCategory]
		[Required]
		[SerializeField]
		private EnemyRegistry enemyRegistry;
	
		[RegistryCategory]
		[Required]
		[SerializeField]
		private PlayerRegistry playerRegistry;

		[TitleGroup("Jobs", "Parameters", Alignment = TitleAlignments.Centered)]
		[Tooltip("The minimum number of enemies to process in a single batch.\n" +
		         "Higher values may improve performance for larger enemy groups.")]
		[DisableInPlayMode]
		[SerializeField]
		private int batchCount = 10;
		
		[TitleGroup("Jobs")]
		[Tooltip("The number of frames to wait before completing the job.\n" +
		         "Useful for giving the job more time to calculate & complete the given task.\n\n" +
		         "The '<b>Allocator.TempJob</b>' type requires that the job finishes within 4 frames, so this value should be no higher than 4.")]
		[DisableInPlayMode]
		[PropertyRange(1, 4)]
		[SerializeField]
		private int completionWaitTime = 4;
		
		[TitleGroup("Debug", "Parameters", Alignment = TitleAlignments.Centered)]
		[SerializeField]
		private LoggerScriptableObject logger;

		[TitleGroup("Debug")]
		[Tooltip("Enable logging of when the job starts and ends.")]
		[SerializeField]
		private bool logJobStartAndEnd = false;
		
		[TitleGroup("Debug")]
		[Tooltip("Enable logging of the native arrays and their state.")]
		[SerializeField]
		private bool logNativeArrays = false;

		#endregion

		private int _frameCount = 0;
		private bool _jobActive = false;
		
		private EnemyComponent[] _cachedEnemies;
		private PlayerData[] _cachedPlayerData;
		
		private NativeArray<Vector3> _enemyPositions;
		private NativeArray<PlayerData> _playerData;
		private NativeArray<PlayerData> _resultData;
		private FindClosestPlayerJob _findPlayerJob;
		private JobHandle _runningJobHandle;
		private readonly JobHandle _dependencyJobHandleHandle = new();

		#region Unity Methods

		private void Awake()
		{
			_findPlayerJob = new FindClosestPlayerJob();
		}

		private void OnEnable()
		{
			_frameCount = 0;
			TickSystem.AddListener("EnemyTarget", OnEnemyTick);
		}

		private void OnDisable()
		{
			TickSystem.RemoveListener("EnemyTarget", OnEnemyTick);
			_runningJobHandle.Complete();
			if (!_runningJobHandle.IsCompleted) return;
		
			DisposeNativeArrays();
		}

		private void Update()
		{
			if (_jobActive)
			{
				if (_frameCount == completionWaitTime)
				{
					HandleJobEnd();
					_jobActive = false;
					_frameCount = 0;
					if (logJobStartAndEnd) LogWrapper($"Job ended at frame '{Time.frameCount}'.", LoggerType.Info);
				}
				else
				{
					_frameCount++;
				}
			}
		}

		#endregion

		private void OnEnemyTick()
		{
			HandleJobBegin();
			_jobActive = true;
			if (logJobStartAndEnd) LogWrapper($"Job started at frame '{Time.frameCount}'.", LoggerType.Info);
		}
		
		/// <summary>
		/// Handles the initialization of the job and the subsequent scheduling of the job.
		/// </summary>
		private void HandleJobBegin()
		{
			CacheRegistryValues();
			
			if (enemyRegistry.Count == 0 || playerRegistry.Count == 0) return;
			
			InitializeNativeArrays();
			
			// set the job data
			_findPlayerJob.SetData(_enemyPositions, _playerData, _resultData);
			
			_runningJobHandle = _findPlayerJob.ScheduleParallel(_cachedEnemies.Length, batchCount, _dependencyJobHandleHandle);
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
						LogWrapper($"Player with ID '{_resultData[i].ID}' not found or inactive.", LoggerType.Warning);
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
			
			int[] playerKeys = playerRegistry.GetKeysArray();
			Transform[] playerValues = playerRegistry.GetValuesArray();
			_cachedPlayerData = new PlayerData[playerKeys.Length];
			for (int i = 0; i < _cachedPlayerData.Length; i++)
			{
				_cachedPlayerData[i] = new PlayerData(playerKeys[i], playerValues[i].position);
			}
		}
		
		/// <summary>
		/// Converts the cached data into native arrays for use in the job.
		/// </summary>
		private void InitializeNativeArrays()
		{
			_enemyPositions = new NativeArray<Vector3>(_cachedEnemies.Length, Allocator.TempJob);
			_playerData = new NativeArray<PlayerData>(_cachedPlayerData.Length, Allocator.TempJob);
			_resultData = new NativeArray<PlayerData>(_cachedEnemies.Length, Allocator.TempJob);

			// fill the native arrays with the cached data
			for (int i = 0; i < _cachedEnemies.Length; i++)
			{
				_enemyPositions[i] = _cachedEnemies[i].transform.position;
			}
			
			if (logNativeArrays) LogWrapper($"Native array '{nameof(_enemyPositions)}' initialized w/ {_cachedEnemies.Length} items.", LoggerType.Info);
		
			PlayerData reusablePlayerData = new();
			for (int i = 0; i < _cachedPlayerData.Length; i++)
			{
				reusablePlayerData.SetData(_cachedPlayerData[i].ID, _cachedPlayerData[i].Position);
				_playerData[i] = reusablePlayerData;
			}
			
			if (logNativeArrays) LogWrapper($"Native array '{nameof(_playerData)}' initialized w/ {_cachedPlayerData.Length} items.", LoggerType.Info);
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
				if (logNativeArrays) LogWrapper($"Native array '{nameof(_enemyPositions)}' disposed.", LoggerType.Info);
			}
			
			if (_playerData.IsCreated)
			{
				_playerData.Dispose();
				if (logNativeArrays) LogWrapper($"Native array '{nameof(_playerData)}' disposed.", LoggerType.Info);
			}
			
			if (_resultData.IsCreated)
			{
				_resultData.Dispose();
				if (logNativeArrays) LogWrapper($"Native array '{nameof(_resultData)}' disposed.", LoggerType.Info);
			}
		}
		
		private void LogWrapper(string message, LoggerType type)
		{
			if (logger != null)
			{
				logger.Log(message, this, type);
			}
		}
	}
}