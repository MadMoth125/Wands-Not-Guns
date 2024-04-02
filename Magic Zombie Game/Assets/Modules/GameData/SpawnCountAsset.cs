using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Player.Registry;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnCountAsset", menuName = "Gameplay/Spawn Count Asset")]
public class SpawnCountAsset : ScriptableObject
{
	[Tooltip("The maximum number of enemies that can be spawned and alive at once.")]
	[SerializeField]
	private int maxConcurrentEnemies = 32;

	[SerializeField]
	private RoundValueAsset roundValue;

	[SerializeField]
	private PlayerRegistryAsset playerRegistry;

	public int GetSpawnCount()
	{
		return GameFunction.GetSpawnCount(roundValue.Round, playerRegistry.Count);
	}
	
	public int GetSpawnCount(int round, int players)
	{
		return GameFunction.GetSpawnCount(round, players);
	}
	
	public int GetMaxConcurrentEnemies()
	{
		return maxConcurrentEnemies;
	}
}