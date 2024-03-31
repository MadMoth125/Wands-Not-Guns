using System.Collections.Generic;
using ScriptExtensions.Shared;
using UnityEngine;

namespace ScriptExtensions
{
	public static class VectorExtensions
	{
		public static Vector3 MultiplyVector(in Vector3 a, in Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		
		public static Vector2 MultiplyVector(in Vector2 a, in Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);
		
		public static Vector3 GetCenterOfVectors(IEnumerable<Vector3> vectors)
		{
			Vector3 center = Vector3.zero;
			int count = 0;

			foreach (Vector3 vector in vectors)
			{
				center += vector;
				count++;
			}

			return center / count;
		}
		
		public static Vector2 GetCenterOfVectors(IEnumerable<Vector2> vectors)
		{
			Vector2 center = Vector2.zero;
			int count = 0;

			foreach (Vector2 vector in vectors)
			{
				center += vector;
				count++;
			}

			return center / count;
		}
		
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

		public static Vector3 Multiply(this Vector3 a, in Vector3 b)
		{
			a.x *= b.x;
			a.y *= b.y;
			a.z *= b.z;
			return a;
		}

		/// <summary>
		/// Return the value multiplied by <see cref="Time.deltaTime"/>
		/// </summary>
		public static Vector3 Delta(this Vector3 value) => value * Time.deltaTime;

		/// <summary>
		/// Swizzle the Vector2 XY to a Vector3 XZ.
		/// Vector2(1, 2) -> Vector3(1, 0, 2)
		/// </summary>
		public static Vector3 SwizzleXZ(this Vector2 vector) => new Vector3(vector.x, 0f, vector.y);

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

		#region Vector2 Extensions

		public static Vector2 SetX(this Vector2 vector, in float x)
		{
			vector.x = x;
			return vector;
		}

		public static Vector2 SetY(this Vector2 vector, in float y)
		{
			vector.y = y;
			return vector;
		}

		public static Vector2 Multiply(this Vector2 a, in Vector2 b)
		{
			a.x *= b.x;
			a.y *= b.y;
			return a;
		}

		/// <summary>
		/// Return the value multiplied by <see cref="Time.deltaTime"/>
		/// </summary>
		public static Vector2 Delta(this Vector2 value) => value * Time.deltaTime;

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