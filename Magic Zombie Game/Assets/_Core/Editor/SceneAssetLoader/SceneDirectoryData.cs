namespace Core.Editor.SceneLoader
{
	/// <summary>
	/// Data related to the scene and its directory.
	/// </summary>
	public struct SceneDirectoryData
	{
		public SceneDirectoryData(string name, string path)
		{
			SceneName = name;
			ScenePath = path;
		}
		
		/// <summary>
		/// The file name of the scene.
		/// </summary>
		public string SceneName { get; }
		
		/// <summary>
		/// The relative path of the scene from the Assets folder.
		/// </summary>
		public string ScenePath { get; }
		
		/// <summary>
		/// The extension of the scene file. (Value is always ".unity")
		/// </summary>
		public string Extension => ".unity";
	}
}