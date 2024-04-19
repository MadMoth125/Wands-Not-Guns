using System;
using UnityEngine;

/// <summary>
/// Value container that can be observed for changes.
/// Invokes any listeners when the value is updated.
/// </summary>
/// <typeparam name="T">The type of value to observe.</typeparam>
[Serializable]
public class Observable<T> where T : IEquatable<T>
{
	/// <summary>
	/// Allow implicit conversion from Observable&lt;T&gt; to T.
	/// This allows you to use the <see cref="Observable{T}"/> as if it were a T.
	/// e.g.,
	/// Observable myInt = new Observable(5);
	/// int value = myInt; (automatically calls myInt.Value)
	/// </summary>
	public static implicit operator T(Observable<T> value) => value.Value;
	
	/// <summary>
	/// Create a new observable with the given value and optional callback.
	/// </summary>
	/// <param name="value">The initial value of the observable.</param>
	/// <param name="callback">The callback to invoke when the value is updated. (can be added later)</param>
	public Observable(T value, Action<T> callback = null)
	{
		this.value = value;
		if (callback != null) OnValueChanged += callback;
	}
	
	/// <summary>
	/// Internal event that is invoked when the value is updated.
	/// </summary>
	private event Action<T> OnValueChanged;

	/// <summary>
	/// The current value of the observable.
	/// </summary>
	public T Value
	{
		get => value;
		set => Set(value);
	}
	
	/// <summary>
	/// The current value of the observable.
	/// </summary>
	[SerializeField] 
	private T value;

	#region Public Methods

	/// <summary>
	/// Set the value of the observable.
	/// If the value is different, invoke the listeners.
	/// </summary>
	/// <param name="value">The new value of the observable.</param>
	public virtual void Set(T value)
	{
		if (Equals(this.value, value)) return;
		this.value = value;
		Invoke();
	}
	
	/// <summary>
	/// Invoke the listeners with the current value.
	/// </summary>
	public virtual void Invoke()
	{
		OnValueChanged?.Invoke(value);
	}
	
	/// <summary>
	/// Add a listener to the observable.
	/// </summary>
	/// <param name="callback">The callback to invoke when the value is updated.</param>
	public virtual void AddListener(Action<T> callback)
	{
		OnValueChanged += callback;
	}
	
	/// <summary>
	/// Remove a listener from the observable.
	/// </summary>
	/// <param name="callback">The callback to remove from the listeners.</param>
	public virtual void RemoveListener(Action<T> callback)
	{
		OnValueChanged -= callback;
	}
	
	/// <summary>
	/// Clear all listeners from the observable.
	/// </summary>
	public virtual void ClearListeners()
	{
		if (OnValueChanged == null) return;
		var delegates = OnValueChanged.GetInvocationList();
		foreach (var del in delegates)
		{
			if (del == null) continue;
			OnValueChanged -= (Action<T>)del;
		}
	}
	
	/// <summary>
	/// Clears all listeners,
	/// sets the value to the 'default',
	/// and nulls the event.
	/// </summary>
	public virtual void Dispose()
	{
		ClearListeners();
		OnValueChanged = null;
		value = default;
	}

	#endregion
}