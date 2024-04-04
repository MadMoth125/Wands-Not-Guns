using System;
using Core.CustomDebugger;
using Enemy.Registry;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(EnemyObjectPool))]
public class EnemySpawner : MonoBehaviour, IManagerComponent<EnemyManager>
{
	#region Events

	public event Action<EnemyComponent> OnEnemySpawn;
	public event Action<EnemyComponent> OnEnemyDie;

	#endregion

	#region Properties

	public EnemyRegistryAsset Registry => registry;
	public EnemyObjectPool EnemyPool => _pool;

	#endregion

	#region Fields

	[Required]
	[SerializeField]
	private EnemyRegistryAsset registry;
	
	[TitleGroup("Debug", "Parameters", Alignment = TitleAlignments.Centered)]
	[SerializeField]
	private LoggerAsset logger;
	
	[Tooltip("Enable logging of when enemies are spawned and die.")]
	[SerializeField]
	private bool logEnemyLifecycle = false;

	[Tooltip("Enable logging of issues with spawning (if there are any).")]
	[SerializeField]
	private bool logSpawnState = false;

	#endregion
	
	private EnemyObjectPool _pool;
	private EnemyManager _manager;
	
	public void SetParentManager(EnemyManager manager)
	{
		_manager = manager;
	}
	
	public EnemyComponent SpawnEnemy(Transform point)
	{
		return SpawnEnemy(point.position, point.rotation);
	}
	
	public EnemyComponent SpawnEnemy(Vector3 position)
	{
		return SpawnEnemy(position, Quaternion.identity);
	}
	
	public EnemyComponent SpawnEnemy(Vector3 position, Quaternion rotation)
	{
		var enemy = _pool.GetElement(position, rotation); // pull from pool
		registry.Register(enemy.EnemyId, enemy); // register
		enemy.OnDie += EnemyDieListener; // subscribe to die event
		OnEnemySpawn?.Invoke(enemy); // invoke spawn event
		if (logEnemyLifecycle) LogWrapper($"Enemy '{enemy.EnemyId}' spawned.", LoggerAsset.LogType.Info);
		return enemy; // return value
	}
	
	#region Unity Methods

	private void Awake()
	{
		_pool = GetComponent<EnemyObjectPool>();
	}

	private void OnEnable()
	{
		_manager.SpawnTimer.OnTimerElapsed += OnTimerElapsed;
	}

	private void OnDisable()
	{
		_manager.SpawnTimer.OnTimerElapsed -= OnTimerElapsed;
	}

	#endregion

	private void OnTimerElapsed()
	{
		if (!CanSpawnEnemy()) return;
		
		var point = _manager.SpawnPositions.GetValidSpawnPosition();
		SpawnEnemy(point);
	}

	/// <summary>
	/// Handles the post-death cleanup of an enemy.
	/// </summary>
	private void EnemyDieListener(EnemyComponent enemy)
	{
		registry.Unregister(enemy.EnemyId); // unregister
		_pool.ReleaseElement(enemy); // release to pool
		enemy.OnDie -= EnemyDieListener; // unsubscribe from die event
		OnEnemyDie?.Invoke(enemy); // invoke die event
		if (logEnemyLifecycle) LogWrapper($"Enemy '{enemy.EnemyId}' died.", LoggerAsset.LogType.Info);
	}

	/// <summary>
	/// Conditions to check if an enemy can be spawned.
	/// </summary>
	/// <returns>Whether an enemy can be spawned.</returns>
	private bool CanSpawnEnemy()
	{
		if (!_manager.SpawningEnabled)
		{
			if (logSpawnState) LogWrapper("Spawning is disabled.", LoggerAsset.LogType.Warning);
			return false;
		}
		
		if (_manager.EnemyCounter.ReachedMaxTotalEnemies())
		{
			if (logSpawnState) LogWrapper("Max total enemy count reached, cannot spawn more.", LoggerAsset.LogType.Warning);
			return false;
		}

		if (_manager.ConcurrentLimitEnforced && _manager.EnemyCounter.ReachedMaxConcurrentEnemies())
		{
			if (logSpawnState) LogWrapper("Max concurrent enemy count reached, cannot spawn more.", LoggerAsset.LogType.Warning);
			return false;
		}

		if (!_manager.ParentManager.RoundManager.ValidSpawnPeriod)
		{
			if (logSpawnState) LogWrapper("Invalid spawn period.", LoggerAsset.LogType.Warning);
			return false;
		}

		return true;
	}

	private void LogWrapper(string message, LoggerAsset.LogType type)
	{
		if (logger != null)
		{
			logger.Log(message, this, type);
		}
	}
	
	/// <summary>
	/// Internal class to handle the enemy registry and OnDie event.
	/// Each enemy spawned is registered in the registry and unregistered when it dies.
	/// </summary>
	[Obsolete]
	private class EnemyRegistryHandler
	{
		public EnemyRegistryHandler(EnemyRegistryAsset registry, EnemyObjectPool pool)
		{
			_registry = registry;
			_pool = pool;
		}
		
		public event Action<EnemyComponent> OnEnemyDie;
		
		private readonly EnemyRegistryAsset _registry;
		private readonly EnemyObjectPool _pool;

		/// <summary>
		/// Handle the spawning of an enemy and addition to the registry.
		/// </summary>
		public void EnemySpawned(EnemyComponent enemy)
		{
			_registry.Register(enemy.EnemyId, enemy);
			enemy.OnDie += OnDieListener;
		}
		
		/// <summary>
		/// Handles the post-death cleanup of an enemy.
		/// </summary>
		/// <param name="enemy"></param>
		private void OnDieListener(EnemyComponent enemy)
		{
			_registry.Unregister(enemy.EnemyId);
			_pool.ReleaseElement(enemy);
			enemy.OnDie -= OnDieListener;
			OnEnemyDie?.Invoke(enemy);
		}
	}
}