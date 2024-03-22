using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace CustomTickSystem.Editor
{
	public class TickSystemEditor : OdinEditorWindow
	{
		[InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
		[HideLabel]
		[SerializeField]
		private TickSystemAsset dataAsset;
	
		[MenuItem("Window/Tick")]
		public static void OpenWindow() => GetWindow<TickSystemEditor>().Show();
	
		protected override void OnEnable()
		{
			base.OnEnable();
			dataAsset ??= Resources.Load<TickSystemAsset>("TickSystemAsset");
		}
	}
}