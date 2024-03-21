using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.TickSystem
{
	[System.Serializable]
	public class TickGroup
	{
		public TickGroup(string name = "Tick Element", int tickRate = 20, bool enabled = true)
		{
			this.enabled = enabled;
			this.name = name;
			this.tickRate = tickRate;
			_listeners = new List<Action>();
		}
	
		public event Action OnTick;

		#region Properties

		public bool Enabled => enabled;
	
		public string Name => name;
	
		public int TickRate => tickRate;
	
		public float TickInterval => GetTickInterval();

		public IEnumerable<Action> Listeners => _listeners;
		
		private bool NoListeners => _listeners == null || _listeners.Count == 0;

		#endregion

		public static readonly int tickRateMin = 1;
		public static readonly int tickRateMax = 120;

		#region Serialized Fields

		[VerticalGroup("Layer")]
		[EnableIf("@enabled")]
		[LabelWidth(50)]
		[SerializeField]
		private string name;

		[VerticalGroup("Layer")]
		[HorizontalGroup("Layer/Parameters", 100)]
		[LabelWidth(50)]
		[SerializeField]
		private bool enabled;

		[VerticalGroup("Layer")]
		[HorizontalGroup("Layer/Parameters")]
		[DisableInPlayMode]
		[EnableIf("@enabled")]
		[PropertyRange("@TickGroup.tickRateMin", "@TickGroup.tickRateMax")]
		[LabelWidth(60)]
		[SuffixLabel("tick/s", Icon = SdfIconType.Clock)]
		[SerializeField]
		private int tickRate;

		#endregion

		private List<Action> _listeners;
		private float _tickInterval = 0;
		private float _timeSinceLastTick = 0;
		private bool _initialized = false;

		#region Tick Methods

		/// <summary>
		/// Invokes the OnTick event for this layer.
		/// Does nothing if the layer is disabled.
		/// </summary>
		public void Tick()
		{
			if (!Enabled) return;
			if (NoListeners) return;
		
			OnTick?.Invoke();
		}
	
		/// <summary>
		/// Returns the tick interval in seconds.
		/// Value is calculated once, then cached.
		/// </summary>
		public float GetTickInterval()
		{
			if (!_initialized) _tickInterval = 1f / tickRate;
			return _tickInterval;
		}
	
		/// <summary>
		/// Handles the tick interval and returns true if the layer should tick.
		/// Once the tick interval is reached, the time since the last tick is reset.
		/// </summary>
		/// <returns></returns>
		public bool ShouldTick()
		{
			if (!Enabled) return false;
			if (NoListeners) return false;
		
			if (_timeSinceLastTick >= GetTickInterval())
			{
				_timeSinceLastTick = 0;
				return true;
			}
		
			_timeSinceLastTick += Time.deltaTime;
			return false;
		}
	
		/// <summary>
		/// Sets the enabled state of this layer.
		/// </summary>
		public void SetEnabled(bool enable)
		{
			enabled = enable;
		}

		#endregion

		#region Listener Methods

		/// <summary>
		/// Adds a listener to this tick layer.
		/// If the listener is already added, does nothing.
		/// </summary>
		/// <param name="listener">The listener to add.</param>
		public void AddListener(Action listener)
		{
			if (NoListeners || _listeners.Contains(listener)) return;
			_listeners.Add(listener);
			OnTick += listener;
		}
	
		/// <summary>
		/// Removes a listener from this tick layer.
		/// If the listener is not found, does nothing.
		/// </summary>
		/// <param name="listener">The listener to remove.</param>
		public void RemoveListener(Action listener)
		{
			if (NoListeners || !_listeners.Contains(listener)) return;
			_listeners.Remove(listener);
			OnTick -= listener;
		}
	
		/// <summary>
		/// Removes all listeners from this tick layer.
		/// </summary>
		public void ClearListeners()
		{
			if (NoListeners) return;
			
			foreach (var listener in _listeners)
			{
				OnTick -= listener;
			}
			
			_listeners.Clear();
		}

		#endregion
	}
}