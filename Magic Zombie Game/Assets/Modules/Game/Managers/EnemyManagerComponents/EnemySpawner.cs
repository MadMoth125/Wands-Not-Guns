using System;
using Core.CustomDebugger;
using Enemy.Registry;
using Obvious.Soap;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(EnemyObjectPool))]
public class EnemySpawner : MonoBehaviour, IManagerComponent<EnemyManager>
{
	#region Properties

	public EnemyRegistry EnemyRegistry => enemyRegistry;
	public EnemyObjectPool EnemyPool => _pool;

	#endregion

	#region Fields

	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventInt enemySpawnEvent;
	
	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventInt enemyDieEvent;
	
	[RegistryCategory]
	[Required]
	[SerializeField]
	private EnemyRegistry enemyRegistry;
	
	[TitleGroup("Debug", "Parameters", Alignment = TitleAlignments.Centered)]
	[SerializeField]
	private LoggerScriptableObject logger;
	
	[TitleGroup("Debug", "Parameters", Alignment = TitleAlignments.Centered)]
	[Tooltip("Enable logging of when enemies are spawned and die.")]
	[SerializeField]
	private bool logEnemyLifecycle = false;

	[TitleGroup("Debug", "Parameters", Alignment = TitleAlignments.Centered)]
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
		enemyRegistry.Register(enemy.EnemyId, enemy); // register
		
		enemyDieEvent.OnRaised += EnemyDieListener; // subscribe to die event
		enemySpawnEvent.Raise(enemy.EnemyId); // invoke spawn event
		
		if (logEnemyLifecycle) LogWrapper($"Enemy '{enemy.EnemyId}' spawned.", LoggerType.Info);
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
	private void EnemyDieListener(int enemyId)
	{
		var enemy = enemyRegistry.GetValue(enemyId);
		if (enemy == null) return;
		
		enemyRegistry.Unregister(enemy.EnemyId); // unregister
		enemyDieEvent.OnRaised -= EnemyDieListener;
		
		_pool.ReleaseElement(enemy); // release to pool
		
		if (logEnemyLifecycle) LogWrapper($"Enemy '{enemy.EnemyId}' died.", LoggerType.Info);
	}

	/// <summary>
	/// Conditions to check if an enemy can be spawned.
	/// </summary>
	/// <returns>Whether an enemy can be spawned.</returns>
	private bool CanSpawnEnemy()
	{
		if (!_manager.SpawningEnabled)
		{
			if (logSpawnState) LogWrapper("Spawning is disabled.", LoggerType.Warning);
			return false;
		}
		
		if (_manager.EnemyCounter.ReachedMaxTotalEnemies())
		{
			if (logSpawnState) LogWrapper("Max total enemy count reached, cannot spawn more.", LoggerType.Warning);
			return false;
		}

		if (_manager.ConcurrentLimitEnforced && _manager.EnemyCounter.ReachedMaxConcurrentEnemies())
		{
			if (logSpawnState) LogWrapper("Max concurrent enemy count reached, cannot spawn more.", LoggerType.Warning);
			return false;
		}

		if (!_manager.ParentManager.RoundManager.ValidSpawnPeriod)
		{
			if (logSpawnState) LogWrapper("Invalid spawn period.", LoggerType.Warning);
			return false;
		}

		return true;
	}

	private void LogWrapper(string message, LoggerType type)
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
		public EnemyRegistryHandler(EnemyRegistry registry, EnemyObjectPool pool)
		{
			_registry = registry;
			_pool = pool;
		}
		
		public event Action<EnemyComponent> OnEnemyDie;
		
		private readonly EnemyRegistry _registry;
		private readonly EnemyObjectPool _pool;

		/// <summary>
		/// Handle the spawning of an enemy and addition to the registry.
		/// </summary>
		public void EnemySpawned(EnemyComponent enemy)
		{
			_registry.Register(enemy.EnemyId, enemy);
			// enemy.OnDie += OnDieListener;
		}
		
		/// <summary>
		/// Handles the post-death cleanup of an enemy.
		/// </summary>
		/// <param name="enemy"></param>
		private void OnDieListener(EnemyComponent enemy)
		{
			_registry.Unregister(enemy.EnemyId);
			_pool.ReleaseElement(enemy);
			// enemy.OnDie -= OnDieListener;
			OnEnemyDie?.Invoke(enemy);
		}
	}
}