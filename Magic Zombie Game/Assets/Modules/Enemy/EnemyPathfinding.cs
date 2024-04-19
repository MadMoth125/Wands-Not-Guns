using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyPathfinding : MonoBehaviour
{
	#region Properties

	public Vector3 Destination => Pathfinder.destination;
	
	public bool HasTarget => _hasTarget;
	
	public AIPath Pathfinder => pathfinderComponent;

	#endregion

	[Required]
	[SerializeField]
	private AIPath pathfinderComponent;

	private Vector3 _targetPosition;
	private bool _hasTarget;
	
	public void SetTarget(Transform target)
	{
		if (Pathfinder == null) return;
		SetTargetPosition(target.position);
	}

	public void SetTargetPosition(Vector3 position)
	{
		if (Pathfinder == null) return;
		
		_targetPosition = position;
		_hasTarget = true;
	}

	public void ClearTarget()
	{
		if (Pathfinder == null) return;
		
		Pathfinder.destination = transform.position;
		_hasTarget = false;
	}
	
	private void FixedUpdate()
	{
		if (Pathfinder != null && _hasTarget)
		{
			Pathfinder.destination = _targetPosition;
		}
	}
}
