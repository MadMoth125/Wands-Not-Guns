using System.Collections.Generic;
using ScriptExtensions.Shared;
using UnityEngine;

namespace ScriptExtensions
{
	public static class VectorExtensions
	{
		public static Vector3 MultiplyVector(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
		
		public static Vector2 MultiplyVector(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);
		
		#region Vector3 Extensions

		public static Vector3 SetX(this Vector3 vector, float x) => new(x, vector.y, vector.z);

		public static Vector3 SetY(this Vector3 vector, float y) => new(vector.x, y, vector.z);

		public static Vector3 SetZ(this Vector3 vector, float z) => new(vector.x, vector.y, z);

		public static Vector3 Multiply(this Vector3 a, Vector3 b) => new(a.x * b.x, a.y * b.y, a.z * b.z);
		
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
		public static (Vector3, float) FindClosestPosition(this Vector3 position, IEnumerable<Vector3> positions)
			=> FindClosestOrFurthestPosition(ref position, ref positions, PositionSortType.Closest);

		/// <summary>
		/// Find the furthest position from a list of positions.
		/// </summary>
		/// <param name="position">The origin position to search from.</param>
		/// <param name="positions">The list of positions to search through.</param>
		/// <returns>The furthest position from the list, and the distance squared from the origin position.</returns>
		public static (Vector3, float) FindFurthestPosition(this Vector3 position, IEnumerable<Vector3> positions) 
			=> FindClosestOrFurthestPosition(ref position, ref positions, PositionSortType.Furthest);

		#endregion

		#region Vector2 Extensions

		public static Vector2 SetX(this Vector2 vector, float x) => new(x, vector.y);
		
		public static Vector2 SetY(this Vector2 vector, float y) => new(vector.x, y);
		
		public static Vector2 Multiply(this Vector2 a, Vector2 b) => new(a.x * b.x, a.y * b.y);

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
		private static (Vector3, float) FindClosestOrFurthestPosition(ref Vector3 position, ref IEnumerable<Vector3> positions, PositionSortType searchType)
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