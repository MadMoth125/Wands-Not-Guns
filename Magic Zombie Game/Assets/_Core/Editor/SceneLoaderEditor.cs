using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Core.Utils.Editor
{
	public class SceneLoaderEditor : OdinEditorWindow
	{
		#if UNITY_EDITOR
		
		[ShowInInspector]
		[LabelText("Scenes")]
		[NonSerialized]
		private List<LoadedSceneData> _loadedScenes = new List<LoadedSceneData>();
		
		[ShowInInspector]
		[LabelText("Third-Party Scenes")]
		[NonSerialized]
		private List<LoadedSceneData> _thirdPartyScenes = new List<LoadedSceneData>();
		
		[ShowInInspector]
		[LabelText("Built-in Scenes")]
		[NonSerialized]
		private List<LoadedSceneData> _packageScenes = new List<LoadedSceneData>();
		
		#region Static Methods

		[MenuItem("Window/Scene Loader")]
		private static void OpenWindow() => GetWindow<SceneLoaderEditor>().Show();
		
		#endregion
		
		protected override void Initialize()
		{
			base.Initialize();

			ReloadSceneReferences();
		}

		[ButtonGroup("Reload Scenes References", Order = -1)]
		private void ReloadSceneReferences()
		{
			_loadedScenes.Clear();
			
			foreach (var sceneAsset in AssetUtilities.GetAllAssetsOfType<SceneAsset>())
			{
				string path = AssetDatabase.GetAssetPath(sceneAsset);
				
				if (path.Contains("/ThirdParty/"))
				{
					_thirdPartyScenes.Add(new LoadedSceneData(sceneAsset.name, path));
				}
				else if (path.Contains("/com."))
				{
					_packageScenes.Add(new LoadedSceneData(sceneAsset.name, path));
				}
				else
				{
					_loadedScenes.Add(new LoadedSceneData(sceneAsset.name, path));
				}
			}
		}
		
		private struct LoadedSceneData
		{
			public LoadedSceneData(string name, string path)
			{
				_sceneName = name;
				_scenePath = path;
			}
			
			[SuffixLabel("name")]
			[DisplayAsString]
			[ShowInInspector]
			[HideLabel]
			private string _sceneName;
			
			[SuffixLabel("path")]
			[DisplayAsString]
			[ShowInInspector]
			[HideLabel]
			private string _scenePath;
			
			[Button("Load")]
			public void LoadScene()
			{
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				{
					EditorSceneManager.OpenScene(_scenePath);
				}
			}
		}
		
		#endif
	}
	
}