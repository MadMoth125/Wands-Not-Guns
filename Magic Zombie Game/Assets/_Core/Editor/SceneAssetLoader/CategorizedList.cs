using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Core.Editor.SceneLoader
{
	public struct CategorizedList
	{
		public CategorizedList(string category)
		{
			Category = category;
			scenes = new List<LoadedSceneData>();
		}
			
		public string Category { get; }
			
		// [ShowInInspector]
		[LabelText("@Category")] 
		public readonly List<LoadedSceneData> scenes;
	}
}