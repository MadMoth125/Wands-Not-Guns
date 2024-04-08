using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemySpawnPositions : MonoBehaviour, IManagerComponent<EnemyManager>
{
	[SerializeField]
	private List<Transform> spawnPositions = new();
	
	private EnemyManager _manager;
	
	public void SetParentManager(EnemyManager manager)
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
		if (spawnPositions.Count == 0)
		{
			Debug.LogError("No spawn positions found in " + name);
		}
		
		// Remove any null or inactive spawn positions
		spawnPositions.ForEach(item =>
		{
			if (item == null || !item.gameObject.activeSelf)
			{
				Debug.LogWarning("Removing invalid spawn position");
				spawnPositions.Remove(item);
			}
		});
	}

	#endregion
}