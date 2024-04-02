using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyCountHandler : MonoBehaviour
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
	
	public void EnemyCountIncreased(EnemyComponent enemy)
	{
		_totalEnemyCount++;
		_enemyCount++;

		if (MaxConcurrentEnemies())
		{
			OnConcurrentMaxEnemiesReached?.Invoke();
		}
		
		if (MaxTotalEnemies())
		{
			OnTotalMaxEnemiesReached?.Invoke();
		}
	}
	
	public void EnemyCountDecreased(EnemyComponent enemy)
	{
		_enemyCount--;
	}

	/// <summary>
	/// Whether the max number of active/concurrent enemies has been reached.
	/// </summary>
	public bool MaxConcurrentEnemies()
	{
		return _enemyCount >= spawnCountAsset.GetMaxConcurrentEnemies();
	}
	
	/// <summary>
	/// Whether the total number of enemies to spawn for the round has been reached.
	/// </summary>
	/// <returns></returns>
	public bool MaxTotalEnemies()
	{
		return _totalEnemyCount >= spawnCountAsset.GetSpawnCount();
	}
}