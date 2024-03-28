using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Core.Editor.SceneLoader
{
	/// <summary>
	/// A list of items that have a collective category.
	/// </summary>
	/// <typeparam name="T">The type of item in the list.</typeparam>
	public struct CategorizedList<T>
	{
		public CategorizedList(string category)
		{
			Category = category;
			list = new();
		}
		
		/// <summary>
		/// The category of the items in the list.
		/// </summary>
		public string Category { get; }

		/// <summary>
		/// The list of items.
		/// </summary>
		[LabelText("@Category")] 
		public readonly List<T> list;
	}
}