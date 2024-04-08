using System;
using Core.HealthSystem;
using Obvious.Soap;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
	public int EnemyId => gameObject.GetInstanceID();

	public EnemyPathfinding PathfindingComponent => pathfindingComponent;

	public HealthComponent HealthComponent => healthComponent;

	[TitleGroup("Events","Assets", Alignment = TitleAlignments.Centered)]
	[Required]
	[SerializeField]
	private ScriptableEventInt onDieEventAsset;

	[TitleGroup("References","Components", Alignment = TitleAlignments.Centered)]
	[SerializeField]
	private EnemyPathfinding pathfindingComponent;
	
	[TitleGroup("References")]
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