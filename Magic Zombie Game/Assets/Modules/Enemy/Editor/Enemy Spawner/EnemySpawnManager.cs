using System;
using System.Collections.Generic;
using Enemy.Registry;
using Enemy.Spawner.Components;
using Player.Registry;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy.Spawner
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EnemyObjectPool))]
	public class EnemySpawnManager : MonoBehaviour
	{
		#region Properties

		public EnemyObjectPool Pool => _pool;
		public RoundValueAsset Round => roundValueAsset;
		public EnemyRegistryHandler RegistryHandler => _registryHandler;
		public SpawnHandler SpawnerHandler => spawnerHandler;
		public SpawnCountTracker SpawnCountTracker => spawnCountTracker;
		public SpawnTimer SpawnTimer => spawnTimer;

		#endregion
		
		private EnemyObjectPool _pool;

		[TitleGroup("Registries")]
		[Required]
		[SerializeField]
		private EnemyRegistryAsset enemyRegistry;

		[TitleGroup("Registries")]
		[Required]
		[SerializeField]
		private PlayerRegistryAsset playerRegistry;

		[TitleGroup("Round")]
		[Required]
		[SerializeField]
		private RoundValueAsset roundValueAsset;
		
		[TabGroup("Spawner")]
		[HideLabel]
		[InlineProperty]
		[SerializeField]
		private SpawnHandler spawnerHandler = new();

		[TabGroup("Counter")]
		[HideLabel]
		[InlineProperty]
		[SerializeField]
		private SpawnCountTracker spawnCountTracker = new();

		[TabGroup("Timer")]
		[HideLabel]
		[InlineProperty]
		[SerializeField]
		private SpawnTimer spawnTimer = new();

		private EnemyRegistryHandler _registryHandler = new();
		private List<IEnemySpawnerComponent> _enemySpawnerComponents;
	
		#region Unity Methods

		private void Awake()
		{
			_pool = GetComponent<EnemyObjectPool>();
		
			_enemySpawnerComponents = new List<IEnemySpawnerComponent>
			{
				spawnCountTracker,
				spawnTimer,
				spawnerHandler,
				_registryHandler
			};

			foreach (var component in _enemySpawnerComponents)
			{
				component.SetSpawnManager(this);
				component.SetRegistryAssets(enemyRegistry, playerRegistry, roundValueAsset);
			}
		}

		private void Update()
		{
			spawnTimer.Tick(Time.deltaTime);
		}
	
		private void OnEnable()
		{
			spawnTimer.OnTimerTick += SpawnTick;
			spawnerHandler.OnEnemySpawn += _registryHandler.Register;
			spawnerHandler.OnEnemySpawn += HandleIncreaseEnemyCount;
			spawnerHandler.OnEnemyDie += _registryHandler.Unregister;
			spawnerHandler.OnEnemyDie += HandleDecreaseEnemyCount;
		}

		private void OnDisable()
		{
			spawnTimer.OnTimerTick -= SpawnTick;
			spawnerHandler.OnEnemySpawn -= _registryHandler.Register;
			spawnerHandler.OnEnemySpawn -= HandleIncreaseEnemyCount;
			spawnerHandler.OnEnemyDie -= _registryHandler.Unregister;
			spawnerHandler.OnEnemyDie -= HandleDecreaseEnemyCount;
		}

		#endregion

		private void SpawnTick(float time)
		{
			if (!roundValueAsset.roundActive) return;
			if (spawnCountTracker.GetTotalEnemyCount() >= spawnCountTracker.GetMaxTotalEnemies()) return;
			if (spawnCountTracker.GetEnemyCount() >= spawnCountTracker.GetMaxConcurrentEnemies()) return;
		
			var spawnPoint = spawnerHandler.GetRandomSpawnPoint();
			spawnerHandler.SpawnEnemy(spawnPoint);
		}
	
		private void HandleIncreaseEnemyCount(EnemyComponent enemy)
		{
			spawnCountTracker.IncrementEnemyCount();
		}
	
		private void HandleDecreaseEnemyCount(EnemyComponent enemy)
		{
			spawnCountTracker.DecrementEnemyCount();
		}
	}
}