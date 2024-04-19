using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using MyCustomControls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Controller
{
	[RequireComponent(typeof(KinematicCharacterMotor))]
	public class MovementController : MonoBehaviour, ICharacterController
	{
		#region Properties

		public CharacterRotation RotationComponent => rotationComponent;
		public CharacterMovement MovementComponent => movementComponent;
		public GravityMovement GravityComponent => gravityComponent;
		
		public CharacterGroundingReport GroundedStatus => _motor != null ? _motor.GroundingStatus : new CharacterGroundingReport();
		public Vector3 Velocity => _motor != null ? _motor.Velocity : Vector3.zero;
		public Vector3 BaseVelocity => _motor != null ? _motor.BaseVelocity : Vector3.zero;
		public Vector3 CharacterUp => _motor != null ? _motor.CharacterUp : Vector3.up;
		public Vector3 CharacterForward => _motor != null ? _motor.CharacterForward : Vector3.forward;
		public Vector3 CharacterRight => _motor != null ? _motor.CharacterRight : Vector3.right;

		#endregion

		#region Fields

		[SerializeField]
		private ScriptableObjectGameControls gameControls;
	
		[Title("Rotation")]
		[Inlined]
		[SerializeField]
		private CharacterRotation rotationComponent = new();
		
		[Title("Movement")]
		
		[Tooltip("Enable or disable the ability to apply impulse forces to the character motor.")]
		[SerializeField]
		private bool enableImpulse = true;
		
		[Inlined]
		[SerializeField]
		private CharacterMovement movementComponent = new();
	
		[Title("Gravity")]
		[Inlined]
		[SerializeField]
		private GravityMovement gravityComponent = new();

		[Title("Collision")]
		[SerializeField]
		private LayerMaskVariable collisionLayerMask;

		#endregion
		
		private IEnumerable<IMovementAbility> _abilityCollection;
		private KinematicCharacterMotor _motor;
		private Vector3 _impact;
		

		/// <summary>
		/// Applies an impulse force to the character motor.
		/// </summary>
		/// <param name="direction">The direction of the impulse force.</param>
		/// <param name="force">The strength of the impulse force.</param>
		public void ApplyImpulse(Vector3 direction, float force)
		{
			if (!enableImpulse) return;
			_impact = direction.normalized * force / _motor.SimulatedCharacterMass;
		}
		
		#region Unity Methods

		private void Awake()
		{
			_motor = GetComponent<KinematicCharacterMotor>();
			_motor.CharacterController = this;

			_abilityCollection = new IMovementAbility[]
			{
				rotationComponent,
				movementComponent,
				gravityComponent,
			};
		
			foreach (var ability in _abilityCollection)
			{
				ability.SetReferences(gameControls, _motor);
			}
		}

		private void OnEnable()
		{
			gameControls.EnableControls();
		
			foreach (var ability in _abilityCollection)
			{
				ability.Enable();
			}
		}

		private void OnDisable()
		{
			gameControls.DisableControls();
		
			foreach (var ability in _abilityCollection)
			{
				ability.Disable();
			}		
		}

		#endregion

		void ICharacterController.UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			foreach (var ability in _abilityCollection)
			{
				ability.UpdateRotation(ref currentRotation, deltaTime);
			}
		}

		void ICharacterController.UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			if (_impact.sqrMagnitude > 0.2f)
			{
				currentVelocity += _impact * deltaTime;
				_impact = Vector3.Lerp(_impact, Vector3.zero, deltaTime * 10f);
			}
			
			foreach (var ability in _abilityCollection)
			{
				ability.UpdateVelocity(ref currentVelocity, deltaTime);
			}
		}

		public bool IsColliderValidForCollisions(Collider coll)
		{
			return collisionLayerMask != null && (collisionLayerMask.Value & (1 << coll.gameObject.layer)) != 0;
		}

		#region Misc ICharacterController Methods

		void ICharacterController.BeforeCharacterUpdate(float deltaTime)
		{
		
		}

		void ICharacterController.PostGroundingUpdate(float deltaTime)
		{
		
		}

		void ICharacterController.AfterCharacterUpdate(float deltaTime)
		{
		
		}

		void ICharacterController.OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		
		}

		void ICharacterController.OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		
		}

		void ICharacterController.ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
			Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		
		}

		void ICharacterController.OnDiscreteCollisionDetected(Collider hitCollider)
		{
		
		}

		#endregion
	}
}