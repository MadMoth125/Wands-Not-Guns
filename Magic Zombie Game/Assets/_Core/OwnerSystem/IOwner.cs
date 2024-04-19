using System.Collections.Generic;
using UnityEngine;

namespace Core.Owning
{
	public interface IOwner<out T> where T : Object
	{
		public T GetOwned();
	
		public IReadOnlyList<T> GetOwnables();
	}
}