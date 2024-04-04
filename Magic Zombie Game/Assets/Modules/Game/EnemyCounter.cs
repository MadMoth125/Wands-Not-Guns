using System;
using Core.CustomDebugger;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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
	
	[TitleGroup("Debug", "Parameters", Alignment = TitleAlignments.Centered)]
	[SerializeField]
	private LoggerAsset logger;

	[TitleGroup("Debug")]
	[Tooltip("Enable logging of when the maximum concurrent enemy count is reached, " +
	         "or the maximum total enemy count is reached.")]
	[SerializeField]
	private bool logMaxEnemyCountsReached = false;
	
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

	private void Start()
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
			if (logMaxEnemyCountsReached) LogWrapper($"Maximum concurrent enemy count '{spawnCountAsset.GetMaxConcurrentEnemyCount()}' reached.", LoggerAsset.LogType.Info);
		}
		
		_remainingTotalEnemySpots--;
		_remainingTotalEnemySpots = Mathf.Clamp(_remainingTotalEnemySpots, 0, spawnCountAsset.GetMaxEnemyCount());
		
		if (ReachedMaxTotalEnemies())
		{
			OnMaxTotalCountReached?.Invoke();
			if (logMaxEnemyCountsReached) LogWrapper($"Maximum total enemy count '{spawnCountAsset.GetMaxEnemyCount()}'reached.", LoggerAsset.LogType.Info);
		}
	}

	private void OnEnemyCountDecreased(EnemyComponent enemy)
	{
		// INCREASE available spots when an enemy DIES
		_remainingConcurrentEnemySpots++;
		_remainingConcurrentEnemySpots = Mathf.Clamp(_remainingConcurrentEnemySpots, 0, spawnCountAsset.GetMaxConcurrentEnemyCount());
	}
	
	private void LogWrapper(string message, LoggerAsset.LogType logType)
	{
		if (logger != null)
		{
			logger.Log(message, this, logType);
		}
	}
}