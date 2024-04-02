using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Spawner.Components
{
	[Serializable]
	public class SpawnHandler : EnemySpawnerComponentBase
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
	
		public IEnumerable<Transform> SpawnPoints => spawnPoints;
	
		[SerializeField]
		private List<Transform> spawnPoints = new();
	
		/// <summary>
		/// Spawn an enemy at the given spawn point, using the point's position and rotation.
		/// </summary>
		/// <param name="spawnPoint">The spawn point to spawn the enemy at.</param>
		public void SpawnEnemy(Transform spawnPoint)
		{
			if (spawnPoint == null)
			{
				Debug.LogWarning("Spawn point is null, cannot spawn enemy.");
				return;
			}
			SpawnEnemy(spawnPoint.position, spawnPoint.rotation);
		}

		/// <summary>
		/// Spawn an enemy at the given position with default rotation.
		/// </summary>
		/// <param name="position">The position to spawn the enemy at.</param>
		public void SpawnEnemy(Vector3 position)
		{
			SpawnEnemy(position, Quaternion.identity);
		}

		/// <summary>
		/// Spawn an enemy at the given position and rotation.
		/// </summary>
		/// <param name="position">The position to spawn the enemy at.</param>
		/// <param name="rotation">The rotation to spawn the enemy with.</param>
		public void SpawnEnemy(Vector3 position, Quaternion rotation)
		{
			var enemy = SpawnManager.Pool.GetElement(position, rotation);
			enemy.HealthComponent.HealComplete();
			enemy.OnDie += EnemyDeathListener;
			OnEnemySpawn?.Invoke(enemy);
		}
	
		public Transform GetRandomSpawnPoint()
		{
			if (spawnPoints.Count == 0)
			{
				Debug.LogWarning("No spawn points available.");
				return null;
			}
			return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
		}
	
		/// <summary>
		/// Listener for when an enemy dies.
		/// Handles releasing the enemy back to the pool and invoking the <see cref="OnEnemyDie"/> event.
		/// </summary>
		private void EnemyDeathListener(EnemyComponent enemy)
		{
			SpawnManager.Pool.ReleaseElement(enemy);
			enemy.OnDie -= EnemyDeathListener;
			OnEnemyDie?.Invoke(enemy);
		}
	}
}