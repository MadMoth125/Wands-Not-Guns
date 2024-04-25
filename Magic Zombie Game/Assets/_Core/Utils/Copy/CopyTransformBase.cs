using System;
using Core.Utils;
using Core.Utils.Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Utils
{
	/* Exposed items:
	 *
	 * Public variables:
	 * - bool jumpToTarget
	 * - Transform target
	 * - TickType updateGroup
	 * - CopyType copyMethod
	 * - float lerpSharpness
	 */
	
	/// <summary>
	/// A base class for copying the transform of a target object.
	/// Derived classes define the specific transform property to copy (position, rotation, scale, etc.).
	/// </summary>
	public abstract class CopyTransformBase : MonoBehaviour
	{
		[Tooltip("Instantly jump to the target position on Awake")]
		public bool jumpToTarget = true;
		
		[TitleGroup("References")]
		public Transform target;
		
		[TitleGroup("Tick")]
		[EnumToggleButtons]
		public TickType updateGroup = TickType.Update;
		
		[TitleGroup("Method")]
		[EnumToggleButtons]
		public CopyType copyMethod = CopyType.Exact;
		
		[ShowIf("@copyMethod == CopyType.Lerp")]
		public float lerpSharpness = 10f;
		
		/// <summary>
		/// Called in the <see cref="Awake"/> method if <see cref="jumpToTarget"/> is true.
		/// Functionality should include setting the proper transform values to match the target.
		/// </summary>
		protected abstract void JumpToTarget();
		
		/// <summary>
		/// Called in the <see cref="Update"/>, <see cref="LateUpdate"/>, or <see cref="FixedUpdate"/> loop based on the <see cref="updateGroup"/> parameter.
		/// </summary>
		protected abstract void CustomUpdateLoop();
		
		protected float LerpValue(float sharpness, float deltaTime) => 1 - Mathf.Exp(-sharpness * deltaTime);
		
		#region Unity Methods

		protected virtual void Awake()
		{
			if (jumpToTarget) JumpToTarget();
		}

		protected virtual void Update()
		{
			if (updateGroup == TickType.Update) CustomUpdateLoop();
		}

		protected virtual void LateUpdate()
		{
			if (updateGroup == TickType.LateUpdate) CustomUpdateLoop();
		}

		protected virtual void FixedUpdate()
		{
			if (updateGroup == TickType.FixedUpdate) CustomUpdateLoop();
		}

		#endregion
		
	}
}