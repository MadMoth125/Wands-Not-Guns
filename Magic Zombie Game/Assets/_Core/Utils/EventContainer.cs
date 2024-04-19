using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Utils
{
	#region Parameter-less EventContainer

	public class EventContainer
	{
		#region Properties

		public IEnumerable<Action> Listeners => _listeners;
		
		public bool HasListeners => _listeners.Count > 0;
		
		public int ListenerCount => _listeners?.Count ?? 0;

		#endregion
		
		private event Action Event;

		private readonly List<Action> _listeners = new();

		public void Invoke()
		{
			if (Event == null || Event.GetInvocationList().Length == 0) return;
			Event?.Invoke();
		}

		public void AddListener(Action action)
		{
			if (_listeners.Contains(action)) return;
			_listeners.Add(action);
			Event += action;
		}

		public void RemoveListener(Action action)
		{
			if (!_listeners.Contains(action)) return;
			_listeners.Remove(action);
			Event -= action;
		}
		
		public void Clear()
		{
			foreach (var listener in _listeners)
			{
				Event -= listener;
			}
			_listeners.Clear();
		}
	}

	#endregion

	#region Single Parameter EventContainer

	public class EventContainer<T1>
	{
		#region Properties

		public IEnumerable<Action<T1>> Listeners => _listeners;
		
		public bool HasListeners => _listeners.Count > 0;
		
		public int ListenerCount => _listeners?.Count ?? 0;

		#endregion
		
		private event Action<T1> Event;

		private readonly List<Action<T1>> _listeners = new();

		public void Invoke(T1 value)
		{
			if (Event == null || Event.GetInvocationList().Length == 0) return;
			Event?.Invoke(value);
		}

		public void AddListener(Action<T1> action)
		{
			if (_listeners.Contains(action)) return;
			_listeners.Add(action);
			Event += action;
		}

		public void RemoveListener(Action<T1> action)
		{
			if (!_listeners.Contains(action)) return;
			_listeners.Remove(action);
			Event -= action;
		}
		
		public void Clear()
		{
			foreach (var listener in _listeners)
			{
				Event -= listener;
			}
			_listeners.Clear();
		}
	}

	#endregion
}