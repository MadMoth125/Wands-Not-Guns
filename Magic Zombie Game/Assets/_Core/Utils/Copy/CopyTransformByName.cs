using System.Collections.Generic;
using System.Linq;
using Core.Utils.Shared;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Utils
{
	public class CopyTransformByName : CopyTransformBase
	{
		[TitleGroup("References")]
		[Tooltip("The name of the target object to copy the transform of.")]
		[SerializeField]
		private string targetName;
	
		[Space(20)]
		[EnumToggleButtons]
		public OffsetType positionOffsetType;

		[ShowIf("@positionOffsetType != OffsetType.None")]
		public Vector3 positionOffset;
	
		[Space(20)]
		[EnumToggleButtons]
		public OffsetType rotationOffsetType;
	
		[ShowIf("@rotationOffsetType != OffsetType.None")]
		public Vector3 rotationOffset;
	
		private Transform _cachedTarget;
		private HashSet<Transform> _cachedChildren;
	
		public string GetTargetName()
		{
			return targetName;
		}

		public void SetTargetName(string name)
		{
			targetName = name;
			_cachedTarget = FindTarget();
		}
		
		public Transform GetTarget()
		{
			return _cachedTarget;
		}
	
		#region Unity Methods

		protected override void Awake()
		{
			_cachedTarget = FindTarget();
			base.Awake();
		}

		protected override void JumpToTarget()
		{
			if (copyMethod is CopyType.Exact or CopyType.Lerp)
			{
				CopyPositionExact();
				CopyRotationExact();
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
					CopyRotationExact();
					break;
			
				case CopyType.Lerp:
					CopyPositionLerp();
					CopyRotationLerp();
					break;
			}
		}

		#endregion
	
		private Transform FindTarget()
		{
			if (target == null)
			{
				return null;
			}
			
			if (string.IsNullOrEmpty(targetName) || string.IsNullOrWhiteSpace(targetName))
			{
				return null;
			}

			if (_cachedChildren == null)
			{
				_cachedChildren = new HashSet<Transform>();
				_cachedChildren.UnionWith(target.GetComponentsInChildren<Transform>());
			}
			
			if (_cachedChildren.TryGetValue(_cachedChildren.FirstOrDefault(t => t.name == targetName), out var targetTransform))
			{
				return targetTransform;
			}

			return null;
		}

		#region Position

		private void CopyPositionExact()
		{
			if (_cachedTarget == null) return;
			
			switch (positionOffsetType)
			{
				case OffsetType.None:
					transform.position = _cachedTarget.position;
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
			if (_cachedTarget == null) return;

			switch (positionOffsetType)
			{
				case OffsetType.None:
					transform.position = Vector3.Lerp(transform.position, _cachedTarget.position, LerpValue(lerpSharpness, Time.deltaTime));
					break;
				
				case OffsetType.Relative:
					transform.position = Vector3.Lerp(transform.position, RelativeOffset(), LerpValue(lerpSharpness, Time.deltaTime));
					break;
				
				case OffsetType.Absolute:
					transform.position = Vector3.Lerp(transform.position, AbsoluteOffset(), LerpValue(lerpSharpness, Time.deltaTime));
					break;
			}
		}
		
		private Vector3 AbsoluteOffset() => _cachedTarget.position + positionOffset;

		private Vector3 RelativeOffset() => _cachedTarget.position + _cachedTarget.TransformDirection(positionOffset);

		#endregion

		#region Rotation

		private void CopyRotationExact()
		{
			if (_cachedTarget == null) return;

			switch (rotationOffsetType)
			{
				case OffsetType.None:
					transform.rotation = _cachedTarget.rotation;
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
			if (_cachedTarget == null) return;

			switch (rotationOffsetType)
			{
				case OffsetType.None:
					transform.rotation = Quaternion.Lerp(transform.rotation, _cachedTarget.rotation, LerpValue(lerpSharpness, Time.deltaTime));
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
	
		private Quaternion RelativeRotOffset() => _cachedTarget.rotation * Quaternion.Euler(rotationOffset);

		#endregion
	}
}