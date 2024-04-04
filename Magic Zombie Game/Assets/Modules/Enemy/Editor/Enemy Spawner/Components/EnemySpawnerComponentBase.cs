using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Registry;
using Player.Registry;

namespace Enemy.Spawner.Components
{
	public class EnemySpawnerComponentBase : IEnemySpawnerComponent
	{
		protected EnemyRegistryAsset EnemyRegistry => _enemyRegistry;
		protected PlayerRegistryAsset PlayerRegistry => _playerRegistry;
		protected EnemySpawnManager SpawnManager => _spawnManager;
		protected RoundValueAsset Round => _roundValue;
	
		private EnemyRegistryAsset _enemyRegistry;
		private PlayerRegistryAsset _playerRegistry;
		private EnemySpawnManager _spawnManager;
		private RoundValueAsset _roundValue;
	
		public void SetSpawnManager(EnemySpawnManager manager)
		{
			_spawnManager = manager;
		}
	
		public void SetRegistryAssets(EnemyRegistryAsset enemyRegistry, PlayerRegistryAsset playerRegistry, RoundValueAsset roundValue)
		{
			_enemyRegistry = enemyRegistry;
			_playerRegistry = playerRegistry;
			_roundValue = roundValue;
		}
	}
}