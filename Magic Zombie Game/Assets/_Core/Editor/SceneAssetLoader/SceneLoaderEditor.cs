using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Core.Editor.SceneLoader
{
	public class SceneLoaderEditor : OdinEditorWindow
	{
		[SerializeField] 
		private List<string> categoryKeywords = new();
		
		[SerializeField] 
		private List<string> excludedKeywords = new();

		[PropertyOrder(10)] // set to bottom of the window
		[ShowInInspector]
		[NonSerialized] 
		private readonly List<CategorizedList> _sceneLists = new();

		private const string DEFAULT_CATEGORY = "Uncategorized";
		
		#region Static Methods

		[MenuItem("Window/Custom Editors/Scene Loader")]
		private static void OpenWindow() => GetWindow<SceneLoaderEditor>().Show();

		#endregion

		protected override void Initialize()
		{
			base.Initialize();

			ReloadSceneReferences();
		}

		[Tooltip("Re-categorize the scenes based on the keywords provided.")]
		[Button("Reload")]
		private void ReloadSceneReferences()
		{
			ClearEmptyKeywords();
			ClearSceneLists();
			Categorize();
		}

		private void Categorize()
		{
			List<SceneAssetData> sceneData = GetAllSceneAssets().ToList();

			foreach (var scene in sceneData)
			{
				bool categorized = false;
				
				foreach (string keyword in categoryKeywords.Where(keyword => scene.ScenePath.Contains(keyword)))
				{
					AssignSceneToCategory(_sceneLists, scene, keyword);
					categorized = true;
					break; // Break out of the loop once categorized
				}

				if (categorized) continue;
				
				// If scene wasn't categorized, add it to the default category
				AssignSceneToCategory(_sceneLists, scene, DEFAULT_CATEGORY);
			}

			return;

			void AssignSceneToCategory(List<CategorizedList> scenesList, in SceneAssetData scene, string category)
			{
				if (scenesList.Any(x => x.Category == category))
				{
					scenesList.First(x => x.Category == category).scenes
						.Add(new LoadedSceneData(scene.SceneName, scene.ScenePath));
				}
				else
				{
					scenesList.Add(new CategorizedList(category));
					scenesList.Last().scenes.Add(new LoadedSceneData(scene.SceneName, scene.ScenePath));
				}
			}
		}

		/// <summary>
		/// Clears all scene lists and their contents.
		/// </summary>
		private void ClearSceneLists()
		{
			_sceneLists.ForEach(x => x.scenes.Clear());
			_sceneLists.Clear();
		}

		/// <summary>
		/// Loads all scene assets in the project and returns them as a collection of <see cref="SceneAssetData"/>.
		/// If a scene asset contains any of the excluded keywords, it will not be included in the collection.
		/// </summary>
		private IEnumerable<SceneAssetData> GetAllSceneAssets()
		{
			var scenes = AssetUtilities.GetAllAssetsOfType<SceneAsset>().ToList();
			var paths = scenes.Select(AssetDatabase.GetAssetPath).ToList();
			var sceneAssetData = new List<SceneAssetData>();
		
			// Filter out scenes that contain any of the excluded keywords
			for (int i = 0; i < scenes.Count; i++)
			{
				bool validPath = excludedKeywords.All(exKeyword => !paths[i].Contains(exKeyword));
				if (!validPath) continue;
				sceneAssetData.Add(new SceneAssetData(scenes[i].name, paths[i]));
			}

			return sceneAssetData;
		}
		
		private void ClearEmptyKeywords()
		{
			categoryKeywords = categoryKeywords.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
			excludedKeywords = excludedKeywords.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
		}
	}
}