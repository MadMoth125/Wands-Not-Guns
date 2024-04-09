using System;
using Core.HealthSystem;
using Obvious.Soap;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
	#region Properties

	public EnemyPathfinding PathfindingComponent => pathfindingComponent;
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
	private HealthComponent healthComponent;

	#region Unity Methods

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