using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyCounter : MonoBehaviour, IManagerComponent<EnemyManager>
{
	#region Events

	public event Action OnMaxConcurrentCountReached;
	public event Action OnMaxTotalCountReached;

	#endregion

	#region Properties

	public int RemainingConcurrentEnemySpots => _remainingConcurrentEnemySpots;
	public int RemainingTotalEnemySpots => _remainingTotalEnemySpots;
	public int ConcurrentEnemyCount => spawnCountAsset.GetMaxConcurrentEnemyCount() - _remainingConcurrentEnemySpots;
	public int TotalEnemyCount => spawnCountAsset.GetMaxEnemyCount() - _remainingTotalEnemySpots;

	#endregion

	#region Fields

	[Required]
	[SerializeField]
	private SpawnCountAsset spawnCountAsset;

	#endregion
	
	private int _remainingConcurrentEnemySpots;
	private int _remainingTotalEnemySpots;
	private EnemyManager _manager;
	
	public void SetParentManager(EnemyManager manager)
	{
		_manager = manager;
	}

	public bool ReachedMaxConcurrentEnemies()
	{
		return _remainingConcurrentEnemySpots <= 0;
	}

	public bool ReachedMaxTotalEnemies()
	{
		return _remainingTotalEnemySpots <= 0;
	}

	public void ResetCounts()
	{
		_remainingConcurrentEnemySpots = spawnCountAsset.GetMaxConcurrentEnemyCount();
		_remainingTotalEnemySpots = spawnCountAsset.GetMaxEnemyCount();
	}

	#region Unity Methods

	private void Awake()
	{
		ResetCounts();
	}

	private void OnEnable()
	{
		_manager.Spawner.OnEnemySpawn += OnEnemyCountIncreased;
		_manager.Spawner.OnEnemyDie += OnEnemyCountDecreased;
		_manager.ParentManager.RoundManager.OnRoundEnd += ResetCounts;
	}

	private void OnDisable()
	{
		_manager.Spawner.OnEnemySpawn -= OnEnemyCountIncreased;
		_manager.Spawner.OnEnemyDie -= OnEnemyCountDecreased;
		_manager.ParentManager.RoundManager.OnRoundEnd -= ResetCounts;
	}

	#endregion

	private void OnEnemyCountIncreased(EnemyComponent enemy)
	{
		// DECREASE available spots when an enemy SPAWNS
		_remainingConcurrentEnemySpots--;
		_remainingConcurrentEnemySpots = Mathf.Clamp(_remainingConcurrentEnemySpots, 0, spawnCountAsset.GetMaxConcurrentEnemyCount());
		
		if (ReachedMaxConcurrentEnemies())
		{
			OnMaxConcurrentCountReached?.Invoke();
		}
		
		_remainingTotalEnemySpots--;
		_remainingTotalEnemySpots = Mathf.Clamp(_remainingTotalEnemySpots, 0, spawnCountAsset.GetMaxEnemyCount());
		
		if (ReachedMaxTotalEnemies())
		{
			OnMaxTotalCountReached?.Invoke();
		}
		
		Debug.Log($"OnEnemyCountIncreased: Remaining spots: {_remainingConcurrentEnemySpots}");
	}

	private void OnEnemyCountDecreased(EnemyComponent enemy)
	{
		// INCREASE available spots when an enemy DIES
		_remainingConcurrentEnemySpots++;
		_remainingConcurrentEnemySpots = Mathf.Clamp(_remainingConcurrentEnemySpots, 0, spawnCountAsset.GetMaxConcurrentEnemyCount());
		
		Debug.Log($"OnEnemyCountDecreased: Remaining spots: {_remainingConcurrentEnemySpots}");
	}
}