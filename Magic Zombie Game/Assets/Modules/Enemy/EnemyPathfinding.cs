using Core.Owning;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy
{
	public class EnemyPathfinding : MonoBehaviour, IOwnable<EnemyComponent>
	{
		#region Properties

		public Vector3 Destination => Pathfinder.destination;
	
		public bool HasTarget => _hasTarget;
	
		public AIPath Pathfinder => pathfinderComponent;

		#endregion

		[Required]
		[SerializeField]
		private AIPath pathfinderComponent;

		private EnemyComponent _owner;
		private Vector3 _targetPosition;
		private bool _hasTarget;
	
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

		#region Unity Methods

		private void FixedUpdate()
		{
			if (Pathfinder != null && _hasTarget)
			{
				Pathfinder.destination = _targetPosition;
			}
		}

		#endregion
	}
}