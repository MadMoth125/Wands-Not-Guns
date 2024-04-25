using System;
using System.Collections;
using System.Collections.Generic;
using Mr_sB.UnityTimer;
using UnityEngine;

public class SemiAutoShootComponent : IDisposable
{
	public event Action OnShouldShoot;
	
	public bool IsFiring { get; private set; }
	public DelayTimer FireIntervalTimer => _fireDelayTimer;
	public float Interval => _interval;

	#region Constructors

	public SemiAutoShootComponent(float interval)
	{
		SetFireInterval(interval);
	}
	
	public SemiAutoShootComponent(float interval, Action onFire)
	{
		_fireAction = onFire;
		SetFireInterval(interval);
	}

	#endregion
	
	private bool _canFire = true;
	private float _interval;
	private Action _fireAction;
	private DelayTimer _fireDelayTimer;
	
	/// <summary>
	/// Set the action to be called when the weapon fires.
	/// This is separate from the <see cref="OnShouldShoot"/>
	/// event that is also called when the weapon fires.
	/// </summary>
	/// <param name="action">The action to be called when the weapon fires.</param>
	public void SetFireAction(Action action)
	{
		_fireAction = action;
	}
	
	/// <summary>
	/// Clear the action to be called when the weapon fires.
	/// </summary>
	public void ClearFireAction()
	{
		_fireAction = null;
	}
	
	/// <summary>
	/// Set the time between each shot.
	/// Interval is clamped to a minimum of 0.05 seconds.
	/// </summary>
	/// <param name="interval">The time between each shot.</param>
	public void SetFireInterval(float interval)
	{
		_interval = Mathf.Max(0.05f, interval);
	}
	
	public void ForceStopTimer()
	{
		_fireDelayTimer?.Cancel();
		_canFire = true;
	}
	
	public void BeginFire()
	{
		if (!_canFire) return;
		
		IsFiring = true;
		_canFire = false;
		HandleFire();
		
		// (the delay timer acts as a cooldown
		// before being able to shoot again.)
		if (_fireDelayTimer == null)
		{
			_fireDelayTimer = Timer.DelayAction(_interval, () =>
			{
				IsFiring = false;
				_canFire = true;
			});
		}
		else if (_fireDelayTimer.isDone)
		{
			_fireDelayTimer.Restart();
		}
	}
	
	private void HandleFire()
	{
		_fireAction?.Invoke();
		OnShouldShoot?.Invoke();
	}

	public void Dispose()
	{
		_fireDelayTimer?.Cancel();
		_fireDelayTimer = null;
		_fireAction = null;
	}
}