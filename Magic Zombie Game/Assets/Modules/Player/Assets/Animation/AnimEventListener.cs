using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Player.AnimHandling
{
	[DisallowMultipleComponent]
	public class AnimEventListener : MonoBehaviour
	{
		[UnityEventsCategory]
		[SerializeField]
		private UnityEvent<Vector3> onRightFootStep = new();
		
		[UnityEventsCategory]
		[SerializeField]
		private UnityEvent<Vector3> onLeftFootStep = new();
		
		[UnityEventsCategory]
		[SerializeField]
		private UnityEvent<Vector3> onFootStep = new();
		
		[Tooltip("Enable or disable footstep events.")]
		[SerializeField]
		private bool enableFootStepEvents = true;
		
		[EnableIf(nameof(enableFootStepEvents))]
		[Tooltip("If true, Vector3.zero is passed to the event if no foot transform reference is found.\n" +
		         "Otherwise, the event will not be called if the foot transform is null.")]
		[SerializeField]
		private bool forceFootStepEvents = false;
		
		[SerializeField]
		private Transform rightFoot;
		
		[SerializeField]
		private Transform leftFoot;
		
		public void OnFootRightContact()
		{
			if (!enableFootStepEvents) return;
			
			if (!forceFootStepEvents)
			{
				if (rightFoot.OrNull()) return;
			}
			
			Vector3 rightFootPosition = rightFoot.OrNull() ? rightFoot.position : Vector3.zero;
			onRightFootStep?.Invoke(rightFootPosition);
			onFootStep?.Invoke(rightFootPosition);
		}
	
		public void OnFootLeftContact()
		{
			if (!enableFootStepEvents) return;
			
			if (!forceFootStepEvents)
			{
				if (leftFoot.OrNull()) return;
			}
			
			Vector3 leftFootPosition = leftFoot.OrNull() ? leftFoot.position : Vector3.zero;
			onLeftFootStep?.Invoke(leftFootPosition);
			onFootStep?.Invoke(leftFootPosition);
		}
	}
}