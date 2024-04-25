using System.Collections.Generic;
using RotaryHeart.Lib.PhysicsExtension;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using PhysicsEx = RotaryHeart.Lib.PhysicsExtension.Physics;

namespace Enemy
{
	public class ProximityHandler : MonoBehaviour
	{
		#region Properties

		public Vector3 PositionOffset
		{
			get => positionOffset;
			set => positionOffset = value;
		}

		public float DetectionRadius
		{
			get => detectionRadius;
			set => detectionRadius = Mathf.Max(0f, value);
		}

		public float DetectionAngle
		{
			get => detectionAngle;
			set => detectionAngle = Mathf.Clamp(value, 0f, 360f);
		}

		#endregion

		#region Fields

		[TitleGroup("Detection Settings", Order = 10)]
		[SerializeField]
		private Vector3 positionOffset = Vector3.zero;
		
		[TitleGroup("Detection Settings")]
		[SerializeField]
		private float detectionRadius = 5f;

		[TitleGroup("Detection Settings")]
		[PropertyRange(0f, 360f)]
		[SerializeField]
		private float detectionAngle = 45f;
		
		[TitleGroup("Detection Settings")]
		[SerializeField]
		private LayerMaskVariable layers;

		#endregion
		
		private readonly List<GameObject> _overlappedObjects = new();
		
		/// <summary>
		/// Returns if there are any objects found in the list of cached overlapped objects.
		/// </summary>
		public bool AnyOverlap()
		{
			return _overlappedObjects.Count > 0;
		}
		
		/// <summary>
		/// Gets the cached list of overlapped objects.
		/// Recommended to use <see cref="CheckProximity"/> first to get accurate results.
		/// </summary>
		/// <returns>A read-only list of overlapped objects.</returns>
		public IReadOnlyList<GameObject> GetOverlappedObjects()
		{
			return _overlappedObjects;
		}

		/// <summary>
		/// Called when an object is detected within the proximity and view angle.
		/// </summary>
		/// <param name="target">The detected object.</param>
		protected virtual void OnOverlapDetected(GameObject target)
		{
			
		}

		/// <summary>
		/// Called when an object is lost from the proximity and view angle.
		/// Object must have been previously detected for this to be called.
		/// </summary>
		/// <param name="target">The lost object.</param>
		protected virtual void OnOverlapLost(GameObject target)
		{
			
		}

		/// <summary>
		/// Checks for objects within the proximity and view angle.
		/// </summary>
		protected void CheckProximity()
		{
			// indices for the 'colliders' loop get skipped when the object is not within the detection angle.
			// So we track a separate index to ensure indices are contiguous for the '_overlappedObjects' list.
			int savedIndex = 0;
			
			var colliders = PhysicsEx.OverlapSphere(
				transform.position + positionOffset,
				detectionRadius,
				layers.Value,
				PreviewCondition.Editor,
				0.1f,
				Color.green,
				Color.red,
				true);
			
			// Skip if there are no colliders
			if (colliders.Length > 0)
			{
				for (int i = 0; i < colliders.Length; i++)
				{
					// skip self
					if (colliders[i].gameObject == gameObject) continue;
					
					// skip if object is not within detection angle
					if (!WithinDetectionAngle(colliders[i].transform)) continue;
					
					if (i >= _overlappedObjects.Count)
					{
						// add new object if index does not exist
						_overlappedObjects.Add(colliders[i].gameObject);
						OnOverlapDetected(colliders[i].gameObject);
					}
					else
					{
						// replace object if index exists
						_overlappedObjects[savedIndex] = colliders[i].gameObject;
						OnOverlapDetected(colliders[i].gameObject);
					}

					savedIndex++;
				}
				
				// remove any excess objects
				if (_overlappedObjects.Count > savedIndex + 1)
				{
					_overlappedObjects.RemoveRange(savedIndex, _overlappedObjects.Count - savedIndex + 1);
				}
			}
			else
			{
				// avoid clearing the list if there are no objects
				if (_overlappedObjects.Count > 0)
				{
					_overlappedObjects.Clear();
				}
			}
		}

		/// <summary>
		/// Checks if the target is within the detection angle.
		/// </summary>
		/// <param name="target">The target to check.</param>
		/// <returns>If the target is within the detection angle.</returns>
		protected bool WithinDetectionAngle(Transform target)
		{
			Vector3 targetPos = target.position;
			Vector3 pos = transform.position + positionOffset;
			Vector3 forward = transform.forward;
			
			Vector3 direction = targetPos.SetY(pos.y) - pos;
			float angle = Vector3.Angle(direction, forward);

			return angle <= detectionAngle;
		}
		
		#region Unity Methods

		protected virtual void OnValidate()
		{
			detectionRadius = Mathf.Max(0f, detectionRadius);
			detectionAngle = Mathf.Clamp(detectionAngle, 0f, 360f);
		}

		#endregion
	}
}