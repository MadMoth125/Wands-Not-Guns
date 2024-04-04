using System;
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
			
			// if the timer is close enough to 0, reset it for simplicity
			// if (_timer.ApproxEquals(0f, 0.001f))
			// {
			// 	ResetTimer();
			// }
			
			OnTimerElapsed?.Invoke();
		}
		
		_totalTime += deltaTime;
	}

	public void ResetTimer()
	{
		_timer = 0f;
		_totalTime = 0f;
		OnTimerReset?.Invoke();
	}

	public void PauseTimer()
	{
		if (!active) return;
		active = false;
		OnTimerPaused?.Invoke();
	}

	public void ResumeTimer()
	{
		if (active) return;
		active = true;
		OnTimerPlayed?.Invoke();
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
}