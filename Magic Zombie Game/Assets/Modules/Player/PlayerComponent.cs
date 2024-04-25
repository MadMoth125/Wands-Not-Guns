using System;
using System.Collections;
using Core.HealthSystem;
using Obvious.Soap;
using Player.Controller;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Player
{
	[SelectionBase]
	public class PlayerComponent : MonoBehaviour
	{
		#region Properties

		public MovementController ControllerComponent => controllerComponent;
		public HealthComponent HealthComponent => healthComponent;
		public WeaponBackpack WeaponBackpackComponent => weaponBackpackComponent;
		public int PlayerId => gameObject.GetInstanceID();

		#endregion
	
		[ScriptableEventCategory]
		[Required]
		[SerializeField]
		private ScriptableEventInt onDieEventAsset;
		
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private HealthComponent healthComponent;

		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private WeaponBackpack weaponBackpackComponent;

		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private MovementController controllerComponent;

		private bool _isDead;
		
		#region Unity Methods

		private void Awake()
		{
			healthComponent.SetOwner(gameObject);
			weaponBackpackComponent.SetOwner(this);
		}

		private void OnEnable()
		{
			healthComponent.OnDamaged += HandleDamageKnockBack;
			healthComponent.OnHealthChanged += HandleHealthChanged;
			healthComponent.OnDie += HandleDie;
		}

		private void OnDisable()
		{
			healthComponent.OnDamaged -= HandleDamageKnockBack;
			healthComponent.OnHealthChanged -= HandleHealthChanged;
			healthComponent.OnDie -= HandleDie;
		}

		#endregion

		private void HandleDie()
		{
			if (_isDead) return;
			
			onDieEventAsset.Raise(PlayerId);
			_isDead = true;
			
			controllerComponent.DisableRotation();
			controllerComponent.DisableMovement();
			
			// Face the direction of the last damage source
			if (healthComponent.GetLastDamageSource() is Component sourceComponent)
			{
				Vector3 pos = transform.position;
				Vector3 sourcePos = sourceComponent.transform.position;
				Vector3 direction = (sourcePos.SetY(pos.y) - pos).normalized;
				
				controllerComponent.ApplyImpulse(-direction.normalized, 100f);
				controllerComponent.RotationComponent.SetLookDirection(direction.normalized);
			}

			if (weaponBackpackComponent.GetCurrentGun() != null)
			{
				weaponBackpackComponent.UnEquipCurrentGun();
			}
		}

		private void HandleHealthChanged(HealthChangedArgs healthArgs)
		{
			if (healthArgs.PreviousValue == 0f && healthArgs.CurrentValue > 0f)
			{
				controllerComponent.EnableRotation();
				controllerComponent.EnableMovement();
				_isDead = false;
			}
		}

		private void HandleDamageKnockBack(HealthChangedArgs obj, Object source)
		{
			if (_isDead) return;
			
			if (source is Component sourceComponent)
			{
				Vector3 pos = transform.position;
				Vector3 sourcePos = sourceComponent.transform.position;
				Vector3 oppositeDirection = (pos - sourcePos.SetY(pos.y)).normalized;
				
				controllerComponent.ApplyImpulse(oppositeDirection, 100f);
			}
		}
	}
}