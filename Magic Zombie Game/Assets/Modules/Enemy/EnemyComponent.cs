using System;
using Core.HealthSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyComponent : MonoBehaviour
{
	public event Action<EnemyComponent> OnDie;
	
	public int EnemyId => gameObject.GetInstanceID();
	
	public EnemyPathfinding PathfindingComponent => pathfindingComponent;
	
	public HealthComponent HealthComponent => healthComponent;
	
	[SerializeField]
	private EnemyPathfinding pathfindingComponent;
	
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
		OnDie?.Invoke(this);
	}
}