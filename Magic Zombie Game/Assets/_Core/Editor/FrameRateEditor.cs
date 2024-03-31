using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Utils.Editor
{
	public class FrameRateEditor : OdinEditorWindow
	{
		private static int globalMaxFrameRate = 60;

		private static string globalFrameRateDisplay = "Uncapped";

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
		[DisplayAsString]
		[SerializeField]
		private string frameRateDisplay = "Uncapped";

		private const string UNCAPPED_DISPLAY_NAME = "Uncapped";

		#region Static Methods

		[MenuItem("Window/Custom Editors/Frame Rate Adjuster")]
		public static void OpenWindow() => GetWindow<FrameRateEditor>().Show();

		// [InitializeOnLoadMethod]
		private static void SetFrameRateAuto()
		{
			SetFrameRate(globalMaxFrameRate);
			globalFrameRateDisplay = GetFrameRateDisplay(globalMaxFrameRate);
		}

		private static void SetFrameRate(int frameRate)
		{
			globalMaxFrameRate = frameRate;
			Application.targetFrameRate = frameRate;
			globalFrameRateDisplay = GetFrameRateDisplay(globalMaxFrameRate);
		}

		private static string GetFrameRateDisplay(int frameRate)
		{
			return frameRate == -1 ? UNCAPPED_DISPLAY_NAME : frameRate.ToString();
		}

		private static void ResetFrameRate()
		{
			SetFrameRate(-1);
			globalFrameRateDisplay = GetFrameRateDisplay(-1);
		}

		#endregion

		[HorizontalGroup("Properties", Width = 0.2f)]
		[VerticalGroup("Properties/Buttons")]
		[Tooltip("'Apply' sets the application's frame rate to the value above.")]
		[Button("Apply", ButtonSizes.Medium)]
		public void Apply()
		{
			SetFrameRate(maxFrameRate);
			frameRateDisplay = GetFrameRateDisplay(maxFrameRate);
		}

		[HorizontalGroup("Properties")]
		[VerticalGroup("Properties/Buttons")]
		[Tooltip("'Reset' sets the application's frame rate of -1, essentially un-capping the frame rate.")]
		[Button("Reset", ButtonSizes.Medium)]
		public void Reset()
		{
			ResetFrameRate();
			maxFrameRate = 60;
			frameRateDisplay = GetFrameRateDisplay(globalMaxFrameRate);
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			
			maxFrameRate = globalMaxFrameRate;
			frameRateDisplay = globalFrameRateDisplay;
		}
	}
}