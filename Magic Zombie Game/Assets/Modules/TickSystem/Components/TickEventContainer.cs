using System;
using System.Collections.Generic;

namespace CustomTickSystem.Components
{
	/// <summary>
	/// A tick component class that manages listeners for a tick event.
	/// Has the ability to add and remove listeners, and invoking the tick event.
	/// </summary>
	public class TickEventContainer
	{
		~TickEventContainer()
		{
			ClearListeners();
		}
		
		// consider making this private
		private event Action OnTick;
	
		private readonly List<Action> _listeners = new();
	
		/// <summary>
		/// Calls the <see cref="OnTick"/> event.
		/// </summary>
		public void Tick()
		{
			OnTick?.Invoke();
		}
	
		/// <summary>
		/// Adds a listener to the tick event.
		/// If the listener is already added, it will not be added again.
		/// </summary>
		/// <param name="listener">The listener to add.</param>
		public void AddListener(Action listener)
		{
			if (_listeners.Contains(listener)) return;
			_listeners.Add(listener);
			OnTick += listener;
		}
	
		/// <summary>
		/// Adds multiple listeners to the tick event.
		/// Calls <see cref="AddListener"/> for each listener in the collection.
		/// </summary>
		/// <param name="listeners">The listeners to add.</param>
		public void AddListeners(IEnumerable<Action> listeners)
		{
			foreach (var listener in listeners)
			{
				AddListener(listener);
			}
		}
	
		/// <summary>
		/// Removes a listener from the tick event.
		/// If the listener is not found, it will not be removed.
		/// </summary>
		/// <param name="listener">The listener to remove.</param>
		public void RemoveListener(Action listener)
		{
			if (!_listeners.Contains(listener)) return;
			_listeners.Remove(listener);
			OnTick -= listener;
		}
	
		/// <summary>
		/// Clears all listeners from the tick event.
		/// If there are no listeners, nothing will happen.
		/// </summary>
		public void ClearListeners()
		{
			if (!HasAnyListeners()) return;
			foreach (var listener in _listeners)
			{
				OnTick -= listener;
			}
			_listeners.Clear();
		}
	
		public IEnumerable<Action> GetAllListeners()
		{
			return _listeners;
		}
	
		/// <summary>
		/// Whether the tick event has any listeners.
		/// </summary>
		/// <returns></returns>
		public bool HasAnyListeners()
		{
			return _listeners != null && _listeners.Count > 0;
		}
	}
}