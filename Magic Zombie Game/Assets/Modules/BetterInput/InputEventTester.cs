using MyCustomControls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyCustomControls
{
	public class InputEventTester : MonoBehaviour
	{
		public ScriptableObjectGameControls gameControlsAsset;
		
		#region Unity Methods

		private void OnEnable()
		{
			gameControlsAsset.EnableControls();
			
			gameControlsAsset.OnMoveCallback += OnMoveListener;
			gameControlsAsset.OnMoveCursorCallback += OnMoveCursorListener;
			gameControlsAsset.OnUseAbilityCallback += OnUseAbilityListener;
			gameControlsAsset.OnInteractCallback += OnInteractListener;
		}

		private void OnDisable()
		{
			gameControlsAsset.DisableControls();
			
			gameControlsAsset.OnMoveCallback -= OnMoveListener;
			gameControlsAsset.OnMoveCursorCallback -= OnMoveCursorListener;
			gameControlsAsset.OnUseAbilityCallback -= OnUseAbilityListener;
			gameControlsAsset.OnInteractCallback -= OnInteractListener;
		}

		#endregion
		
		private void OnMoveListener(InputAction.CallbackContext ctx)
		{
			Debug.Log($"Move event: {ctx.ReadValue<Vector2>()}");
		}
		
		private void OnMoveCursorListener(InputAction.CallbackContext ctx)
		{
			Debug.Log($"Move cursor event: {ctx.ReadValue<Vector2>()}");
		}
		
		private void OnUseAbilityListener(InputAction.CallbackContext ctx)
		{
			Debug.Log($"Use ability event: {ctx.ReadValue<float>()}");
		}
		
		private void OnInteractListener(InputAction.CallbackContext ctx)
		{
			Debug.Log($"Interact event: {ctx.ReadValue<float>()}");
		}
	}
}