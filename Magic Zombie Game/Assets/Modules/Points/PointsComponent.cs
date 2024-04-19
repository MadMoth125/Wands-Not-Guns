using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Points
{
	public class PointsComponent : MonoBehaviour, IGetPointSystem
	{
		[SerializeField]
		private int startingPoints = 0;
	
		[SerializeField]
		private bool clampPoints = false;
	
		[EnableIf(nameof(clampPoints))]
		[SerializeField]
		private int minPoints = 0;
	
		[EnableIf(nameof(clampPoints))]
		[SerializeField]
		private int maxPoints = 1000;
	
		private PointSystem _points;
	
		public PointSystem GetPointsSystem()
		{
			return _points;
		}

		#region Unity Methods

		private void OnValidate()
		{
			minPoints = Mathf.Min(minPoints, maxPoints);
			maxPoints = Mathf.Max(minPoints, maxPoints);
		}

		private void Awake()
		{
			if (clampPoints)
			{
				_points = new PointSystem(startingPoints, minPoints, maxPoints);
			}
			else
			{
				_points = new PointSystem(startingPoints);
			}
		}

		#endregion
	}
}