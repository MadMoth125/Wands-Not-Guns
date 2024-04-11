using UnityEngine;

namespace ScriptExtensions
{
	/// <summary>
	/// Most extensions here come from:
	/// https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/Extensions/GameObjectExtensions.cs
	/// </summary>
	public static class GameObjectExtensions
	{
		/// <summary>
		/// Returns the object itself if it exists, null otherwise.
		/// </summary>
		/// <remarks>
		/// This method helps differentiate between a null reference and a destroyed Unity object. Unity's "== null" check
		/// can incorrectly return true for destroyed objects, leading to misleading behaviour. The OrNull method use
		/// Unity's "null check", and if the object has been marked for destruction, it ensures an actual null reference is returned,
		/// aiding in correctly chaining operations and preventing NullReferenceExceptions.
		/// </remarks>
		/// <typeparam name="T">The type of the object.</typeparam>
		/// <param name="obj">The object being checked.</param>
		/// <returns>The object itself if it exists and not destroyed, null otherwise.</returns>
		public static T OrNull<T>(this T obj) where T : UnityEngine.Object
		{
			return obj ? obj : null;
		}

		/// <summary>
		/// Gets a component of the given type attached to the GameObject. If that type of component does not exist, it adds one.
		/// </summary>
		/// <remarks>
		/// This method is useful when you don't know if a GameObject has a specific type of component,
		/// but you want to work with that component regardless. Instead of checking and adding the component manually,
		/// you can use this method to do both operations in one line.
		/// </remarks>
		/// <typeparam name="T">The type of the component to get or add.</typeparam>
		/// <param name="target">The GameObject to get the component from or add the component to.</param>
		/// <returns>The existing component of the given type, or a new one if no such component exists.</returns> 
		public static T GetOrAddComponent<T>(this GameObject target) where T : Component
		{
			var component = target.GetComponent<T>();
			if (!component) component = target.AddComponent<T>();
			return component;
		}

		/// <summary>
		/// Gets a component of the given type attached to the component's GameObject. If that type of component does not exist, it adds one.
		/// </summary>
		/// <remarks>
		/// This method is useful when you don't know if a component's GameObject has a specific type of component,
		/// but you want to work with that component regardless. Instead of checking and adding the component manually,
		/// you can use this method to do both operations in one line.
		/// </remarks>
		/// <typeparam name="T">The type of the component to get or add.</typeparam>
		/// <param name="target">The component's GameObject to get the component from or add the component to.</param>
		/// <returns>The existing component of the given type, or a new one if no such component exists.</returns> 
		public static T GetOrAddComponent<T>(this Component target) where T : Component
		{
			var component = target.gameObject.GetComponent<T>();
			if (!component) component = target.gameObject.AddComponent<T>();
			return component;
		}
	}
}