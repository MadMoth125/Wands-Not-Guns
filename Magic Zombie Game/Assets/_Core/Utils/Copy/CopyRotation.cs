using UnityEngine;
using Core.Utils;
using Core.Utils.Shared;
using Sirenix.OdinInspector;

namespace Core.Utils
{
	public class CopyRotation : CopyTransformBase
	{
		[Space(20)]
		[EnumToggleButtons]
		public OffsetType rotationOffsetType;
	
		[ShowIf("@rotationOffsetType != OffsetType.None")]
		public Vector3 rotationOffset;
		
		protected override void JumpToTarget()
		{
			if (target == null) return;
			
			transform.rotation = target.rotation;
		}

		protected override void CustomUpdateLoop()
		{
			switch (copyMethod)
			{
				default:
				case CopyType.None:
					break;
				
				case CopyType.Exact:
					CopyRotationExact();
					break;
				
				case CopyType.Lerp:
					CopyRotationLerp();
					break;
			}
		}
		
		private void CopyRotationExact()
		{
			if (target == null) return;

			switch (rotationOffsetType)
			{
				case OffsetType.None:
					transform.rotation = target.rotation;
					break;
			
				case OffsetType.Relative:
					transform.rotation = RelativeRotOffset();
					break;
			
				case OffsetType.Absolute:
					transform.rotation = AbsoluteRotOffset();
					break;
			}
		}
		
		private void CopyRotationLerp()
		{
			if (target == null) return;

			switch (rotationOffsetType)
			{
				case OffsetType.None:
					transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, LerpValue(lerpSharpness, Time.deltaTime));
					break;
			
				case OffsetType.Relative:
					transform.rotation = Quaternion.Lerp(transform.rotation, RelativeRotOffset(), LerpValue(lerpSharpness, Time.deltaTime));
					break;
			
				case OffsetType.Absolute:
					transform.rotation = Quaternion.Lerp(transform.rotation, AbsoluteRotOffset(), LerpValue(lerpSharpness, Time.deltaTime));
					break;
			}
		}
	
		private Quaternion AbsoluteRotOffset() => Quaternion.Euler(rotationOffset);
	
		private Quaternion RelativeRotOffset() => target.rotation * Quaternion.Euler(rotationOffset);
	}
}
