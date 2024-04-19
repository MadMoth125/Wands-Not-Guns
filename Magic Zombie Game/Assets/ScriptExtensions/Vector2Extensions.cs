using System.Collections.Generic;
using UnityEngine;

namespace ScriptExtensions
{
	public static class Vector2Extensions
	{
		public static Vector2 MultiplyVector(in Vector2 a, in Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);

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

		/// <summary>
		/// Swizzle the Vector2 XY to a Vector3 XZ.
		/// Vector2(1, 2) -> Vector3(1, 0, 2)
		/// </summary>
		public static Vector3 SwizzleXZ(this Vector2 vector) => new Vector3(vector.x, 0f, vector.y);

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
	}
}