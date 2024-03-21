using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Core.TickSystem.Editor
{
	public class TickSystemEditor : OdinMenuEditorWindow
	{
		private const string TICK_SYSTEM_ASSET_NAME = "TickSystemBackend";
		private const string TICK_PARAMS_ASSET_NAME = "TickSystemParameters";
		private const string ASSET_PATH = "Assets/_Core/TickSystem/Resources/";
		private const string ASSET_SUFFIX = ".asset";
		
		[MenuItem("Window/Tick System")]
		public static void OpenWindow()
		{
			GetWindow<TickSystemEditor>().Show();
		}

		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree();
			tree.AddAssetAtPath("Tick System", ASSET_PATH + TICK_SYSTEM_ASSET_NAME + ASSET_SUFFIX);
			tree.AddAssetAtPath("System Options", ASSET_PATH + TICK_PARAMS_ASSET_NAME + ASSET_SUFFIX);
			return tree;
		}
	}
}