namespace Core.Editor.SceneLoader
{
	public struct SceneAssetData
	{
		public SceneAssetData(string name, string path)
		{
			SceneName = name;
			ScenePath = path;
		}
			
		public string SceneName { get; }
			
		public string ScenePath { get; }
	}
}