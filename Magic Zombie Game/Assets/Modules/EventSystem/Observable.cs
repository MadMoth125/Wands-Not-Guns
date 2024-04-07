using System;
using UnityEngine;

[Serializable]
public class Observable<T>
{
	public static implicit operator T(Observable<T> value) => value.Value;
	
	public Observable(T value, Action<T> callback = null)
	{
		this.value = value;
		if (callback != null) OnValueChanged += callback;
	}
	
	private event Action<T> OnValueChanged;

	public T Value
	{
		get => value;
		set => Set(value);
	}
	
	[SerializeField] 
	private T value;

	public virtual void Set(T value)
	{
		if (Equals(this.value, value)) return;
		this.value = value;
		Invoke();
	}
	
	public virtual void Invoke()
	{
		OnValueChanged?.Invoke(value);
	}
	
	public virtual void AddListener(Action<T> callback)
	{
		OnValueChanged += callback;
	}
	
	public virtual void RemoveListener(Action<T> callback)
	{
		OnValueChanged -= callback;
	}
	
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
	
	public virtual void Dispose()
	{
		ClearListeners();
		OnValueChanged = null;
		value = default;
	}
}