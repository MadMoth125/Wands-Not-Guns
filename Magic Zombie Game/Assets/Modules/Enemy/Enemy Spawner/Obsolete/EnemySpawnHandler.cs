using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Obsolete]
public class EnemySpawnHandler : MonoBehaviour
{
	#region Events

	/// <summary>
	/// Invoked when an enemy is spawned.
	/// </summary>
	public event Action<EnemyComponent> OnEnemySpawn;
	
	/// <summary>
	/// Invoked when an enemy dies.
	/// </summary>
	public event Action<EnemyComponent> OnEnemyDie;

	#endregion
	
	[Required]
	[SerializeField]
	private EnemySpawnManager spawnManager;

	[Tooltip("The list of spawn points for the enemies.")]
	public List<Transform> spawnPoints = new();
	
	public void SpawnEnemy(Transform spawnPoint)
	{
		SpawnEnemy(spawnPoint.position, spawnPoint.rotation);
	}

	public void SpawnEnemy(Vector3 position, Quaternion rotation)
	{
		var spawnedEnemy = spawnManager.Pool.GetElement(position, rotation);
		spawnedEnemy.HealthComponent.HealComplete();
		
		spawnedEnemy.OnDie += EnemyDeathListener;
		OnEnemySpawn?.Invoke(spawnedEnemy);
	}

	private void EnemyDeathListener(EnemyComponent enemy)
	{
		// int id = enemy.gameObject.GetInstanceID();
		spawnManager.Pool.ReleaseElement(enemy);
		
		enemy.OnDie -= EnemyDeathListener;
		OnEnemyDie?.Invoke(enemy);
	}
}