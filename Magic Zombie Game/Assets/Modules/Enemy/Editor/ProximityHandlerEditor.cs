using UnityEditor;
using UnityEngine;

namespace Enemy.Editor
{
	[CustomEditor(typeof(ProximityHandler))]
	public class ProximityHandlerEditor : UnityEditor.Editor
	{
		[DrawGizmo(GizmoType.Active | GizmoType.Selected)]
		private static void DrawSelected(ProximityHandler target, GizmoType gizmoType)
		{
			Transform cachedTransform = target.transform;
			
			Gizmos.color = target.AnyOverlap() ? Color.green : Color.yellow;
			DrawFOV(cachedTransform.position + target.PositionOffset, cachedTransform.forward, target.DetectionAngle, target.DetectionRadius);
		}
		
		private static void DrawFOV(Vector3 origin, Vector3 direction, float angle, float distance)
		{
			Vector3 rightLimit = Quaternion.Euler(0, angle / 2, 0) * direction;
			Vector3 leftLimit = Quaternion.Euler(0, -angle / 2, 0) * direction;

			// Draw FOV lines
			Gizmos.DrawLine(origin, origin + rightLimit * distance);
			Gizmos.DrawLine(origin, origin + leftLimit * distance);
			
			// connect FOV lines
			DrawArc(origin, direction, rightLimit, leftLimit, angle, distance);
		}
		
		private static void DrawArc(Vector3 origin, Vector3 direction, Vector3 rightLimit, Vector3 leftLimit, float angle, float distance, float stepSize = 10f)
		{
			Vector3 lastPoint = origin + leftLimit * distance;
			for (float i = -angle / 2 + stepSize; i < angle / 2; i += stepSize)
			{
				Vector3 point = Quaternion.Euler(0, i, 0) * direction * distance + origin;
				Gizmos.DrawLine(lastPoint, point);
				lastPoint = point;
			}
			Gizmos.DrawLine(lastPoint, origin + rightLimit * distance);
		}
	}
}