using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class EnemyManager : MonoBehaviour, IManagerComponent<GameManager>
{
	#region Properties

	public bool SpawningEnabled => enableSpawning;
	public bool ConcurrentLimitEnforced => enableConcurrentLimit;
	public GameManager ParentManager => _manager;
	public EnemySpawner Spawner => spawner;
	public EnemySpawnTimer SpawnTimer => spawnTimer;
	public EnemyCounter EnemyCounter => enemyCounter;
	public EnemySpawnPositions SpawnPositions => spawnPositions;

	#endregion

	#region Fields

	[Tooltip("Whether to allow enemy spawning.")]
	public bool enableSpawning = true;
	
	[Tooltip("Whether to limit the number of enemies that can be spawned at once." +
	         "If false, enemies will only stop spawning when the total enemy limit is reached.")]
	public bool enableConcurrentLimit = true;
	
	[ExternalComponentCategory]
	[Required]
	[SerializeField]
	private EnemySpawner spawner;
	
	[ExternalComponentCategory]
	[Required]
	[SerializeField]
	private EnemySpawnTimer spawnTimer;
	
	[ExternalComponentCategory]
	[Required]
	[SerializeField]
	private EnemyCounter enemyCounter;
	
	[ExternalComponentCategory]
	[Required]
	[SerializeField]
	private EnemySpawnPositions spawnPositions;

	#endregion
	
	private List<IManagerComponent<EnemyManager>> _components;
	private GameManager _manager;

	public void SetParentManager(GameManager manager)
	{
		_manager = manager;
	}

	#region Unity Methods

	private void Awake()
	{
		_components = new List<IManagerComponent<EnemyManager>>
		{
			spawner,
			spawnTimer,
			enemyCounter,
			spawnPositions,
		};

		foreach (var component in _components)
		{
			component.SetParentManager(this);
		}
	}

	#endregion
}