namespace Core.ObjectPool
{
	public interface IPoolableObject
	{
		/// <summary>
		/// Called when the object is retrieved from the pool.
		/// </summary>
		public void OnGet();
	
		/// <summary>
		/// Called when the object is released back to the pool.
		/// </summary>
		public void OnRelease();
	}
}