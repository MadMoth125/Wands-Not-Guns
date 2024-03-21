using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Core.Utils.Editor
{
	public class FrameRateEditorWindow : OdinEditorWindow
	{
		#if UNITY_EDITOR
		
		#region Static Methods

		[MenuItem("Window/Frame Rate Adjustment")]
		public static void OpenWindow() => GetWindow<FrameRateEditorWindow>().Show();
	
		private static void SetFrameRate(int frameRate) => Application.targetFrameRate = frameRate;

		#endregion

		[BoxGroup("Settings")]
		[PropertySpace(10, 0)]
		[Range(1, 240)]
		[LabelWidth(120)]
		[SerializeField]
		private int maxFrameRate = 60;
	
		[BoxGroup("Settings")]
		[PropertySpace(0, 10)]
		[LabelWidth(120)]
		[LabelText("Current:")]
		[ShowInInspector]
		[DisplayAsString]
		private string _currentFrameRate = "Uncapped";
	
		[BoxGroup("Settings")]
		[HorizontalGroup("Settings/Buttons")]
		[Tooltip("'Apply' sets the application's frame rate to the value above.")]
		[Button("Apply", ButtonSizes.Medium)]
		public void ApplyFrameRate()
		{
			Application.targetFrameRate = maxFrameRate;
			_currentFrameRate = Application.targetFrameRate == -1 ? "Uncapped" : Application.targetFrameRate.ToString();
		}
	
		[BoxGroup("Settings")]
		[HorizontalGroup("Settings/Buttons")]
		[Tooltip("'Reset' sets the application's frame rate of -1, essentially un-capping the frame rate.")]
		[Button("Reset", ButtonSizes.Medium)]
		public void ResetFrameRate()
		{
			maxFrameRate = -1;
			ApplyFrameRate();
			maxFrameRate = 60;
		}
		
		#endif
	}
}