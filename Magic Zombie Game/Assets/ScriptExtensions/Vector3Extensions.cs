using System.Collections.Generic;
using System.Linq;
using ScriptExtensions.Shared;
using UnityEngine;

namespace ScriptExtensions
{
	public static class Vector3Extensions
	{
		#region Vector3 Extensions

		public static Vector3 SetX(this Vector3 vector, in float x)
		{
			vector.x = x;
			return vector;
		}

		public static Vector3 SetY(this Vector3 vector, in float y)
		{
			vector.y = y;
			return vector;
		}

		public static Vector3 SetZ(this Vector3 vector, in float z)
		{
			vector.z = z;
			return vector;
		}

		public static Vector3 Set(this Vector3 vector, float? x = null, float? y = null, float? z = null)
		{
			vector.x = x ?? vector.x;
			vector.y = y ?? vector.y;
			vector.z = z ?? vector.z;
			return vector;
		}

		public static Vector3 Multiply(this Vector3 a, in Vector3 b)
		{
			a.x *= b.x;
			a.y *= b.y;
			a.z *= b.z;
			return a;
		}

		public static Vector3 Center(this IEnumerable<Vector3> vectors)
		{
			Vector3 center = Vector3.zero;
			
			var vectorArray = vectors as Vector3[] ?? vectors.ToArray();
			foreach (var vector in vectorArray)
			{
				center += vector;
			}

			return center / vectorArray.Length;
		}

		/// <summary>
		/// Return the value multiplied by <see cref="Time.deltaTime"/>
		/// </summary>
		public static Vector3 Delta(this Vector3 value) => value * Time.deltaTime;

		/// <summary>
		/// Find the closest position from a list of positions.
		/// </summary>
		/// <param name="position">The origin position to search from.</param>
		/// <param name="positions">The list of positions to search through.</param>
		/// <returns>The closest position from the list, and the distance squared from the origin position.</returns>
		public static (Vector3 position, float distance) FindClosestPosition(this Vector3 position, IEnumerable<Vector3> positions)
			=> FindClosestOrFurthestPosition(position, positions, PositionSortType.Closest);

		/// <summary>
		/// Find the furthest position from a list of positions.
		/// </summary>
		/// <param name="position">The origin position to search from.</param>
		/// <param name="positions">The list of positions to search through.</param>
		/// <returns>The furthest position from the list, and the distance squared from the origin position.</returns>
		public static (Vector3, float) FindFurthestPosition(this Vector3 position, IEnumerable<Vector3> positions) 
			=> FindClosestOrFurthestPosition(position, positions, PositionSortType.Furthest);

		#endregion

		/// <summary>
		/// Internal method to find the closest or furthest position from a list of positions.
		/// Called by <see cref="FindClosestPosition"/> and <see cref="FindFurthestPosition"/> to expose selective search methods.
		/// </summary>
		/// <returns>A tuple containing the closest or furthest position and the distance squared from the origin position.</returns>
		private static (Vector3, float) FindClosestOrFurthestPosition(in Vector3 position, in IEnumerable<Vector3> positions, PositionSortType searchType)
		{
			Vector3 closestOrFurthest = Vector3.zero;
			float closestOrFurthestDistanceSqr = Mathf.Infinity;

			foreach (Vector3 point in positions)
			{
				Vector3 directionToTarget = point - position;
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