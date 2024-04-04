using System;
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

	#endregion
	
	private EnemyObjectPool _pool;
	private EnemyRegistryHandler _registryHandler;
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
		var enemy = _pool.GetElement(position, rotation);
		_registryHandler.EnemySpawned(enemy);
		OnEnemySpawn?.Invoke(enemy);
		return enemy;
	}
	
	#region Unity Methods

	private void Awake()
	{
		_pool = GetComponent<EnemyObjectPool>();
		_registryHandler = new EnemyRegistryHandler(registry, _pool);
	}

	private void OnEnable()
	{
		_registryHandler.OnEnemyDie += (enemy) => OnEnemyDie?.Invoke(enemy);
		_manager.SpawnTimer.OnTimerElapsed += OnTimerElapsed;
	}

	private void OnDisable()
	{
		_registryHandler.ClearEvent();
		_manager.SpawnTimer.OnTimerElapsed -= OnTimerElapsed;
	}

	#endregion

	private void OnTimerElapsed()
	{
		if (_manager.EnemyCounter.ReachedMaxTotalEnemies())
		{
			Debug.LogWarning("Max total enemies reached, cannot spawn more.");
			return;
		}

		if (_manager.EnemyCounter.ReachedMaxConcurrentEnemies())
		{
			Debug.LogWarning("Max concurrent enemies reached, cannot spawn more.");
			return;
		}

		if (!_manager.ParentManager.RoundManager.ValidSpawnPeriod)
		{
			Debug.LogWarning("Invalid spawn period, cannot spawn more enemies.");
			return;
		}
		
		var point = _manager.SpawnPositions.GetValidSpawnPosition();
		SpawnEnemy(point);
	}

	/// <summary>
	/// Internal class to handle the enemy registry and OnDie event.
	/// Each enemy spawned is registered in the registry and unregistered when it dies.
	/// </summary>
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
		/// Clear any listeners from the OnEnemyDie event.
		/// </summary>
		public void ClearEvent()
		{
			var listeners = OnEnemyDie?.GetInvocationList();
			if (listeners == null) return;
			foreach (var del in listeners)
			{
				OnEnemyDie -= (Action<EnemyComponent>) del;
			}
		}
		
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