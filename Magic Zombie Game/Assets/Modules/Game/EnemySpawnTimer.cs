using System;
using Core.CustomDebugger;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySpawnTimer : MonoBehaviour, IManagerComponent<EnemyManager>
{
	public event Action OnTimerElapsed;
	public event Action OnTimerReset;
	public event Action OnTimerPaused;
	public event Action OnTimerPlayed;
	
	public float Interval => spawnIntervalAsset != null ? spawnIntervalAsset.GetInterval() : float.MaxValue;
	public float TotalElapsedTime => _totalTime;
	public float ElapsedTime => _timer;
	public SpawnIntervalAsset SpawnIntervalAsset => spawnIntervalAsset;

	[Required]
	[SerializeField]
	private SpawnIntervalAsset spawnIntervalAsset;
	
	[Tooltip("Whether the timer is active. If false, the timer will not tick.")]
	[SerializeField]
	private bool active = true;
	
	[Tooltip("Whether the timer should tick manually. If false, the timer will tick automatically.")]
	public bool manualTick = false;
	
	[TitleGroup("Debug", "Parameters", Alignment = TitleAlignments.Centered)]
	[SerializeField]
	private LoggerAsset logger;

	[TitleGroup("Debug")]
	[Tooltip("Enable logging of when the timer elapses.")]
	[SerializeField]
	private bool logTimerElapsed = false;
	
	[TitleGroup("Debug")]
	[Tooltip("Enable logging of when the timer's state is update via reset, pause, or play.")]
	[SerializeField]
	private bool logTimerStateUpdates = false;
	
	private float _timer;
	private float _totalTime;
	private EnemyManager _manager;
	
	public void SetParentManager(EnemyManager manager)
	{
		_manager = manager;
	}

	public void Tick(float deltaTime)
	{
		if (_timer < Interval)
		{
			_timer += deltaTime;
		}
		else
		{
			_timer = Mathf.Max(0f, _timer - Interval);

			OnTimerElapsed?.Invoke();
			
			if (logTimerElapsed) LogWrapper("Timer elapsed.", LoggerAsset.LogType.Info);
		}
		
		_totalTime += deltaTime;
	}

	public void ResetTimer()
	{
		_timer = 0f;
		_totalTime = 0f;
		OnTimerReset?.Invoke();
		
		if (logTimerStateUpdates) LogWrapper("Timer reset.", LoggerAsset.LogType.Info);
	}

	public void PauseTimer()
	{
		if (!active) return;
		active = false;
		OnTimerPaused?.Invoke();
		
		if (logTimerStateUpdates) LogWrapper("Timer paused.", LoggerAsset.LogType.Info);
	}

	public void ResumeTimer()
	{
		if (active) return;
		active = true;
		OnTimerPlayed?.Invoke();
		
		if (logTimerStateUpdates) LogWrapper("Timer resumed.", LoggerAsset.LogType.Info);
	}

	#region Unity Methods

	private void Update()
	{
		if (!active)
		{
			return;
		}
		
		if (manualTick)
		{
			return;
		}
		
		Tick(Time.deltaTime);
	}

	#endregion
	
	private void LogWrapper(string message, LoggerAsset.LogType type)
	{
		if (logger != null)
		{
			logger.Log(message, this, type);
		}
	}
}