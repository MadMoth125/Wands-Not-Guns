using System;
using UnityEngine;

namespace Points
{
	/// <summary>
	/// Component that represents a buyable object.
	/// Can be bought by an object that implements <see cref="IGetPointSystem"/>.
	/// </summary>
	public class BuyableComponent : MonoBehaviour, IBuyable
	{
		public event Action OnBuySuccess;
		public event Action OnBuyFail;
		
		[SerializeField]
		private int cost = 10;

		public bool Buy(IGetPointSystem buyer)
		{
			if (buyer.GetPointsSystem().GetPoints() >= cost)
			{
				buyer.GetPointsSystem().RemovePoints(cost);
				OnBuySuccess?.Invoke();
				return true;
			}
			
			OnBuyFail?.Invoke();
			return false;
		}

		public int GetCost()
		{
			return cost;
		}

		#region Unity Methods

		private void OnValidate()
		{
			cost = Mathf.Max(0, cost);
		}

		#endregion
	}
}