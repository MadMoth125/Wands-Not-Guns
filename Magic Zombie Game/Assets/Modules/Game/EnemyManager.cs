using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class EnemyManager : MonoBehaviour, IManagerComponent<GameManager>
{
	#region Properties

	public GameManager ParentManager => _manager;
	public EnemySpawner Spawner => spawner;
	public EnemySpawnTimer SpawnTimer => spawnTimer;
	public EnemyCounter EnemyCounter => enemyCounter;
	public EnemySpawnPositions SpawnPositions => spawnPositions;

	#endregion

	#region Fields

	[Required]
	[SerializeField]
	private EnemySpawner spawner;
	
	[Required]
	[SerializeField]
	private EnemySpawnTimer spawnTimer;
	
	[Required]
	[SerializeField]
	private EnemyCounter enemyCounter;
	
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

	private void OnEnable()
	{
		SpawnTimer.OnTimerElapsed += OnTimerElapsed;
	}

	private void OnDisable()
	{
		SpawnTimer.OnTimerElapsed -= OnTimerElapsed;
	}

	#endregion

	private void OnTimerElapsed()
	{
		
	}
}