using System;
using Core.HealthSystem;
using Core.Owning;
using Obvious.Soap;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
	[SelectionBase]
	public class EnemyComponent : MonoBehaviour
	{
		#region Properties

		public EnemyPathfinding PathfindingComponent => pathfindingComponent;
		public EnemyDamageTargetDetector DamageComponent => damageComponent;
		public HealthComponent HealthComponent => healthComponent;
		public int EnemyId => gameObject.GetInstanceID();

		#endregion

		[ScriptableEventCategory]
		[Required]
		[SerializeField]
		private ScriptableEventInt onDieEventAsset;

		[ExternalComponentCategory]
		[SerializeField]
		private EnemyPathfinding pathfindingComponent;
		
		[ExternalComponentCategory]
		[SerializeField]
		private EnemyDamageTargetDetector damageComponent;
	
		[ExternalComponentCategory]
		[SerializeField]
		private HealthComponent healthComponent;

		#region Unity Methods

		private void Awake()
		{
			pathfindingComponent.SetOwner(this);
			damageComponent.SetOwner(this);
			healthComponent.SetOwner(gameObject);
		}

		private void OnEnable()
		{
			healthComponent.OnDie += HandleDie;
		}

		private void OnDisable()
		{
			healthComponent.OnDie -= HandleDie;
		}

		#endregion

		private void HandleDie()
		{
			onDieEventAsset.Raise(EnemyId);
		}
	}
}