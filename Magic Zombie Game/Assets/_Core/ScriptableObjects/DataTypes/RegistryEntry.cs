namespace Core.Data
{
	[System.Serializable]
	public class RegistryEntry<TKey, TValue>
	{
		public RegistryEntry(TKey key, TValue value)
		{
			this.key = key;
			this.value = value;
		}
		
		public TKey key;
		
		public TValue value;
	}
}
