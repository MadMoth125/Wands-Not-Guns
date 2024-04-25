using UnityEngine;
using System.Linq;
using Obvious.Soap;

namespace Core.Data
{
	public abstract class ScriptableListRegistry<TKey, TValue> : ScriptableList<RegistryEntry<TKey, TValue>>
	{
		public bool Register(TKey key, TValue value)
		{
			if (this.Any(x => x.key.Equals(key))) return false;
			Add(new RegistryEntry<TKey, TValue>(key, value));
			return true;
		}
		
		public bool Unregister(TKey key)
		{
			var entry = this.FirstOrDefault(x => x.key.Equals(key));
			if (entry == null) return false;
			Remove(entry);
			return true;
		}
		
		public TValue GetValue(TKey key)
		{
			var entry = this.FirstOrDefault(x => x.key.Equals(key));
			if (entry == null) return default;
			return entry.value;
		}
		
		public TValue GetRandomValue()
		{
			return Count == 0 ? default : this[Random.Range(0, Count)].value;
		}
	}
}
