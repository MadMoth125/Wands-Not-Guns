using System;
using System.Collections.Generic;
using System.Linq;
using Enemy.Registry;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyObjectPool))]
public class EnemySpawnHandler : MonoBehaviour
{
	[Tooltip("Whether or not the spawner is enabled.")]
	public bool enableSpawning = true;
	
	[Tooltip("The maximum number of enemies that can be spawned at once.")]
	public int maxEnemies = 100;

	[Tooltip("The number of enemies to spawn per second.")]
	public float spawnRate = 1f;
	
	[Tooltip("The default target for the enemies to follow.")]
	public Transform defaultTarget;
	
	[Tooltip("The list of spawn points for the enemies.")]
	public List<Transform> spawnPoints = new();

	private EnemyObjectPool _pool;
	private int _currentEnemies = 0;
	private float _spawnTimer = 0f;

	public void SpawnEnemy(Transform spawnPoint)
	{
		if (_currentEnemies >= maxEnemies) return;
		
		var spawnedEnemy = _pool.GetElement(spawnPoint);
		spawnedEnemy.SetTarget(defaultTarget);
		spawnedEnemy.HealthComponent.HealComplete();
		
		//enemyRegistry.Register(spawnedEnemy.gameObject.GetInstanceID(), spawnedEnemy);
		//_currentEnemies = enemyRegistry.Count;
		
		spawnedEnemy.HealthComponent.OnDie += OnEnemyDi;
	}

	private void OnEnemyDi()
	{
		
	}

	#region Unity Methods

	private void Awake()
	{
		_pool = GetComponent<EnemyObjectPool>();
	}

	private void Update()
	{
		if (!enableSpawning) return;
		if (maxEnemies <= 0) return;
		if (_currentEnemies >= maxEnemies) return;
		if (spawnPoints.Count == 0) return;
		
		if (_spawnTimer < 1f / spawnRate)
		{
			_spawnTimer += Time.deltaTime;
			return;
		}
		
		SpawnEnemy(spawnPoints[Random.Range(0, spawnPoints.Count)]);
		
		_spawnTimer = 0f;
	}

	#endregion
	
	private void OnEnemyDie(int instanceId)
	{
		/*var enemy = enemyRegistry.GetValue(instanceId);
		
		if (enemy != null)
		{
			enemy.OnDie -= OnEnemyDie;
			_pool.ReleaseElement(enemy);
		}
		
		enemyRegistry.Unregister(instanceId);
		_currentEnemies = enemyRegistry.Count;*/
	}
}