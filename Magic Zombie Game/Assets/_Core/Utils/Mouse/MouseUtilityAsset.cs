using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Utils
{
	/* Exposed items
	 *
	 * Properties:
	 * - float RaycastDistance
	 * - LayerMask Layers
	 * - LayerMask IgnoredUILayers
	 *
	 * Serialized Fields:
	 * - float raycastDistance
	 * - LayerMask layers
	 * - LayerMask ignoredUILayers
	 * - bool instantiateEventSystem
	 * 
	 * Methods:
	 * - Vector3 GetMouseWorldPosition()
	 * - Vector3 GetMouseWorldPosition(Vector2 screenPosition)
	 * - Vector3 GetMouseWorldPosition(Vector2, LayerMask layerMask)
	 * - Vector3 GetDirectionTowardsMouse(Vector3 start, bool flattenDirection)
	 * - bool IsMouseOverUI()
	 * - bool IsMouseOverHittableUI()
	 */
	
	/// <summary>
	/// Simple ScriptableObject utility to get data from the mouse on the screen.
	/// </summary>
	[CreateAssetMenu(fileName = "MouseUtils", menuName = "Mouse Utility")]
	public class MouseUtilityAsset : ScriptableObject
	{
		public float RaycastDistance => raycastDistance;
		public LayerMask Layers => layers;
		public LayerMask IgnoredUILayers => ignoredUILayers;

		[Title("Raycast Settings")]
		[SerializeField]
		private float raycastDistance = 1000f;
	
		[Tooltip("The layers to raycast against when finding the world position of the mouse cursor.\n\n" +
		         "Set to \"Everything\" by default.")]
		[SerializeField]
		private LayerMask layers = ~0; // layer mask set to "Everything"
	
		[Tooltip("The layers to ignore when determining if the mouse is over a UI element.\n\n" +
		         "(e.g. A UI element is on layer \"IgnoreUI,\" " +
		         "and this layer mask includes \"IgnoreUI\", " +
		         "the UI element will not be considered when checking any \"hittable\" elements.)")]
		[SerializeField]
		private LayerMask ignoredUILayers = default; // UI layer
		
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

		#region Static Methods

		public static Vector2 NormalizeScreenPosition(Vector2 screenPosition, bool clamp)
		{
			return new Vector2(
				clamp ? Mathf.Clamp01(screenPosition.x / Screen.width) : screenPosition.x / Screen.width,
				clamp ? Mathf.Clamp01(screenPosition.y / Screen.height) : screenPosition.y / Screen.height);
		}

		#endregion
		
		#region Mouse Position Methods
		
		/// <summary>
		/// Gets the world position of the mouse on the screen.
		/// </summary>
		/// <returns>The world position of the mouse.</returns>
		public Vector3 GetMouseWorldPosition() => GetMouseWorldPosition(Input.mousePosition, Layers);

		/// <summary>
		/// Gets the world position of the mouse on the screen.
		/// </summary>
		/// <param name="screenPosition">The absolute position of the mouse on the screen.</param>
		/// <returns>The world position of the mouse.</returns>
		public Vector3 GetMouseWorldPosition(Vector2 screenPosition) => GetMouseWorldPosition(screenPosition, Layers);
	
		/// <summary>
		/// Gets the world position of the mouse on the screen.
		/// Raycasts against the provided layer mask to determine the world position.
		/// </summary>
		/// <param name="screenPosition">The absolute position of the mouse on the screen.</param>
		/// <param name="layerMask">The layer mask to raycast against.</param>
		/// <returns>The world position of the mouse.</returns>
		public Vector3 GetMouseWorldPosition(Vector2 screenPosition, LayerMask layerMask)
		{
			if (!CheckCameraReference()) return _cachedMouseWorldPosition;
		
			Ray ray = _mainCamera.ScreenPointToRay(screenPosition);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, RaycastDistance, layerMask))
			{
				_cachedMouseWorldPosition = hitInfo.point;
				return hitInfo.point;
			}

			return _cachedMouseWorldPosition;
		}
		
		#endregion

		#region Mouse Interaction Methods

		public bool IsMouseOverComponent<T>(out T component)
		{
			component = default;
			
			if (!CheckCameraReference()) return false;
			if (IsMouseOverHittableUI()) return false;

			if (!Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition),
				    out RaycastHit hitInfo, RaycastDistance, Layers)) return false;
			
			component = hitInfo.collider.GetComponent<T>();
			return component != null;
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
			return CheckEventSystemReference() && _cachedEventSystem.IsPointerOverGameObject();
		}
	
		/// <summary>
		/// Determines if the mouse is over a UI element that is not on an ignored layer.
		/// Handles the instantiation of an <see cref="EventSystem"/> if none is found in the scene.
		/// </summary>
		public bool IsMouseOverHittableUI()
		{
			if (!CheckEventSystemReference()) return false;
			
			_cachedPointerEventData ??= new PointerEventData(_cachedEventSystem);
			_cachedPointerEventData.position = Input.mousePosition;

			_cachedEventSystem.RaycastAll(_cachedPointerEventData, _raycastResults);

			// return true if any of the raycast results are not on an ignored layer
			return _raycastResults.Any(result => (IgnoredUILayers.value & 1 << result.gameObject.layer) == 0);
		}
		
		/// <summary>
		/// Determines if the <see cref="Camera.main"/> <see cref="Camera"/> reference is valid.
		/// Camera is cached upon being found.
		/// </summary>
		private bool CheckCameraReference()
		{
			#if UNITY_EDITOR
			
			if (!Application.isPlaying) return false;
			
			#endif
			
			if (_mainCamera == null)
			{
				if (Camera.main == null)
				{
					Debug.LogWarning($"{nameof(MouseUtilityAsset)}: " +
					               $"No camera found in the scene",this);
					return false;
				}
			
				_mainCamera = Camera.main;
			}

			return true;
		}
		
		/// <summary>
		/// Determines if the <see cref="EventSystem.current"/> <see cref="EventSystem"/> reference is valid.
		/// If the reference is null, and <see cref="instantiateEventSystem"/> is true, a new <see cref="EventSystem"/> will be instantiated.
		/// </summary>
		private bool CheckEventSystemReference()
		{
			#if UNITY_EDITOR
			
			if (!Application.isPlaying) return false;
			
			#endif
			
			if (_cachedEventSystem == null)
			{
				if (EventSystem.current == null)
				{
					if (instantiateEventSystem)
					{
						Debug.LogWarning($"{nameof(MouseUtilityAsset)}: " +
						                 $"No EventSystem found in the scene. Instantiating a new one.", this);
						_cachedEventSystem = new GameObject(
								"EventSystem (Generated)",
								typeof(EventSystem),
								typeof(StandaloneInputModule))
							.GetComponent<EventSystem>();
					}
					else
					{
						Debug.LogWarning($"{nameof(MouseUtilityAsset)}: " +
						               $"No EventSystem found in the scene", this);
						return false;
					}
				}
			}

			return true;
		}
	}
}