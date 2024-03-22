using InputMapping;
using UnityEngine;

namespace InputModule
{
	public class InputVisuals : MonoBehaviour
	{
		public RectTransform moveableObject;
	
		public InputScriptableObject inputAsset;
	
		#region Unity Methods

		private void Update()
		{
			if (moveableObject == null || inputAsset == null) return;
			moveableObject.position = inputAsset.GetAbsoluteCursorPosition();
		}

		private void OnEnable()
		{
			inputAsset.EnableInput();
		}

		private void OnDisable()
		{
			inputAsset.DisableInput();
		}

		#endregion
	}
}