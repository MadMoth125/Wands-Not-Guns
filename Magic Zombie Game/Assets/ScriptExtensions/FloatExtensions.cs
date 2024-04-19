using UnityEngine;

namespace ScriptExtensions
{
	public static class FloatExtensions
	{
		/// <summary>
		/// Return the value multiplied by <see cref="Time.deltaTime"/>
		/// </summary>
		public static float Delta(this float value)
		{
			return value * Time.deltaTime;
		}
		
		/// <summary>
		/// Return if the value is 'equal' to another value. (within a tolerance)
		/// </summary>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
		/// <param name="tolerance">The tolerance for the comparison.</param>
		/// <returns>If the value is almost equal to another value.</returns>
		public static bool ApproxEquals(this float a, float b, float tolerance = 0.0001f)
		{
			return Mathf.Abs(a - b) < tolerance;
		}
	}
}