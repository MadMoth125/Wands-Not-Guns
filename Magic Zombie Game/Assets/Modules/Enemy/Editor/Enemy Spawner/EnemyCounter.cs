using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy.Spawner
{
	public class EnemyCounter : MonoBehaviour
	{
		/// <summary>
		/// Invoked when the total number of enemies to spawn for the round has been reached.
		/// </summary>
		public event Action OnTotalMaxEnemiesReached;
	
		/// <summary>
		/// Invoked when the max number of active enemies has been reached.
		/// </summary>
		public event Action OnConcurrentMaxEnemiesReached;
	
		[Required]
		[SerializeField]
		private SpawnCountAsset spawnCountAsset;
	
		[Required]
		[SerializeField]
		private EnemySpawnManager spawnManager;
	
		private int _enemyCount;
		private int _totalEnemyCount;
	
		public void ResetCount()
		{
			_enemyCount = 0;
			_totalEnemyCount = 0;
		}
	
		public void IncrementEnemyCount()
		{
			_totalEnemyCount++;
			_enemyCount++;

			if (ReachedMaxConcurrentEnemies())
			{
				OnConcurrentMaxEnemiesReached?.Invoke();
			}
		
			if (ReachedMaxTotalEnemies())
			{
				OnTotalMaxEnemiesReached?.Invoke();
			}
		}
	
		public void DecrementEnemyCount()
		{
			_enemyCount--;
		}

		public bool CanSpawnEnemy()
		{
			bool maxConcurrent = _enemyCount >= spawnCountAsset.GetMaxConcurrentEnemyCount();
			bool maxTotal = _totalEnemyCount >= spawnCountAsset.GetMaxEnemyCount();
			
			return !maxConcurrent && !maxTotal;
		}
		
		/// <summary>
		/// Whether the max number of active/concurrent enemies has been reached.
		/// </summary>
		public bool ReachedMaxConcurrentEnemies()
		{
			return _enemyCount >= spawnCountAsset.GetMaxConcurrentEnemyCount();
		}
	
		public int GetMaxConcurrentEnemies()
		{
			return spawnCountAsset.GetMaxConcurrentEnemyCount();
		}
	
		/// <summary>
		/// Whether the total number of enemies to spawn for the round has been reached.
		/// </summary>
		/// <returns></returns>
		public bool ReachedMaxTotalEnemies()
		{
			return _totalEnemyCount >= spawnCountAsset.GetMaxEnemyCount();
		}
	
		public int GetMaxTotalEnemies()
		{
			return spawnCountAsset.GetMaxEnemyCount();
		}
	
		public int GetTotalEnemyCount()
		{
			return _totalEnemyCount;
		}
	}
}