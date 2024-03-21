using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Core.TickSystem.Editor
{
	public class TickSystemEditor : OdinMenuEditorWindow
	{
		private const string ASSET_PATH = "Assets/_Core/TickSystem/Resources/TickSystemBackend.asset";
		
		[MenuItem("Window/Tick System")]
		public static void OpenWindow()
		{
			GetWindow<TickSystemEditor>().Show();
		}

		protected override OdinMenuTree BuildMenuTree()
		{
			var tree = new OdinMenuTree();
			tree.AddAssetAtPath("Tick System", ASSET_PATH);
			return tree;
		}
	}
}