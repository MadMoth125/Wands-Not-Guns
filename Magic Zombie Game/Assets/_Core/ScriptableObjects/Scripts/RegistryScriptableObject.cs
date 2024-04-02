using System;
using System.Collections.Generic;
using System.Linq;
using Core.CustomDebugger;
using UnityEngine;

namespace Core.Registries
{
	public abstract class RegistryScriptableObject<TKey, TValue> : ScriptableObject where TKey : IEquatable<TKey>
	{
		#region Events

		public event Action<TKey, TValue> OnItemAdded;
		
		public event Action<TKey, TValue> OnItemRemoved;
		
		public event Action<IEnumerable<TKey>, IEnumerable<TValue>> OnRegistryCleared;

		#endregion
		
		public int Count => registry?.Count ?? 0;

		[Tooltip("Logger to use for this registry. If null, no logging will occur.")]
		[SerializeField]
		protected LoggerAsset logger;
		
		protected TKey[] cachedKeys;
		protected TValue[] cachedValues;
		protected readonly Dictionary<TKey, TValue> registry = new();

		/// <summary>
		/// Adds a key-value pair to the registry.
		/// Returns true if successful, false if the key already exists.
		/// </summary>
		public virtual bool Register(TKey key, TValue value)
		{
			if (registry == null)
			{
				LogRegistryInvalid();
				return false;
			}

			if (key == null)
			{
				LogKeyInvalid();
				return false;
			}

			if (registry.TryAdd(key, value))
			{
				RefreshCache();
				InvokeItemAdded(key, value);
				LogWrapper($"Value \"{value}\" with key of \"{key}\" has been added to the registry.", LoggerAsset.LogType.Info);
				return true;
			}
			
			LogWrapper($"Key \"{key}\" already exists in the registry.", LoggerAsset.LogType.Warning);
			return false;
		}
		
		/// <summary>
		/// Removes a value from the registry if the provided key exists.
		/// </summary>
		public virtual bool Unregister(TKey key)
		{
			if (registry == null)
			{
				LogRegistryInvalid();
				return false;
			}

			if (key == null)
			{
				LogKeyInvalid();
				return false;
			}

			if (registry.Remove(key, out TValue value))
			{
				RefreshCache();
				InvokeItemRemoved(key, value);
				LogWrapper($"Value \"{value}\" with key \"{key}\" has been removed from the registry.", LoggerAsset.LogType.Info);
				return true;
			}
			
			LogWrapper($"Key \"{key}\" does not exist in the registry.", LoggerAsset.LogType.Warning);
			return false;
		}
		
		/// <summary>
		/// Returns a value from the registry if the provided key exists.
		/// </summary>
		public TValue GetValue(TKey key)
		{
			if (registry == null)
			{
				LogRegistryInvalid();
				return default;
			}

			if (key == null)
			{
				LogKeyInvalid();
				return default;
			}

			if (registry.TryGetValue(key, out TValue value))
			{
				LogWrapper($"Value \"{value}\" with key \"{key}\" has been retrieved from the registry.", LoggerAsset.LogType.Info);
				return value;
			}
			
			LogWrapper($"Key \"{key}\" does not exist in the registry.", LoggerAsset.LogType.Warning);
			return default;
		}
		
		/// <summary>
		/// Sets a value in the registry if the provided key exists.
		/// </summary>
		public void SetValue(TKey key, TValue value)
		{
			if (registry == null)
			{
				LogRegistryInvalid();
				return;
			}

			if (key == null)
			{
				LogKeyInvalid();
				return;
			}

			if (registry.ContainsKey(key))
			{
				registry[key] = value;
				RefreshCache();
				LogWrapper($"Value \"{value}\" with key \"{key}\" has been set in the registry.", LoggerAsset.LogType.Info);
			}
			else
			{
				LogWrapper($"Key \"{key}\" does not exist in the registry.", LoggerAsset.LogType.Warning);
			}
		}
		
		/// <summary>
		/// Returns all keys in the registry in an <see cref="IEnumerable{T}"/> format.
		/// Keys are pulled from the cached array rather than directly from the dictionary.
		/// </summary>
		public virtual IEnumerable<TKey> GetKeysEnumerable()
		{
			LogWrapper($"Got {cachedKeys.Length} registry keys.", LoggerAsset.LogType.Info);
			return cachedKeys;
		}
		
		/// <summary>
		/// Returns all keys in the registry in an array format.
		/// Keys are pulled from the cached array rather than directly from the dictionary.
		/// </summary>
		public virtual TKey[] GetKeysArray()
		{
			LogWrapper($"Got {cachedKeys.Length} registry keys.", LoggerAsset.LogType.Info);
			return cachedKeys;
		}

		/// <summary>
		/// Returns all values in the registry in an <see cref="IEnumerable{T}"/> format.
		/// Values are pulled from the cached array rather than directly from the dictionary.
		/// </summary>
		public virtual IEnumerable<TValue> GetValuesEnumerable()
		{
			LogWrapper($"Got {cachedValues.Length} registry values.", LoggerAsset.LogType.Info);
			return cachedValues;
		}
		
		/// <summary>
		/// Returns all values in the registry in an array format.
		/// Values are pulled from the cached array rather than directly from the dictionary.
		/// </summary>
		public virtual TValue[] GetValuesArray()
		{
			LogWrapper($"Got {cachedValues.Length} registry values.", LoggerAsset.LogType.Info);
			return cachedValues;
		}
		
		/// <summary>
		/// Gets a random key from the registry.
		/// </summary>
		/// <returns></returns>
		public virtual TKey GetRandomKey()
		{
			if (Count == 0)
			{
				LogWrapper("Registry is empty, unable to get random key.", LoggerAsset.LogType.Warning);
				return default;
			}

			int randomIndex = UnityEngine.Random.Range(0, Count);
			var randomKey = GetKeysArray()[randomIndex];
			LogWrapper($"Random key \"{randomKey}\" has been retrieved from the registry.", LoggerAsset.LogType.Info);
			return randomKey;
		}
		
		/// <summary>
		/// Returns a random value from the registry.
		/// </summary>
		public virtual TValue GetRandomValue()
		{
			if (Count == 0)
			{
				LogWrapper("Registry is empty, unable to get random value.", LoggerAsset.LogType.Warning);
				return default;
			}

			int randomIndex = UnityEngine.Random.Range(0, Count);
			var randomValue = GetValuesArray()[randomIndex];
			LogWrapper($"Random value \"{randomValue}\" has been retrieved from the registry.", LoggerAsset.LogType.Info);
			return randomValue;
		}
		
		/// <summary>
		/// Clears the registry.
		/// </summary>
		public virtual void Clear()
		{
			InvokeRegistryCleared(registry.Keys, registry.Values);
			
			registry?.Clear();
			RefreshCache();
			
			LogWrapper("Registry has been cleared.", LoggerAsset.LogType.Info);
		}

		/// <summary>
		/// Refreshes the cached keys and values arrays.
		/// Not necessary to call this method manually, as it is called automatically when adding or removing items.
		/// </summary>
		public void RefreshCache()
		{
			cachedKeys = registry.Keys.ToArray();
			cachedValues = registry.Values.ToArray();
		}
		
		#region Event Invokers

		/// <summary>
		/// Simple event wrapper for when an item is added to the registry.
		/// </summary>
		protected void InvokeItemAdded(TKey key, TValue value)
		{
			OnItemAdded?.Invoke(key, value);
		}
		
		/// <summary>
		/// Simple event wrapper for when an item is removed from the registry.
		/// </summary>
		protected void InvokeItemRemoved(TKey key, TValue value)
		{
			OnItemRemoved?.Invoke(key, value);
		}
		
		/// <summary>
		/// Simple event wrapper for when the registry is cleared.
		/// </summary>
		protected void InvokeRegistryCleared(IEnumerable<TKey> keys, IEnumerable<TValue> values)
		{
			OnRegistryCleared?.Invoke(keys, values);
		}

		#endregion

		#region Logging

		protected void LogRegistryInvalid()
		{
			LogWrapper("Internal registry reference is null, unable to perform operation.", LoggerAsset.LogType.Error);
		}

		protected void LogKeyInvalid()
		{
			LogWrapper("Key is null, unable to perform operation.", LoggerAsset.LogType.Warning);
		}
		
		protected void LogWrapper(string message, LoggerAsset.LogType logType)
		{
			if (logger == null) return;
			logger.Log(message, this, logType);
		}

		#endregion
	}
}