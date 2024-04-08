using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Player.Registry;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SpawnCountAsset", menuName = "Gameplay/Spawn Count Asset")]
public class SpawnCountScriptableObject : ScriptableObject
{
	[Tooltip("The maximum number of enemies that can be spawned and alive at once.")]
	[SerializeField]
	private int maxConcurrentEnemies = 32;

	[FormerlySerializedAs("roundValue")]
	[Required]
	[SerializeField]
	private RoundNumberScriptableObject roundNumberAsset;

	[Required]
	[SerializeField]
	private PlayerRegistry playerRegistry;

	/// <summary>
	/// Gets the maximum number of enemies that can be spawned for the current round.
	/// Uses the round value and player count to determine the number of enemies to spawn.
	/// </summary>
	public int GetMaxEnemyCount()
	{
		return GameFunction.GetSpawnCount(roundNumberAsset.Round, playerRegistry.Count);
	}
	
	/// <summary>
	/// Gets the maximum number of enemies that can be spawned for the current round.
	/// Allows for a custom round value and player count to determine the number of enemies to spawn.
	/// </summary>
	/// <param name="round">The round to get the spawn count for.</param>
	/// <param name="players">The number of players in the game.</param>
	public int GetMaxEnemyCount(int round, int players)
	{
		return GameFunction.GetSpawnCount(round, players);
	}
	
	/// <summary>
	/// The maximum number of enemies that can be spawned and alive at once.
	/// </summary>
	public int GetMaxConcurrentEnemyCount()
	{
		return maxConcurrentEnemies;
	}
}