using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPositions : MonoBehaviour, IEnemyManagerComponent
{
	[SerializeField]
	private List<Transform> spawnPositions = new();
	
	private EnemyManager _manager;
	
	public void SetManager(EnemyManager manager)
	{
		_manager = manager;
	}
	
	public Transform GetValidSpawnPosition()
	{
		return GetRandomSpawnPosition();
	}
	
	public Transform GetRandomSpawnPosition()
	{
		return spawnPositions[Random.Range(0, spawnPositions.Count)];
	}

	#region Unity Methods

	private void Awake()
	{
		// Remove any null or inactive spawn positions
		spawnPositions.ForEach(item =>
		{
			if (item == null || !item.gameObject.activeSelf) spawnPositions.Remove(item);
		});
	}

	#endregion
}