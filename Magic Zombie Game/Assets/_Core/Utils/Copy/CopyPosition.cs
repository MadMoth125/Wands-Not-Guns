using UnityEngine;
using Core.Utils;
using Core.Utils.Shared;
using Sirenix.OdinInspector;

namespace Core.Utils
{
	public class CopyPosition : CopyTransformBase
	{
		[Space(20)]
		[EnumToggleButtons]
		public OffsetType positionOffsetType;

		[ShowIf("@positionOffsetType != OffsetType.None")]
		public Vector3 positionOffset;
		
		protected override void JumpToTarget()
		{
			if (copyMethod is CopyType.Exact or CopyType.Lerp)
			{
				CopyPositionExact();
			}
		}
		
		protected override void CustomUpdateLoop()
		{
			switch (copyMethod)
			{
				default:
				case CopyType.None:
					break;
				
				case CopyType.Exact:
					CopyPositionExact();
					break;
				
				case CopyType.Lerp:
					CopyPositionLerp();
					break;
			}
		}

		private void CopyPositionExact()
		{
			if (target == null) return;
			
			switch (positionOffsetType)
			{
				case OffsetType.None:
					transform.position = target.position;
					break;
				
				case OffsetType.Relative:
					transform.position = RelativeOffset();
					break;
				
				case OffsetType.Absolute:
					transform.position = AbsoluteOffset();
					break;
			}
		}
		
		private void CopyPositionLerp()
		{
			if (target == null) return;

			switch (positionOffsetType)
			{
				case OffsetType.None:
					transform.position = Vector3.Lerp(transform.position, target.position, LerpValue(lerpSharpness, Time.deltaTime));
					break;
				
				case OffsetType.Relative:
					transform.position = Vector3.Lerp(transform.position, RelativeOffset(), LerpValue(lerpSharpness, Time.deltaTime));
					break;
				
				case OffsetType.Absolute:
					transform.position = Vector3.Lerp(transform.position, AbsoluteOffset(), LerpValue(lerpSharpness, Time.deltaTime));
					break;
			}
		}
		
		private Vector3 AbsoluteOffset() => target.position + positionOffset;

		private Vector3 RelativeOffset() => target.position + target.TransformDirection(positionOffset);
	}
}
