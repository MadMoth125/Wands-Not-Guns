using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Utils
{
	// TODO: Integrate with Scriptable Objects for customizable settings and simpler method calls
	public static class MouseUtils
	{
		private static Camera mainCamera;
		private static EventSystem cachedEventSystem;
		private static PointerEventData cachedPointerEventData;
		private static Vector3 cachedMouseWorldPosition = Vector3.zero;
		private static readonly List<RaycastResult> raycastResults = new();

		#region Screen Position Methods

		/// <summary>
		/// Clamp the provided screen position to the bounds of the current screen resolution.
		/// </summary>
		/// <param name="screenPosition">The screen position.</param>
		/// <returns>The clamped screen position.</returns>
		public static Vector2 ClampScreenPosition(Vector2 screenPosition)
		{
			return new Vector2(
				Mathf.Clamp(screenPosition.x, 0f, Screen.width),
				Mathf.Clamp(screenPosition.y, 0f, Screen.height));
		}
	
		/// <summary>
		/// Normalizes the provided screen position based on the current screen resolution.
		/// Values are clamped between 0 and 1 if <paramref name="clamp"/> is true.
		/// </summary>
		/// <param name="screenPosition">The screen position.</param>
		/// <param name="clamp">Whether to clamp the normalized value between 0 and 1.</param>
		/// <returns>The normalized screen position.</returns>
		public static Vector2 NormalizeScreenPosition(Vector2 screenPosition, bool clamp)
		{
			return new Vector2(
				clamp ? Mathf.Clamp01(screenPosition.x / Screen.width) : screenPosition.x / Screen.width,
				clamp ? Mathf.Clamp01(screenPosition.y / Screen.height) : screenPosition.y / Screen.height);
		}

		#endregion

		#region Mouse Get Component Methods

		/// <summary>
		/// Gets if the mouse is over a component of type <typeparamref name="T"/>.
		/// If so, the component is returned in the out parameter.
		/// Uses <see cref="Input.mousePosition"/> as the screen position.
		/// </summary>
		/// <param name="distance">The distance to raycast.</param>
		/// <param name="layerMask">The layer mask to raycast against.</param>
		/// <param name="component">The component that the mouse is over.</param>
		/// <typeparam name="T">The type of component to check for.</typeparam>
		/// <returns>If the mouse is over a component of type <typeparamref name="T"/>.</returns>
		public static bool IsMouseOverComponent<T>(float distance, LayerMask layerMask, out T component)
		{
			return IsMouseOverComponent(Input.mousePosition, distance, layerMask, out component);
		}

		/// <summary>
		/// Gets if the mouse is over a component of type <typeparamref name="T"/>.
		/// If so, the component is returned in the out parameter.
		/// </summary>
		/// <param name="screenPosition">The absolute position of the mouse on the screen.</param>
		/// <param name="distance">The distance to raycast.</param>
		/// <param name="layerMask">The layer mask to raycast against.</param>
		/// <param name="component">The component that the mouse is over.</param>
		/// <typeparam name="T">The type of component to check for.</typeparam>
		/// <returns>If the mouse is over a component of type <typeparamref name="T"/>.</returns>
		public static bool IsMouseOverComponent<T>(Vector2 screenPosition, float distance, LayerMask layerMask,
			out T component)
		{
			component = default;
		
			if (!CameraValid()) return false;

			var ray = mainCamera.ScreenPointToRay(screenPosition);
			if (!Physics.Raycast(ray, out var hitInfo, distance, layerMask)) return false;
			
			component = hitInfo.collider.GetComponent<T>();
			return component != null;
		}

		#endregion

		#region Mouse World Position Methods

		/// <summary>
		/// Gets the world position of the mouse on the screen.
		/// Raycasts against the provided layer mask to determine the world position.
		/// Uses <see cref="Input.mousePosition"/> as the screen position.
		/// </summary>
		/// <param name="distance">The distance to raycast.</param>
		/// <param name="layerMask">The layer mask to raycast against.</param>
		/// <returns>The world position of the mouse.</returns>
		public static Vector3 GetMouseWorldPosition(float distance, LayerMask layerMask)
		{
			return GetMouseWorldPosition(Input.mousePosition, distance, layerMask);
		}
	
		/// <summary>
		/// Gets the world position of the mouse on the screen.
		/// Raycasts against the provided layer mask to determine the world position.
		/// </summary>
		/// <param name="screenPosition">The absolute position of the mouse on the screen.</param>
		/// <param name="distance">The distance to raycast.</param>
		/// <param name="layerMask">The layer mask to raycast against.</param>
		/// <returns>The world position of the mouse.</returns>
		public static Vector3 GetMouseWorldPosition(Vector2 screenPosition, float distance, LayerMask layerMask)
		{
			if (!CameraValid()) return cachedMouseWorldPosition;
		
			var ray = mainCamera.ScreenPointToRay(screenPosition);
			if (Physics.Raycast(ray, out var hitInfo, distance, layerMask))
			{
				cachedMouseWorldPosition = hitInfo.point;
				return hitInfo.point;
			}

			return cachedMouseWorldPosition;
		}

		#endregion

		#region Mouse Over UI Methods

		/// <summary>
		/// Determines if the mouse is over a UI element.
		/// Handles the instantiation of an &lt;see cref="EventSystem"/&gt; if none is found in the scene.
		/// </summary>
		/// <param name="instantiateEventSystem">Whether to instantiate an <see cref="EventSystem"/> if none is found.</param>
		/// <returns>If the mouse is over any UI element.</returns>
		public static bool IsMouseOverAnyUI(bool instantiateEventSystem)
		{
			return EventSystemValid(instantiateEventSystem) && cachedEventSystem.IsPointerOverGameObject();
		}

		/// <summary>
		/// Determines if the mouse is over a UI element that is on a specified layer (or layers.)
		/// Handles the instantiation of an <see cref="EventSystem"/> if none is found in the scene.
		/// </summary>
		/// <param name="layers">The layer mask to check against.</param>
		/// <param name="instantiateEventSystem">Whether to instantiate an <see cref="EventSystem"/> if none is found.</param>
		/// <returns>If the mouse is over a UI element on the specified layer.</returns>
		public static bool IsMouseOverBlockingUI(LayerMask layers, bool instantiateEventSystem)
		{
			if (!EventSystemValid(instantiateEventSystem)) return false;
			
			cachedPointerEventData ??= new PointerEventData(cachedEventSystem);
			cachedPointerEventData.position = Input.mousePosition;

			cachedEventSystem.RaycastAll(cachedPointerEventData, raycastResults);

			// return true if any of the raycast results are not on an ignored layer
			return raycastResults.Any(result => (layers.value & 1 << result.gameObject.layer) != 0);
		}

		#endregion

		#region Validation Methods

		/// <summary>
		/// Determines if the <see cref="Camera.main"/> <see cref="Camera"/> reference is valid.
		/// Camera is cached upon being found.
		/// </summary>
		private static bool CameraValid()
		{
			#if UNITY_EDITOR
			
			if (!Application.isPlaying) return false;
			
			#endif
			
			if (mainCamera == null)
			{
				if (Camera.main == null)
				{
					Debug.LogWarning($"{nameof(MouseUtils)}: No camera found in the scene");
					return false;
				}
			
				mainCamera = Camera.main;
			}

			return true;
		}
	
		/// <summary>
		/// Determines if the <see cref="EventSystem.current"/> <see cref="EventSystem"/> reference is valid.
		/// If the reference is null, and <see cref="instantiateEventSystem"/> is true, a new <see cref="EventSystem"/> will be instantiated.
		/// </summary>
		private static bool EventSystemValid(bool instantiateEventSystem)
		{
			#if UNITY_EDITOR
			
			if (!Application.isPlaying) return false;
			
			#endif

			if (cachedEventSystem != null) return true;
		
			if (EventSystem.current != null)
			{
				cachedEventSystem = EventSystem.current;
				return true;
			}
		
			if (instantiateEventSystem)
			{
				Debug.LogWarning($"{nameof(MouseUtils)}: No EventSystem found in the scene. Instantiating a replacement.");
				cachedEventSystem = new GameObject("EventSystem (replacement)",
						typeof(EventSystem), typeof(StandaloneInputModule))
					.GetComponent<EventSystem>();
			}
			else
			{
				Debug.LogWarning($"{nameof(MouseUtils)}: No EventSystem found in the scene");
				return false;
			}

			return true;
		}

		#endregion
	}
}