using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class SpawnCountTracker : EnemySpawnerComponentBase
{
	public SpawnCountAsset SpawnCountAsset => spawnCountAsset;

	[InlineEditor(InlineEditorObjectFieldModes.Foldout)]
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
		return spawnCountAsset.GetMaxConcurrentEnemies();
	}
	
	/// <summary>
	/// the total number of enemies to spawn for the round.
	/// </summary>
	/// <returns></returns>
	public int GetMaxTotalEnemies()
	{
		return spawnCountAsset.GetMaxSpawnCount();
	}

	/// <summary>
	/// Set the total of how many enemies have been spawned.
	/// </summary>
	/// <param name="count">The total number of enemies spawned.</param>
	public void SetTotalEnemyCount(int count)
	{
		_totalEnemyCount = count;
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