using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy.Spawner.Components
{
	[Serializable]
	public class SpawnCountTracker : EnemySpawnerComponentBase
	{
		public event Action OnTotalEnemyCountReached;
		public event Action OnConcurrentEnemyCountReached;
		
		public SpawnCountAsset SpawnCountAsset => spawnCountAsset;

		[InlineEditor(InlineEditorObjectFieldModes.Boxed)]
		[SerializeField]
		private SpawnCountAsset spawnCountAsset;
	
		private int _currentEnemyCount;
		private int _totalEnemyCount;
	
		/// <summary>
		/// The current number of enemies spawned.
		/// </summary>
		/// <returns></returns>
		public int GetEnemyCount()
		{
			return _currentEnemyCount;
		}
	
		/// <summary>
		/// The total number of enemies spawned in the round.
		/// </summary>
		/// <returns></returns>
		public int GetTotalEnemyCount()
		{
			return _totalEnemyCount;
		}
	
		/// <summary>
		/// The maximum number of enemies that can be spawned at once.
		/// </summary>
		/// <returns></returns>
		public int GetMaxConcurrentEnemies()
		{
			return spawnCountAsset.GetMaxConcurrentEnemyCount();
		}
	
		/// <summary>
		/// the total number of enemies to spawn for the round.
		/// </summary>
		/// <returns></returns>
		public int GetMaxTotalEnemies()
		{
			return spawnCountAsset.GetMaxEnemyCount();
		}

		/// <summary>
		/// Set the total of how many enemies have been spawned.
		/// Value is clamped between 0 and the maximum number of enemies that can be spawned for the round.
		/// </summary>
		/// <param name="count">The total number of enemies spawned.</param>
		public void SetTotalEnemyCount(int count)
		{
			_totalEnemyCount = Mathf.Clamp(count, 0, GetMaxTotalEnemies());
			if (_totalEnemyCount >= GetMaxTotalEnemies())
			{
				OnTotalEnemyCountReached?.Invoke();
			}
		}
	
		/// <summary>
		/// Increments the current enemy count.
		/// Also increments the total enemy count.
		/// </summary>
		public void IncrementEnemyCount()
		{
			_currentEnemyCount++;
			_totalEnemyCount++;
		}

		/// <summary>
		/// Decrements the current enemy count.
		/// </summary>
		public void DecrementEnemyCount()
		{
			_currentEnemyCount--;
		}

		/// <summary>
		/// Sets the current and total enemy count to 0.
		/// </summary>
		public void ResetCounts()
		{
			_currentEnemyCount = 0;
			_totalEnemyCount = 0;
		}
	}
}