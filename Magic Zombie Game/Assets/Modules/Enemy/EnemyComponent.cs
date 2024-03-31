using System;
using Core.HealthSystem;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
	public static event Action<EnemyComponent> OnDie;
	
	public EnemyTarget TargetComponent => targetComponent;
	
	public HealthComponent HealthComponent => healthComponent;
	
	[SerializeField]
	private EnemyTarget targetComponent;
	
	[SerializeField]
	private HealthComponent healthComponent;
	
	public void SetTarget(Transform target)
	{
		if (targetComponent == null) return;
		targetComponent.target = target;
	}

	public void SetTargetPosition(Vector3 position)
	{
		if (targetComponent == null) return;
		targetComponent.aiPath.destination = position;
	}

	public void ClearTarget()
	{
		if (targetComponent == null) return;
		targetComponent.target = null;
	}

	#region Unity Methods

	private void Start()
	{
		
	}

	private void Update()
	{
		
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
		Debug.Log($"{gameObject.name} died");
		OnDie?.Invoke(this);
	}
}