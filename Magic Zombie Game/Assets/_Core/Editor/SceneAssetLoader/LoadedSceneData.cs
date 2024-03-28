using Sirenix.OdinInspector;
using UnityEditor.SceneManagement;

namespace Core.Editor.SceneLoader
{
	public struct LoadedSceneData
	{
		public LoadedSceneData(string name, string path)
		{
			sceneName = name;
			scenePath = path;
		}

		[DisplayAsString] 
		[ShowInInspector] 
		[SuffixLabel("name")] 
		[HideLabel]
		public string sceneName;

		[DisplayAsString] 
		[ShowInInspector] 
		[SuffixLabel("path")] 
		[HideLabel]
		public string scenePath;
		
		[Button("Load")]
		public void LoadScene()
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene(scenePath);
			}
		}
	}
}