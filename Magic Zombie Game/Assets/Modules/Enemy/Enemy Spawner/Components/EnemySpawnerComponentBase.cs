using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Registry;
using Player.Registry;

public class EnemySpawnerComponentBase : IEnemySpawnerComponent
{
	protected EnemyRegistryAsset EnemyRegistry => _enemyRegistry;
	protected PlayerRegistryAsset PlayerRegistry => _playerRegistry;
	protected EnemySpawnManager SpawnManager => _spawnManager;
	
	private EnemyRegistryAsset _enemyRegistry;
	private PlayerRegistryAsset _playerRegistry;
	private EnemySpawnManager _spawnManager;
	
	public void SetSpawnManager(EnemySpawnManager manager)
	{
		_spawnManager = manager;
	}
	
	public void SetRegistryAssets(EnemyRegistryAsset enemyRegistry, PlayerRegistryAsset playerRegistry)
	{
		_enemyRegistry = enemyRegistry;
		_playerRegistry = playerRegistry;
	}
}