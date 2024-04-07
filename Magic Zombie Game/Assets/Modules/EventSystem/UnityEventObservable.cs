using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class UnityEventObservable<T> : Observable<T>
{
	public UnityEventObservable(T value, Action<T> callback = null) : base(value, null)
	{
		onValueChanged = new UnityEvent<T>();
		if (callback != null) onValueChanged.AddListener(callback.Invoke);
	}
	
	[SerializeField]
	private UnityEvent<T> onValueChanged;

	public override void Invoke()
	{
		onValueChanged.Invoke(Value);
	}

	public override void AddListener(Action<T> callback)
	{
		if (callback == null) return;
		onValueChanged ??= new UnityEvent<T>();
		onValueChanged.AddListener(callback.Invoke);
	}
	
	public override void RemoveListener(Action<T> callback)
	{
		if (callback == null) return;
		if (onValueChanged == null) return;
		onValueChanged.RemoveListener(callback.Invoke);
	}
	
	public override void ClearListeners()
	{
		if (onValueChanged == null) return;
		onValueChanged.RemoveAllListeners();
	}
	
	public override void Dispose()
	{
		ClearListeners();
		onValueChanged = null;
		Value = default;
	}
}