using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Core.Utils
{
	/// <summary>
	/// Simple ScriptableObject utility to get data from the mouse on the screen.
	/// </summary>
	[CreateAssetMenu(fileName = "MouseUtils", menuName = "Input/Mouse Utility")]
	public class MouseUtilityAsset : ScriptableObject
	{
		public float RaycastDistance => raycastDistance;
		public LayerMask HittableLayers => hittableLayers;
		public LayerMask HittableLayersUI => hittableLayersUI;

		[Title("Raycast Settings")]
		[SerializeField]
		private float raycastDistance = 1000f;
		
		[Tooltip("The layers to raycast against when finding the world position of the mouse cursor.\n\n" +
		         "Set to \"Everything\" by default.")]
		[SerializeField]
		private LayerMask hittableLayers = ~0; // layer mask set to "Everything"
		
		[Tooltip("The layers to check when determining if the mouse is over a UI element.")]
		[SerializeField]
		private LayerMask hittableLayersUI = default; // UI layer
		
		[Title("Reference Settings")]
		[Tooltip("If true, an EventSystem GameObject will be instantiated if none is found in the scene\n\n" +
		         "Relevant for when \"IsMouseOverUI()\" is called.")]
		[SerializeField]
		private bool instantiateEventSystem = false;
	
		private readonly List<RaycastResult> _raycastResults = new();
		
		private Camera _mainCamera;
		private EventSystem _cachedEventSystem;
		private PointerEventData _cachedPointerEventData;
		private Vector3 _cachedMouseWorldPosition = Vector3.zero;
		
		#region Mouse Position Methods
		
		/// <summary>
		/// Gets the world position of the mouse on the screen.
		/// </summary>
		/// <returns>The world position of the mouse.</returns>
		public Vector3 GetMouseWorldPosition()
		{
			var mousePos = MouseUtils.ClampScreenPosition(Input.mousePosition);
			return MouseUtils.GetMouseWorldPosition(mousePos, RaycastDistance, HittableLayers);
		}

		/// <summary>
		/// Gets the world position of the mouse on the screen.
		/// </summary>
		/// <param name="screenPosition">The absolute position of the mouse on the screen.</param>
		/// <returns>The world position of the mouse.</returns>
		public Vector3 GetMouseWorldPosition(Vector2 screenPosition)
		{
			var mousePos = MouseUtils.ClampScreenPosition(screenPosition);
			return MouseUtils.GetMouseWorldPosition(mousePos, RaycastDistance, HittableLayers);
		}

		#endregion

		#region Mouse Interaction Methods

		public bool TryGetComponentUnderMouse<T>(out T component)
		{
			var mousePos = MouseUtils.ClampScreenPosition(Input.mousePosition);
			return MouseUtils.IsMouseOverComponent<T>(mousePos, RaycastDistance, HittableLayers, out component);
		}
		
		public bool TryGetComponentUnderMouse<T>(Vector2 screenPosition, out T component)
		{
			var mousePos = MouseUtils.ClampScreenPosition(screenPosition);
			return MouseUtils.IsMouseOverComponent<T>(mousePos, RaycastDistance, HittableLayers, out component);
		}

		#endregion
		
		/// <summary>
		/// Returns the vector direction from the provided start position to the mouse world position.
		/// </summary>
		/// <param name="start">The start position to calculate the direction from.</param>
		/// <param name="flattenDirection">If true, the direction will be flattened on the y-axis.</param>
		/// <returns>A normalized vector pointing from the start position to the mouse world position.</returns>
		public Vector3 GetDirectionTowardsMouse(Vector3 start, bool flattenDirection = true)
		{
			if (!flattenDirection) return (GetMouseWorldPosition() - start).normalized;
			
			Vector3 tempDir = GetMouseWorldPosition();
			return (new Vector3(tempDir.x, start.y, tempDir.z) - start).normalized;
		}
		
		/// <summary>
		/// Determines if the mouse is over a UI element.
		/// Handles the instantiation of an <see cref="EventSystem"/> if none is found in the scene.
		/// </summary>
		public bool IsMouseOverAnyUI()
		{
			return MouseUtils.IsMouseOverAnyUI(instantiateEventSystem);
		}
	
		/// <summary>
		/// Determines if the mouse is over a UI element that is not on an ignored layer.
		/// Handles the instantiation of an <see cref="EventSystem"/> if none is found in the scene.
		/// </summary>
		public bool IsMouseOverHittableUI()
		{
			return MouseUtils.IsMouseOverBlockingUI(HittableLayersUI, instantiateEventSystem);
		}
	}
}