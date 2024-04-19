using UnityEngine;

namespace Core.Owning
{
	/// <summary>
	/// Simple interface for objects that can be owned by another object.
	/// Only supports one owner at a time.
	/// </summary>
	/// <typeparam name="T">The type of object that can own this object.</typeparam>
	public interface IOwnable<T> where T : Object
	{
		/// <summary>
		/// Gets the current owner of the object.
		/// </summary>
		/// <returns>The assigned owner of the specified type.</returns>
		public T GetOwner();
	
		/// <summary>
		/// Sets the owner of the object.
		/// </summary>
		/// <param name="owner">The owner to assign.</param>
		public void SetOwner(T owner);
	}
}