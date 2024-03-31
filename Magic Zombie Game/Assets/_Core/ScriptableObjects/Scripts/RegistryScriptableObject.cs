using System;
using System.Collections.Generic;
using System.Linq;
using Core.CustomDebugger;
using UnityEngine;

namespace Core.Registries
{
	public abstract class RegistryScriptableObject<TKey, TValue> : ScriptableObject where TKey : IEquatable<TKey>
	{
		public event Action<TKey, TValue> OnItemAdded;
		
		public event Action<TKey, TValue> OnItemRemoved;
		
		public event Action<IEnumerable<TKey>, IEnumerable<TValue>> OnRegistryCleared;
		
		public int Count => registry.Count;

		[Tooltip("Logger to use for this registry. If null, no logging will occur.")]
		[SerializeField]
		protected LoggerAsset logger;
		
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
		/// Returns all keys in the registry.
		/// </summary>
		public virtual IEnumerable<TKey> GetKeys()
		{
			LogWrapper($"Outputting {registry?.Keys.Count} keys in the registry.", LoggerAsset.LogType.Info);
			return registry?.Keys;
		}

		/// <summary>
		/// Returns all values in the registry.
		/// </summary>
		public virtual IEnumerable<TValue> GetValues()
		{
			LogWrapper($"Outputting {registry?.Values.Count} values in the registry.", LoggerAsset.LogType.Info);
			return registry?.Values;
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
			var randomValue = GetValues().ElementAt(randomIndex);
			LogWrapper($"Random value \"{randomValue}\" has been retrieved from the registry.", LoggerAsset.LogType.Info);
			return randomValue;
		}
		
		/// <summary>
		/// Clears the registry.
		/// </summary>
		public virtual void Clear()
		{
			InvokeRegistryCleared(registry.Keys, registry.Values);
			LogWrapper("Registry has been cleared.", LoggerAsset.LogType.Info);
			registry?.Clear();
		}

		#region Event Invokers

		protected void InvokeItemAdded(TKey key, TValue value)
		{
			OnItemAdded?.Invoke(key, value);
		}
		
		protected void InvokeItemRemoved(TKey key, TValue value)
		{
			OnItemRemoved?.Invoke(key, value);
		}
		
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