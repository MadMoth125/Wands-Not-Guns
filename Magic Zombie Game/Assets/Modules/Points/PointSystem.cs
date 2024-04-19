using System;
using UnityEngine;

namespace Points
{
	public class PointSystem
	{
		public event Action OnPointsChanged;
		public event Action OnMaxPointsReached;
		public event Action OnMinPointsReached;
		
		private int _points = 0;
		private int _minPoints = 0;
		private int _maxPoints = 0;
		private bool _isClamped = false;

		private int PointProperty
		{
			get => _points;
			set
			{
				if (_points == value) return;
				_points = _isClamped ? Mathf.Clamp(value, _minPoints, _maxPoints) : value;

				if (_isClamped)
				{
					if (_points == _minPoints)
					{
						OnMinPointsReached?.Invoke();
					}
			
					if (_points == _maxPoints)
					{
						OnMaxPointsReached?.Invoke();
					}
				}
				else
				{
					if (_points == 0)
					{
						OnMinPointsReached?.Invoke();
					}
				}
				
				OnPointsChanged?.Invoke();
			}
		}

		public PointSystem(int points)
		{
			_isClamped = false;
			_points = points;
			_minPoints = 0;
			_maxPoints = 0;
		}
	
		public PointSystem(int points, int minPoints, int maxPoints)
		{
			_isClamped = true;
			_minPoints = minPoints;
			_maxPoints = maxPoints;
			_points = Mathf.Clamp(points, minPoints, maxPoints);
		}

		public int GetPoints()
		{
			return PointProperty;
		}
		
		public int GetMinPoints()
		{
			return _minPoints;
		}
		
		public int GetMaxPoints()
		{
			return _maxPoints;
		}

		public void AddPoints(int points)
		{
			PointProperty += points;
		}

		public void RemovePoints(int points)
		{
			PointProperty -= points;
		}

		public void SetPoints(int points)
		{
			PointProperty = points;
		}
	
		public void SetMinPoints(int minPoints)
		{
			_minPoints = Mathf.Min(minPoints, _maxPoints);
			PointProperty = Mathf.Max(_points, _minPoints);
		}
	
		public void SetMaxPoints(int maxPoints)
		{
			_maxPoints = Mathf.Max(maxPoints, _minPoints);
			PointProperty = Mathf.Min(_points, _maxPoints);
		}
		
		public void SetClamped(bool isClamped)
		{
			_isClamped = isClamped;
		}
	}
}