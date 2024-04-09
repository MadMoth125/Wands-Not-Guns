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

		public CharacterGroundingReport GroundedStatus => _motor != null ? _motor.GroundingStatus : new CharacterGroundingReport();

		public Vector3 Velocity => _motor != null ? _motor.Velocity : Vector3.zero;
	
		public Vector3 BaseVelocity => _motor != null ? _motor.BaseVelocity : Vector3.zero;
	
		public Vector3 CharacterUp => _motor != null ? _motor.CharacterUp : Vector3.up;
	
		public Vector3 CharacterForward => _motor != null ? _motor.CharacterForward : Vector3.forward;
	
		public Vector3 CharacterRight => _motor != null ? _motor.CharacterRight : Vector3.right;

		#endregion
	
		[SerializeField]
		private ScriptableObjectGameControls gameControls;
	
		[Title("Rotation")]
		[Inlined]
		[SerializeField]
		private CharacterRotation rotationComponent = new();
		
		[Title("Movement")]
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
		
		private IEnumerable<IMovementAbility> _abilityCollection;
		private KinematicCharacterMotor _motor;

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

		public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
		{
			foreach (var ability in _abilityCollection)
			{
				ability.UpdateRotation(ref currentRotation, deltaTime);
			}
		}

		public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
		{
			foreach (var ability in _abilityCollection)
			{
				ability.UpdateVelocity(ref currentVelocity, deltaTime);
			}
		}

		#region Misc ICharacterController Methods

		public void BeforeCharacterUpdate(float deltaTime)
		{
		
		}

		public void PostGroundingUpdate(float deltaTime)
		{
		
		}

		public void AfterCharacterUpdate(float deltaTime)
		{
		
		}

		public bool IsColliderValidForCollisions(Collider coll)
		{
			return collisionLayerMask != null && (collisionLayerMask.Value & (1 << coll.gameObject.layer)) != 0;
		}

		public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		
		}

		public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
		{
		
		}

		public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
			Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
		{
		
		}

		public void OnDiscreteCollisionDetected(Collider hitCollider)
		{
		
		}

		#endregion
	}
}