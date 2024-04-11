using System;
using System.Collections;
using System.Collections.Generic;

namespace ScriptExtensions
{
	public static class ArrayExtensions
	{
		public static T Random<T>(this IList<T> list, out int index)
		{
			index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
		
		public static T Random<T>(this IList<T> list)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
	}
}