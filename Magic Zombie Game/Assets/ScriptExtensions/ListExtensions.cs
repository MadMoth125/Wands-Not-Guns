using System;
using System.Collections;
using System.Collections.Generic;

namespace ScriptExtensions
{
	public static class ListExtensions
	{
		public static void AddItemAtPosition<T>(this List<T> list, T item, int position)
		{
			// If the position exceeds the current count of the list, adjust it
			if (position > list.Count)
			{
				position = list.Count;
			}

			// Insert the item at the specified position
			list.Insert(position, item);

			// Update the order of existing items
			for (int i = 0; i < list.Count; i++)
			{
				if (i != position && Comparer<int>.Default.Compare(i, position) >= 0)
				{
					// If the current index is not the inserted position and it's greater than or equal to the inserted position,
					// decrement it to maintain the correct order
					list.RemoveAt(i);
					list.Insert(i - 1, list[i]);
				}
			}
		}
	}
}