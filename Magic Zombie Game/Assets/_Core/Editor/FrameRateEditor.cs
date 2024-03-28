using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Core.Utils.Editor
{
	public class FrameRateEditor : OdinEditorWindow
	{
		[HorizontalGroup("Properties", Width = 0.8f)]
		[VerticalGroup("Properties/Variables")]
		[Range(1, 240)]
		[LabelWidth(80)]
		[LabelText("Target FPS:")]
		[SerializeField]
		private int maxFrameRate = 60;
		
		[HorizontalGroup("Properties")]
		[VerticalGroup("Properties/Variables")]
		[LabelWidth(80)]
		[LabelText("Current FPS:")]
		[DisableIf("@true")]
		[ShowInInspector]
		[DisplayAsString]
		private string _currentFrameRate = "Uncapped";

		#region Static Methods

		[MenuItem("Window/Custom Editors/Frame Rate Adjuster")]
		public static void OpenWindow() => GetWindow<FrameRateEditor>().Show();
	
		private static void SetFrameRate(int frameRate) => Application.targetFrameRate = frameRate;

		#endregion

		[HorizontalGroup("Properties", Width = 0.2f)]
		[VerticalGroup("Properties/Buttons")]
		[Tooltip("'Apply' sets the application's frame rate to the value above.")]
		[Button("Apply", ButtonSizes.Medium)]
		public void ApplyFrameRate()
		{
			Application.targetFrameRate = maxFrameRate;
			_currentFrameRate = Application.targetFrameRate == -1 ? "Uncapped" : Application.targetFrameRate.ToString();
		}

		[HorizontalGroup("Properties")]
		[VerticalGroup("Properties/Buttons")]
		[Tooltip("'Reset' sets the application's frame rate of -1, essentially un-capping the frame rate.")]
		[Button("Reset", ButtonSizes.Medium)]
		public void ResetFrameRate()
		{
			maxFrameRate = -1;
			ApplyFrameRate();
			maxFrameRate = 60;
		}
	}
}