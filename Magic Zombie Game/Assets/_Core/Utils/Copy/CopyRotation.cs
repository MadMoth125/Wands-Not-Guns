using UnityEngine;
using Core.Utils;

namespace Core.Utils
{
	public class CopyRotation : CopyTransformBase
	{
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
			
			transform.rotation = target.rotation;
		}
		
		private void CopyRotationLerp()
		{
			if (target == null) return;
			
			transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, LerpValue(lerpSharpness, Time.deltaTime));
		}
	}
}
