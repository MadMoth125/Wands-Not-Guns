using System;
using CodeMonkey.HealthSystemCM;
using Core.CustomTickSystem;
using Core.HealthSystem;
using Core.Owning;
using Mr_sB.UnityTimer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
	public class EnemyDamageTargetDetector : ProximityHandler, IOwnable<EnemyComponent>
	{
		#region Events

		public event Action<IDamageable> OnDamageTarget;
		public event Action<GameObject> OnTargetDetected;
		public event Action<GameObject> OnTargetLost;

		#endregion
		
		[TitleGroup("Attack Settings")]
		[SerializeField] 
		private float attackCooldown = 1f;
		
		[TitleGroup("Attack Settings")]
		[SerializeField]
		private float damage = 50f;
		
		private bool _canAttack = true;
		private DelayTimer _attackCooldownTimer;
		private EnemyComponent _owner;
		
		#region Owning Methods

		public EnemyComponent GetOwner()
		{
			return _owner;
		}

		public void SetOwner(EnemyComponent owner)
		{
			_owner = owner;
		}

		#endregion
		
		protected override void OnOverlapDetected(GameObject target)
		{
			// Check if the target is has a health component
			var damageable = target.GetComponent<IHealthProperties>();
			if (damageable != null)
			{
				// Check if the target is dead
				if (damageable.IsDead()) return;
				
				OnTargetDetected?.Invoke(target);
				
				// handling attack cooldown
				if (_attackCooldownTimer == null)
				{
					_attackCooldownTimer = Timer.DelayAction(attackCooldown, ResetAttackCooldown);
				}
				else if (_attackCooldownTimer.isDone)
				{
					_attackCooldownTimer.Restart();
				}
				
				// Attack the target
				if (_canAttack)
				{
					_canAttack = false;
					damageable.Damage(damage, _owner);
					OnDamageTarget?.Invoke(damageable);
				}
			}

			return;
			
			void ResetAttackCooldown()
			{
				_canAttack = true;
			}
		}

		protected override void OnOverlapLost(GameObject target)
		{
			if (target.GetComponent<IHealthProperties>() != null)
			{
				OnTargetLost?.Invoke(target);
			}
		}
		
		#region Unity Methods

		private void OnEnable()
		{
			TickSystem.AddListener("ProxCheck", CheckProximity);
		}

		private void OnDisable()
		{
			TickSystem.RemoveListener("ProxCheck", CheckProximity);
		}

		#endregion
	}
}