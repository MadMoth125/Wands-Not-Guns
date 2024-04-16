using System;
using Core.CustomDebugger;
using Enemy.Registry;
using Obvious.Soap;
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
	public int ConcurrentEnemyCount => spawnCount.GetMaxConcurrentEnemyCount() - _remainingConcurrentEnemySpots;
	public int TotalEnemyCount => spawnCount.GetMaxEnemyCount() - _remainingTotalEnemySpots;

	#endregion

	#region Fields
	
	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventInt enemySpawnEvent;

	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventInt enemyDieEvent;

	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventInt roundEndEvent;
	
	[TabGroup("Scriptable Objects", Icon = SdfIconType.Box)]
	[Required]
	[SerializeField]
	private SpawnCountScriptableObject spawnCount;
	
	[TabGroup("Debug Settings", Icon = SdfIconType.Bug)]
	[SerializeField]
	private LoggerScriptableObject logger;

	[TabGroup("Debug Settings")]
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

	public void ResetCounts(int round)
	{
		_remainingConcurrentEnemySpots = spawnCount.GetMaxConcurrentEnemyCount();
		_remainingTotalEnemySpots = spawnCount.GetMaxEnemyCount();
	}

	#region Unity Methods

	private void Start()
	{
		ResetCounts(-1); // doesn't matter what the round is, just reset the counts
	}

	private void OnEnable()
	{
		enemySpawnEvent.OnRaised += OnEnemyCountIncreased;
		enemyDieEvent.OnRaised += OnEnemyCountDecreased;
		roundEndEvent.OnRaised += ResetCounts;
	}

	private void OnDisable()
	{
		enemySpawnEvent.OnRaised -= OnEnemyCountIncreased;
		enemyDieEvent.OnRaised -= OnEnemyCountDecreased;
		roundEndEvent.OnRaised -= ResetCounts;
	}

	#endregion

	private void OnEnemyCountIncreased(int enemyId)
	{
		// DECREASE available spots when an enemy SPAWNS
		_remainingConcurrentEnemySpots--;
		_remainingConcurrentEnemySpots = Mathf.Clamp(_remainingConcurrentEnemySpots, 0, spawnCount.GetMaxConcurrentEnemyCount());
		
		if (ReachedMaxConcurrentEnemies())
		{
			OnMaxConcurrentCountReached?.Invoke();
			if (logMaxEnemyCountsReached) LogWrapper($"Maximum concurrent enemy count '{spawnCount.GetMaxConcurrentEnemyCount()}' reached.", LoggerType.Info);
		}
		
		_remainingTotalEnemySpots--;
		_remainingTotalEnemySpots = Mathf.Clamp(_remainingTotalEnemySpots, 0, spawnCount.GetMaxEnemyCount());
		
		if (ReachedMaxTotalEnemies())
		{
			OnMaxTotalCountReached?.Invoke();
			if (logMaxEnemyCountsReached) LogWrapper($"Maximum total enemy count '{spawnCount.GetMaxEnemyCount()}'reached.", LoggerType.Info);
		}
	}

	private void OnEnemyCountDecreased(int enemyId)
	{
		// INCREASE available spots when an enemy DIES
		_remainingConcurrentEnemySpots++;
		_remainingConcurrentEnemySpots = Mathf.Clamp(_remainingConcurrentEnemySpots, 0, spawnCount.GetMaxConcurrentEnemyCount());
	}
	
	private void LogWrapper(string message, LoggerType logType)
	{
		if (logger != null)
		{
			logger.Log(message, this, logType);
		}
	}
}