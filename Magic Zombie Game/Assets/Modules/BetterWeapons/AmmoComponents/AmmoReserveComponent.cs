using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
	public class AmmoReserveComponent : MonoBehaviour, IGetAmmoComponent
	{
		[SerializeField]
		private int ammo = 0;

		[SerializeField]
		private int maxAmmo = 240;
	
		private int _defaultReserveCount;
		private int _defaultMaxReserveCount;
	
		private AmmoComponent _ammoComponent;

		#region Public Methods

		/// <summary>
		/// Gets the requested amount of ammo from the reserve.
		/// If the requested amount cannot be fulfilled, the remaining ammo is returned.
		/// </summary>
		/// <param name="requestedAmount">The amount of ammo needed from the reserve.</param>
		/// <returns>The amount of ammo that was taken from the reserve.</returns>
		public int GetAmmoFromReserve(int requestedAmount)
		{
			if (_ammoComponent == null) return 0;
			if (requestedAmount <= 0) return 0;
			if (_ammoComponent.Ammo <= 0) return 0;

			int amount = Mathf.Min(requestedAmount, _ammoComponent.Ammo);
			_ammoComponent.RemoveAmmo(amount);
			return amount;
		}
	
		public void ResetToDefault()
		{
			if (_ammoComponent == null) return;
			_ammoComponent.SetMaxAmmo(_defaultMaxReserveCount, false);
			_ammoComponent.SetAmmo(_defaultReserveCount);
		}

		#endregion

		public AmmoComponent GetAmmoComponent()
		{
			return _ammoComponent;
		}

		#region Unity Methods

		private void OnValidate()
		{
			maxAmmo = Mathf.Max(0, maxAmmo);
			ammo = Mathf.Clamp(ammo, 0, maxAmmo);
		}

		private void Awake()
		{
			_ammoComponent = new AmmoComponent(maxAmmo);
		
			if (ammo != 0)
			{
				_ammoComponent.SetAmmo(ammo);
			}
		
			_defaultMaxReserveCount = _ammoComponent.MaxAmmo;
			_defaultReserveCount = _ammoComponent.Ammo;
		}

		#endregion
	}
}