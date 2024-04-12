using System;
using System.Collections.Generic;
using Mr_sB.UnityTimer;
using UnityEngine;

namespace Weapons.Projectiles
{
	[DisallowMultipleComponent]
	public class ParticleTimerHandler : MonoBehaviour
	{
		private LoopTimer _releaseTimer;
		private DelayTimer _mainGameObjectTimer;
		private readonly List<DelayTimer> _delayTimers = new List<DelayTimer>();

		/// <summary>
		/// Starts a timer for tracking the main particle effect's lifetime.
		/// </summary>
		/// <param name="duration">The duration of the timer.</param>
		/// <param name="onComplete">The callback to call when the timer is complete.</param>
		public void StartMainParticleTimer(float duration, Action onComplete)
		{
			if (_mainGameObjectTimer != null)
			{
				_mainGameObjectTimer.Restart(duration, onComplete, null, false);
			}
			else
			{
				_mainGameObjectTimer = Timer.DelayAction(duration, onComplete, null, false);
			}
		}

		/// <summary>
		/// Stops the main particle effect's timer early.
		/// </summary>
		public void StopMainParticleTimer()
		{
			if (_mainGameObjectTimer != null)
			{
				_mainGameObjectTimer.Cancel();
			}
		}

		/// <summary>
		/// Will delay an action for a given amount of time.
		/// Calls onComplete when the delay has passed.
		/// </summary>
		/// <param name="delay">The amount of time to delay the action.</param>
		/// <param name="onComplete">The callback to call when the delay has passed.</param>
		public void DelayAction(float delay, Action onComplete)
		{
			// Find an open timer to use.
			var openTimer = _delayTimers.Find(t => t.isDone);
			
			if (openTimer != null)
			{
				// if an open timer is found, restart it.
				openTimer.Restart(delay, onComplete, null, false);
			}
			else
			{
				// if no open timer is found, create a new one.
				_delayTimers.Add(Timer.DelayAction(delay, onComplete, null, false));
			}
		}
	
		/// <summary>
		/// Cancels all timers, including the main particle timer.
		/// </summary>
		public void CancelAllTimers()
		{
			StopMainParticleTimer();
		
			foreach (var timer in _delayTimers)
			{
				if (timer != null && !timer.isDone)
				{
					timer.Cancel();
				}
			}
		}
	
		/// <summary>
		/// Returns true if any timers are running. Otherwise, returns false.
		/// </summary>
		public bool AnyTimersRunning()
		{
			bool mainTimerRunning = _mainGameObjectTimer != null && !_mainGameObjectTimer.isDone;
			bool delayTimersRunning = (_delayTimers != null && _delayTimers.Count != 0) && _delayTimers.Exists(t => !t.isDone);
			return mainTimerRunning || delayTimersRunning;
		}
	
		/// <summary>
		/// Checks at a given interval if all timers are complete. If they are, onComplete is called.
		/// Should only be called ONCE per instance of ParticleTimerHandler.
		/// </summary>
		/// <param name="interval">The interval at which to check for completion.</param>
		/// <param name="onComplete">The callback to call when all timers are complete.</param>
		public void CheckForAllTimersComplete(float interval, Action onComplete)
		{
			if (_releaseTimer == null)
			{
				_releaseTimer = Timer.LoopAction(interval, CheckComplete);
			}
			else
			{
				_releaseTimer.Restart(interval);
			}

			return;
		
			void CheckComplete(int var)
			{
				if (!AnyTimersRunning())
				{
					_releaseTimer?.Cancel();
					onComplete();
				}
			}
		}
	}
}