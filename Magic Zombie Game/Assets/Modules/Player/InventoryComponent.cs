using System;
using System.Collections;
using System.Collections.Generic;
using ScriptExtensions;
using UnityEngine;

public class InventoryComponent<T>
{
	public event Action<T> OnItemAdded;
	public event Action<T> OnItemRemoved;
	public event Action OnInventoryFull;
	public event Action OnCapacityChanged;
	
	public IReadOnlyList<T> Items => _items;
	public bool IsFull => _items.Count >= _capacity;
	
	public InventoryComponent(int capacity)
	{
		_capacity = capacity;
	}

	public InventoryComponent(IEnumerable<T> items)
	{
		_items.AddRange(items);
		_capacity = _items.Count;
	}

	private readonly List<T> _items = new List<T>();
	private int _capacity;

	public int GetItemCount()
	{
		return _items.Count;
	}
	
	public int GetCapacity()
	{
		return _capacity;
	}

	public void SetCapacity(int capacity)
	{
		if (_capacity == capacity) return;
		
		_capacity = Mathf.Max(0, capacity);
		OnCapacityChanged?.Invoke();
	}

	public T GetItem(int index)
	{
		return _items.InvalidIndex(index) ? default : _items[index];
	}
	
	public int GetIndexOfItem(T item)
	{
		return _items.IndexOf(item);
	}

	public void AddItem(T item)
	{
		if (_items.Count < _capacity)
		{
			_items.Add(item);
			OnItemAdded?.Invoke(item);
		}
		
		if (_items.Count == _capacity)
		{
			OnInventoryFull?.Invoke();
		}
	}

	public void AddItemAt(int index, T item)
	{
		if (_items.Count < _capacity)
		{
			_items.Insert(index, item);
			OnItemAdded?.Invoke(item);
		}
		
		if (_items.Count == _capacity)
		{
			OnInventoryFull?.Invoke();
		}
	}

	public void RemoveItem(T item)
	{
		if (_items.Contains(item))
		{
			_items.Remove(item);
			OnItemRemoved?.Invoke(item);
		}
	}

	public void RemoveItemAt(int index)
	{
		if (_items.ValidIndex(index))
		{
			var item = _items[index];
			_items.RemoveAt(index);
			OnItemRemoved?.Invoke(item);
		}
	}
	
	public bool ContainsItem(T item)
	{
		return _items.Contains(item);
	}
}