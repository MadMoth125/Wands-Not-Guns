using System;
using Sirenix.OdinInspector;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Core.Editor.SceneLoader
{
	/// <summary>
	/// Items are displayed in the inspector. 
	/// </summary>
	public struct SceneInspectorData
	{
		public SceneInspectorData(string name, string path)
		{
			_sceneName = name;
			_scenePath = path;
		}

		public string SceneName => _sceneName;
		
		public string ScenePath => _scenePath;
		
		[DisplayAsString] 
		[ShowInInspector] 
		[SuffixLabel("name")] 
		[HideLabel]
		private string _sceneName;

		[DisplayAsString] 
		[ShowInInspector] 
		[SuffixLabel("path")] 
		[HideLabel]
		private string _scenePath;
		
		[Tooltip("Load the scene.")]
		[Button("Load")]
		public void LoadScene()
		{
			try
			{
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorSceneManager.OpenScene(_scenePath);
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to load scene: {_sceneName}. Error: {e.Message}");
			}
		}
	}
}