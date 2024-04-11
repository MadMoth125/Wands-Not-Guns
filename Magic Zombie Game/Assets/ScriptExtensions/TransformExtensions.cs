using System;
using System.Collections.Generic;
using ScriptExtensions.Shared;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptExtensions
{
	/// <summary>
	/// Most extensions here come from:
	/// https://github.com/adammyhre/Unity-Utils/blob/master/UnityUtils/Scripts/Extensions/GameObjectExtensions.cs
	/// </summary>
	public static class TransformExtensions
	{
		public static Vector3 GetCenterOfTransforms(IEnumerable<Transform> transforms)
		{
			Vector3 center = Vector3.zero;
			int count = 0;

			foreach (Transform transform in transforms)
			{
				center += transform.position;
				count++;
			}

			return center / count;
		}
		
		/// <summary>
		/// Retrieves all the children of a given Transform.
		/// </summary>
		/// <remarks>
		/// This method can be used with LINQ to perform operations on all child Transforms. For example,
		/// you could use it to find all children with a specific tag, to disable all children, etc.
		/// Transform implements IEnumerable and the GetEnumerator method which returns an IEnumerator of all its children.
		/// </remarks>
		/// <param name="parent">The Transform to retrieve children from.</param>
		/// <returns>An IEnumerable&lt;Transform&gt; containing all the child Transforms of the parent.</returns>    
		public static IEnumerable<Transform> Children(this Transform parent) 
		{
			foreach (Transform child in parent) 
			{
				yield return child;
			}
		}

		/// <summary>
		/// Resets transform's position, scale and rotation
		/// </summary>
		/// <param name="transform">Transform to use</param>
		public static void Reset(this Transform transform) 
		{
			transform.position = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		/// <summary>
		/// Destroys all child game objects of the given transform.
		/// </summary>
		/// <param name="parent">The Transform whose child game objects are to be destroyed.</param>
		public static void DestroyChildren(this Transform parent) 
		{
			parent.ForEveryChild(child => Object.Destroy(child.gameObject));
		}

		/// <summary>
		/// Immediately destroys all child game objects of the given transform.
		/// </summary>
		/// <param name="parent">The Transform whose child game objects are to be immediately destroyed.</param>
		public static void DestroyChildrenImmediate(this Transform parent) 
		{
			parent.ForEveryChild(child => Object.DestroyImmediate(child.gameObject));
		}

		/// <summary>
		/// Enables all child game objects of the given transform.
		/// </summary>
		/// <param name="parent">The Transform whose child game objects are to be enabled.</param>
		public static void EnableChildren(this Transform parent) 
		{
			parent.ForEveryChild(child => child.gameObject.SetActive(true));
		}

		/// <summary>
		/// Disables all child game objects of the given transform.
		/// </summary>
		/// <param name="parent">The Transform whose child game objects are to be disabled.</param>
		public static void DisableChildren(this Transform parent) 
		{
			parent.ForEveryChild(child => child.gameObject.SetActive(false));
		}

		/// <summary>
		/// Executes a specified action for each child of a given transform.
		/// </summary>
		/// <param name="parent">The parent transform.</param>
		/// <param name="action">The action to be performed on each child.</param>
		/// <remarks>
		/// This method iterates over all child transforms in reverse order and executes a given action on them.
		/// The action is a delegate that takes a Transform as parameter.
		/// </remarks>
		public static void ForEveryChild(this Transform parent, Action<Transform> action) 
		{
			for (int i = parent.childCount - 1; i >= 0; i--) {
					action(parent.GetChild(i));
			}
		}
		
		/// <summary>
		/// Find the closest transform from a list of transforms.
		/// </summary>
		/// <param name="transform">The origin transform to search from.</param>
		/// <param name="transforms">The list of transforms to search through.</param>
		/// <returns>The closest transform from the list, and the distance squared from the origin transform.</returns>
		public static (Transform, float) FindClosestTransform(this Transform transform, IEnumerable<Transform> transforms)
			=> FindClosestOrFurthestTransform(ref transform, transforms, PositionSortType.Closest);
		
		/// <summary>
		/// Find the furthest transform from a list of transforms.
		/// </summary>
		/// <param name="transform">The origin transform to search from.</param>
		/// <param name="transforms">The list of transforms to search through.</param>
		/// <returns>The furthest transform from the list, and the distance squared from the origin transform.</returns>
		public static (Transform, float) FindFurthestTransform(this Transform transform, IEnumerable<Transform> transforms) 
			=> FindClosestOrFurthestTransform(ref transform, transforms, PositionSortType.Furthest);
		
		/// <summary>
		/// Internal method to find the closest or furthest transform from a list of transforms.
		/// Called by <see cref="FindClosestTransform"/> and <see cref="FindFurthestTransform"/> to expose selective search methods.
		/// </summary>
		/// <returns>The closest or furthest transform from the list, and the distance squared from the origin transform.</returns>
		private static (Transform, float) FindClosestOrFurthestTransform(ref Transform position, IEnumerable<Transform> positions, PositionSortType searchType)
		{
			Transform closestOrFurthest = null;
			float closestOrFurthestDistanceSqr = Mathf.Infinity;
			
			foreach (Transform point in positions)
			{
				Vector3 directionToTarget = point.position - position.position;
				float distanceSqr = directionToTarget.sqrMagnitude;

				if (searchType == PositionSortType.Closest)
				{
					if (distanceSqr < closestOrFurthestDistanceSqr)
					{
						closestOrFurthestDistanceSqr = distanceSqr;
						closestOrFurthest = point;
					}
				}
				else if (searchType == PositionSortType.Furthest)
				{
					if (distanceSqr > closestOrFurthestDistanceSqr)
					{
						closestOrFurthestDistanceSqr = distanceSqr;
						closestOrFurthest = point;
					}
				}
			}

			return (closestOrFurthest, closestOrFurthestDistanceSqr);
		}
	}
}