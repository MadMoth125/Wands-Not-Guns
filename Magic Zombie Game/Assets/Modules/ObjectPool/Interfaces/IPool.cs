using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
	public interface IPool<T>
	{
		/// <summary>
		/// Get an element from the pool.
		/// Allows you to optionally set the element's new position, rotation, and scale.
		/// </summary>
		/// <param name="position">The element's new position.</param>
		/// <param name="rotation">The element's new rotation.</param>
		/// <param name="scale">The element's new scale.</param>
		/// <returns>The element retrieved from the pool.</returns>
		public T GetElement(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null);
	
		/// <summary>
		/// Return a specified object to the pool.
		/// </summary>
		/// <param name="element">The element to return to the pool.</param>
		public void ReleaseElement(T element);

		/// <summary>
		/// Returns a collection of all active objects in the pool.
		/// </summary>
		public IEnumerable<T> GetAllActiveObjects();

		/// <summary>
		/// Returns a collection of all inactive objects in the pool.
		/// </summary>
		public IEnumerable<T> GetAllInactiveObjects();

		/// <summary>
		/// Returns the number of active objects in the pool.
		/// </summary>
		public int GetActiveObjectCount();
		
		/// <summary>
		/// Returns the number of inactive objects in the pool.
		/// </summary>
		public int GetInactiveObjectCount();
		
		/// <summary>
		/// Returns all objects back to the pool.
		/// </summary>
		/// <param name="resetState">Whether or not to reset the state of the elements when returned to the pool.</param>
		public void ReturnAllObjects(bool resetState);
	}
}