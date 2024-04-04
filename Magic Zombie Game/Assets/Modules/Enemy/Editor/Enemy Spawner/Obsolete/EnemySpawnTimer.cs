using System;
using Enemy.Spawner;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;

[Obsolete]
public class EnemySpawnTimer : MonoBehaviour
{
	/// <summary>
	/// Invoked when the timer ticks.
	/// <see cref="Time.time"/> is passed as an argument.
	/// </summary>
	public event Action<float> OnTimerTick;
	
	[Required]
	[SerializeField]
	private EnemySpawnManager spawnManager;
	
	[SerializeField]
	private SpawnIntervalAsset intervalAsset;
	
	private float _timer;

	public void ResetTimer()
	{
		_timer = 0f;
	}
	
	#region Unity Methods

	private void Update()
	{
		if (_timer < GetValidInterval())
		{
			_timer += Time.deltaTime;
		}
		else
		{
			_timer = Mathf.Max(0f, _timer - GetValidInterval());
			
			// if the timer is close enough to 0, reset it for simplicity
			if (_timer.ApproxEquals(0f, 0.001f))
			{
				ResetTimer();
			}
			
			OnTimerTick?.Invoke(Time.time);
		}
	}

	#endregion
	
	private float GetValidInterval()
	{
		return intervalAsset != null ? intervalAsset.GetInterval() : float.MaxValue;
	}
}