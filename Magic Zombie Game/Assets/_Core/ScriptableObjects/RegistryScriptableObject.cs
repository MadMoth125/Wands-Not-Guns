using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Core.CustomDebugger.Logger;

namespace Core.Registries
{
	public abstract class RegistryScriptableObject<TKey, TValue> : ScriptableObject where TKey : IEquatable<TKey>
	{
		public int Count => registry.Count;

		[Tooltip("Logger to use for this registry. If null, no logging will occur.")]
		[SerializeField]
		private Logger logger;
		
		protected readonly Dictionary<TKey, TValue> registry = new();

		/// <summary>
		/// Adds a key-value pair to the registry.
		/// Returns true if successful, false if the key already exists.
		/// </summary>
		public virtual bool Register(TKey key, TValue value)
		{
			if (registry == null) return false;
			if (key == null) return false;
			if (value == null) return false;

			if (registry.TryAdd(key, value))
			{
				LogWrapper($"Value \"{value}\" with key of \"{key}\" has been added to the registry.", Logger.LogType.Info);
				return true;
			}
			
			LogWrapper($"Key \"{key}\" already exists in the registry.", Logger.LogType.Warning);
			return false;
		}
		
		/// <summary>
		/// Removes a value from the registry if the provided key exists.
		/// </summary>
		public virtual bool Unregister(TKey key)
		{
			if (registry == null) return false;
			if (key == null) return false;

			if (registry.Remove(key, out TValue value))
			{
				LogWrapper($"Value \"{value}\" with key \"{key}\" has been removed from the registry.", Logger.LogType.Info);
				return true;
			}
			
			LogWrapper($"Key \"{key}\" does not exist in the registry.", Logger.LogType.Warning);
			return false;
		}
		
		/// <summary>
		/// Returns all keys in the registry.
		/// </summary>
		public virtual IEnumerable<TKey> GetKeys()
		{
			LogWrapper("Outputting all keys in the registry.", Logger.LogType.Info);
			return registry?.Keys;
		}

		/// <summary>
		/// Returns all values in the registry.
		/// </summary>
		public virtual IEnumerable<TValue> GetValues()
		{
			LogWrapper("Outputting all values in the registry.", Logger.LogType.Info);
			return registry?.Values;
		}

		/// <summary>
		/// Clears the registry.
		/// </summary>
		public virtual void Clear()
		{
			LogWrapper("Registry has been cleared.", Logger.LogType.Info);
			registry?.Clear();
		}
		
		private void LogWrapper(string message, Logger.LogType logType)
		{
			if (logger == null) return;
			logger.Log(message, this, logType);
		}
	}
}