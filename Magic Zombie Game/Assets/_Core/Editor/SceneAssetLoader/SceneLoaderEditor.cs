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
		private List<string> includeKeywords = new();

		[GUIColor("RGB(1.0, 0.8, 0.8)")]
		[SerializeField] 
		private List<string> excludeKeywords = new();

		[PropertyOrder(10)] // set to bottom of the window
		[ShowInInspector]
		[NonSerialized] 
		private readonly List<CategorizedList<SceneInspectorData>> _sceneLists = new();

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

		[Tooltip("Reload scene references.")]
		[Button("Reload")]
		private void ReloadSceneReferences()
		{
			ClearEmptyKeywords();
			ClearSceneLists();
			Categorize(GetAllSceneAssets().ToList(), DEFAULT_CATEGORY);
		}

		/// <summary>
		/// Categorizes the scene data into lists based on the sort keywords.
		/// If no sort keyword is found in the scene path, it will be added to a default category.
		/// </summary>
		/// <param name="sceneData">The scene data to categorize.</param>
		/// <param name="fallbackCategory">The category to assign scenes that don't contain any sort keywords.</param>
		private void Categorize(List<SceneDirectoryData> sceneData, string fallbackCategory)
		{
			foreach (var scene in sceneData)
			{
				bool categorized = false;
				
				foreach (string keyword in includeKeywords.Where(keyword => scene.ScenePath.Contains(keyword)))
				{
					AssignSceneToCategory(_sceneLists, scene, keyword);
					categorized = true;
					break; // Break out of the loop once categorized
				}

				if (categorized) continue;
				
				// If scene wasn't categorized, add it to the default category
				AssignSceneToCategory(_sceneLists, scene, fallbackCategory);
			}

			return;

			void AssignSceneToCategory(List<CategorizedList<SceneInspectorData>> scenesList, in SceneDirectoryData scene, string category)
			{
				if (scenesList.Any(x => x.Category == category))
				{
					scenesList.First(x => x.Category == category).list
						.Add(new SceneInspectorData(scene.SceneName, scene.ScenePath));
				}
				else
				{
					scenesList.Add(new CategorizedList<SceneInspectorData>(category));
					scenesList.Last().list.Add(new SceneInspectorData(scene.SceneName, scene.ScenePath));
				}
			}
		}

		/// <summary>
		/// Clears all scene lists and their contents.
		/// </summary>
		private void ClearSceneLists()
		{
			_sceneLists.ForEach(x => x.list.Clear());
			_sceneLists.Clear();
		}

		/// <summary>
		/// Loads all scene assets in the project and returns them as a collection of <see cref="SceneDirectoryData"/>.
		/// If a scene asset contains any of the excluded keywords, it will not be included in the collection.
		/// </summary>
		private IEnumerable<SceneDirectoryData> GetAllSceneAssets()
		{
			var scenes = AssetUtilities.GetAllAssetsOfType<SceneAsset>().ToList();
			var paths = scenes.Select(AssetDatabase.GetAssetPath).ToList();
			var sceneAssetData = new List<SceneDirectoryData>();
		
			// Filter out scenes that contain any of the excluded keywords
			for (int i = 0; i < scenes.Count; i++)
			{
				bool validPath = excludeKeywords.All(exKeyword => !paths[i].Contains(exKeyword));
				if (!validPath) continue;
				sceneAssetData.Add(new SceneDirectoryData(scenes[i].name, paths[i]));
			}

			return sceneAssetData;
		}
		
		/// <summary>
		/// Removes any empty or whitespace keywords from both keyword lists.
		/// </summary>
		private void ClearEmptyKeywords()
		{
			includeKeywords = includeKeywords.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
			excludeKeywords = excludeKeywords.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToList();
		}
	}
}