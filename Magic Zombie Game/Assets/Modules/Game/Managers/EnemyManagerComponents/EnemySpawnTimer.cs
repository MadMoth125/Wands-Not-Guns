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
	
	public float Interval => spawnInterval != null ? spawnInterval.GetInterval() : float.MaxValue;
	public float TotalElapsedTime => _totalTime;
	public float ElapsedTime => _timer;
	public SpawnIntervalScriptableObject SpawnInterval => spawnInterval;

	[Tooltip("Whether the timer is active. If false, the timer will not tick.")]
	[SerializeField]
	private bool active = true;

	[Tooltip("Whether the timer should tick manually. If false, the timer will tick automatically.")]
	public bool manualTick = false;

	[TitleGroup("Scriptable Objects", Alignment = TitleAlignments.Centered)]
	[Required]
	[SerializeField]
	private SpawnIntervalScriptableObject spawnInterval;

	[TitleGroup("Debug", "Parameters", Alignment = TitleAlignments.Centered)]
	[SerializeField]
	private LoggerScriptableObject logger;

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
			
			if (logTimerElapsed) LogWrapper("Timer elapsed.", LoggerType.Info);
		}
		
		_totalTime += deltaTime;
	}

	public void ResetTimer()
	{
		_timer = 0f;
		_totalTime = 0f;
		OnTimerReset?.Invoke();
		
		if (logTimerStateUpdates) LogWrapper("Timer reset.", LoggerType.Info);
	}

	public void PauseTimer()
	{
		if (!active) return;
		active = false;
		OnTimerPaused?.Invoke();
		
		if (logTimerStateUpdates) LogWrapper("Timer paused.", LoggerType.Info);
	}

	public void ResumeTimer()
	{
		if (active) return;
		active = true;
		OnTimerPlayed?.Invoke();
		
		if (logTimerStateUpdates) LogWrapper("Timer resumed.", LoggerType.Info);
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
	
	private void LogWrapper(string message, LoggerType type)
	{
		if (logger != null)
		{
			logger.Log(message, this, type);
		}
	}
}