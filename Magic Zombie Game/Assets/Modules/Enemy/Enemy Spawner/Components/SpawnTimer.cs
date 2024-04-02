using System;
using ScriptExtensions;
using UnityEngine;

[Serializable]
public class SpawnTimer : EnemySpawnerComponentBase
{
	/// <summary>
	/// Invoked when the timer completes an interval.
	/// <see cref="Time.time"/> is passed as an argument.
	/// </summary>
	public event Action<float> OnTimerTick;
	
	public SpawnIntervalAsset IntervalAsset => intervalAsset;

	[SerializeField]
	private SpawnIntervalAsset intervalAsset;
	
	private float _timer;
	
	/// <summary>
	/// The interval at which the timer ticks.
	/// Returns <see cref="float.MaxValue"/> if the underlying interval asset is null/invalid.
	/// </summary>
	public float GetInterval()
	{
		return intervalAsset != null ? intervalAsset.GetInterval() : float.MaxValue;
	}
	
	/// <summary>
	/// Resets the timer to 0.
	/// </summary>
	public void ResetTimer()
	{
		_timer = 0f;
	}

	/// <summary>
	/// Ticks the timer by the given delta time.
	/// Once the timer reaches the interval, it resets and invokes <see cref="OnTimerTick"/>.
	/// </summary>
	/// <param name="deltaTime">The delta time to tick the timer by.</param>
	public void Tick(float deltaTime)
	{
		if (_timer < GetInterval())
		{
			_timer += deltaTime;
		}
		else
		{
			_timer = Mathf.Max(0f, _timer - GetInterval());
			
			// if the timer is close enough to 0, reset it for simplicity
			if (_timer.Equals(0f, 0.001f))
			{
				ResetTimer();
			}
			
			OnTimerTick?.Invoke(Time.time);
		}
	}
}