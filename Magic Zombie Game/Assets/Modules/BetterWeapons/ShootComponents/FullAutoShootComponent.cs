using System;
using System.Collections;
using System.Collections.Generic;
using Mr_sB.UnityTimer;
using UnityEngine;

/// <summary>
/// Class that handles the firing of a weapon in full auto mode.
/// </summary>
public class FullAutoShootComponent : IDisposable
{
	public event Action OnShouldShoot;
	
	public LoopUntilTimer FireIntervalTimer => _fireIntervalTimer;
	public float Interval => _interval;

	#region Constructors

	public FullAutoShootComponent(float interval)
	{
		SetFireInterval(interval);
	}
	
	public FullAutoShootComponent(float interval, Action onFire)
	{
		_fireAction = onFire;
		SetFireInterval(interval);
	}

	#endregion
	
	private float _interval;
	private bool _inputState;
	private Action _fireAction;
	private LoopUntilTimer _fireIntervalTimer; 
	
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
		_fireIntervalTimer?.Cancel();
		// _fireIntervalTimer = null;
	}
	
	/// <summary>
	/// Starts firing the weapon.
	/// A timer is started that will call any action set by <see cref="SetFireAction"/>
	/// and any listeners to the <see cref="OnShouldShoot"/> event.
	/// </summary>
	public void BeginFire()
	{
		if (_inputState) return;
		
		_inputState = true;
		if (_fireIntervalTimer == null)
		{
			_fireIntervalTimer = Timer.LoopUntilAction(_interval,
				s => !_inputState,
				v => HandleFire(),
				executeOnStart: true);
		}
		else if (_fireIntervalTimer.isDone)
		{
			_fireIntervalTimer.Restart();
		}
	}

	/// <summary>
	/// Stops firing the weapon.
	/// Lets the weapon know that the fire input has been released
	/// and will stop firing as soon as the current loop is finished.
	/// </summary>
	public void EndFire()
	{
		_inputState = false;
	}
	
	private void HandleFire()
	{
		if (!_inputState) return;
		_fireAction?.Invoke();
		OnShouldShoot?.Invoke();
	}

	public void Dispose()
	{
		_fireIntervalTimer?.Cancel();
		_fireIntervalTimer = null;
		_fireAction = null;
	}
}