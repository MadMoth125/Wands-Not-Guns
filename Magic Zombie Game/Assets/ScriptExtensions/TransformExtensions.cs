using System;
using System.Collections;
using System.Collections.Generic;
using ScriptExtensions.Shared;
using UnityEngine;

namespace ScriptExtensions
{
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