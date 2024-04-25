using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using Random = System.Random;

namespace ScriptExtensions
{
	public static class CollectionExtensions
	{
		#region Shuffle

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
		{
			return enumerable.ToList().Shuffle();
		}
		
		public static ICollection<T> Shuffle<T>(this ICollection<T> collection)
		{
			return collection.ToList().Shuffle();
		}
		
		public static IList<T> Shuffle<T>(this IList<T> list)
		{
			for (int i = list.Count - 1; i >= 1; i--)
			{
				int j = random.Next(i + 1);
				(list[i], list[j]) = (list[j], list[i]);
			}

			return list;
		}

		#endregion

		#region Random

		public static T Random<T>(this IEnumerable<T> enumerable)
		{
			return enumerable.ToList().Random();
		}
		
		public static T Random<T>(this ICollection<T> collection)
		{
			return collection.ToList().Random();
		}
		
		public static T Random<T>(this IList<T> list, out int index)
		{
			index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}

		public static T Random<T>(this IList<T> list)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		#endregion

		#region Get Middle

		public static T GetMiddle<T>(this IEnumerable<T> enumerable)
		{
			return enumerable.ToList().GetMiddle();
		}
		
		public static T GetMiddle<T>(this ICollection<T> collection)
		{
			return collection.ToList().GetMiddle();
		}
		
		public static T GetMiddle<T>(this IList<T> list)
		{
			return list[Mathf.FloorToInt(list.Count * 0.5f)];
		}

		#endregion

		public static IList<T> GetElements<T>(this IList<T> list, params int[] indices)
		{
			#if UNITY_EDITOR
			
			for (int i = 0; i < indices.Length; i++)
			{
				if (list.InvalidIndex(indices[i]))
				{
					throw new ArgumentOutOfRangeException($"{nameof(GetElements)}(): index '{indices[i]}' out of range.");
				}
			}
			
			#endif
			
			return indices.Select(index => list[index]).ToList();
		}
		
		public static IList<T> Swap<T>(this IList<T> list, int index1, int index2)
		{
			#if UNITY_EDITOR
			
			if (list.InvalidIndex(index1))
			{
				throw new ArgumentOutOfRangeException($"{nameof(Swap)}(): '{nameof(index1)}' index {index1} out of range.");
			}
			
			if (list.InvalidIndex(index2))
			{
				throw new ArgumentOutOfRangeException($"{nameof(Swap)}(): '{nameof(index2)}' index {index2} out of range.");
			}
			
			#endif
			
			(list[index1], list[index2]) = (list[index2], list[index1]);
			return list;
		}
		
		public static IList<T> MoveIndex<T>(this IList<T> list, int oldIndex, int newIndex)
		{
			#if UNITY_EDITOR
			
			if (list.InvalidIndex(oldIndex))
			{
				throw new ArgumentOutOfRangeException($"{nameof(MoveIndex)}(): '{nameof(oldIndex)}' index {oldIndex} out of range.");
			}
			
			if (list.InvalidIndex(newIndex))
			{
				throw new ArgumentOutOfRangeException($"{nameof(MoveIndex)}(): '{nameof(newIndex)}' index {newIndex} out of range.");
			}
			
			#endif

			if (oldIndex == newIndex) return list;
			T item = list[oldIndex];
			list.RemoveAt(oldIndex);
			list.Insert(newIndex, item);
			return list;
		}
		
		#region Null Or Empty

		public static bool NullOrEmpty<T>(this IEnumerable<T> enumerable) 
		{
			return enumerable == null || !enumerable.Any();
		}
		
		public static bool NullOrEmpty<T>(this ICollection<T> collection) 
		{
			return collection == null || collection.Count == 0;
		}
		
		public static bool NullOrEmpty<T>(this IList<T> list) 
		{
			return list == null || list.Count == 0;
		}

		#endregion

		#region Validation

		public static bool ValidIndex<T>(this IEnumerable<T> enumerable, int index)
		{
			return index >= 0 && index < enumerable.Count();
		}
		
		public static bool InvalidIndex<T>(this IEnumerable<T> enumerable, int index)
		{
			return index < 0 && index >= enumerable.Count();
		}
		
		public static bool Valid<T>(this IEnumerable<T> enumerable)
		{
			return enumerable != null && enumerable.Any();
		}

		public static bool Invalid<T>(this IEnumerable<T> enumerable)
		{
			return enumerable == null || !enumerable.Any();
		}

		#endregion
		
		private static readonly Random random = new Random();
	}
}