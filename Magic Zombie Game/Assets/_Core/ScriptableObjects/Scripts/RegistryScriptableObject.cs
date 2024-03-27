using System;
using System.Collections.Generic;
using Core.CustomDebugger;
using UnityEngine;

namespace Core.Registries
{
	public abstract class RegistryScriptableObject<TKey, TValue> : ScriptableObject where TKey : IEquatable<TKey>
	{
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
				LogWrapper($"Value \"{value}\" with key \"{key}\" has been removed from the registry.", LoggerAsset.LogType.Info);
				return true;
			}
			
			LogWrapper($"Key \"{key}\" does not exist in the registry.", LoggerAsset.LogType.Warning);
			return false;
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
		/// Clears the registry.
		/// </summary>
		public virtual void Clear()
		{
			LogWrapper("Registry has been cleared.", LoggerAsset.LogType.Info);
			registry?.Clear();
		}

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
	}
}