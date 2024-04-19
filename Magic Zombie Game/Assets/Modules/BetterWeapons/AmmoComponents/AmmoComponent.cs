using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
	public class AmmoComponent
	{
		#region Properties

		public int Ammo
		{
			get => _ammo;
			set => SetAmmo(value);
		}

		public int MaxAmmo
		{
			get => _maxAmmo;
			set => SetMaxAmmo(value, false);
		}

		#endregion
	
		private int _ammo;
		private int _maxAmmo;

		#region Constructors

		public AmmoComponent(int ammo, int maxAmmo)
		{
			_maxAmmo = Mathf.Max(1, maxAmmo);
			_ammo = Mathf.Clamp(ammo, 0, maxAmmo);
		}
	
		public AmmoComponent(int maxAmmo)
		{
			_maxAmmo = Mathf.Max(1, maxAmmo);
			_ammo = _maxAmmo;
		}

		#endregion
	
		/// <summary>
		/// Set the ammo to the max ammo.
		/// </summary>
		public void RefillAmmo()
		{
			_ammo = _maxAmmo;
		}
		
		// ammo functions
		
		/// <summary>
		/// Get the current ammo.
		/// </summary>
		public int GetAmmo()
		{
			return _ammo;
		}
	
		/// <summary>
		/// Add ammo to the current ammo.
		/// Ammo will be clamped between 0 and the max ammo.
		/// </summary>
		/// <param name="amount">The amount of ammo to add. Values less than 0 will be ignored.</param>
		public void AddAmmo(int amount)
		{
			if (_ammo == _maxAmmo) return;

			_ammo += Mathf.Max(0, amount);
		
			if (_ammo > _maxAmmo) _ammo = _maxAmmo;
		}
	
		/// <summary>
		/// Remove ammo from the current ammo.
		/// Ammo will be clamped between 0 and the max ammo.
		/// </summary>
		/// <param name="amount">The amount of ammo to remove. Values less than 0 will be ignored.</param>
		public void RemoveAmmo(int amount)
		{
			if (_ammo == 0) return;
		
			_ammo -= Mathf.Max(0, amount);
		
			if (_ammo < 0) _ammo = 0;
		}
	
		/// <summary>
		/// Set the ammo to a specific amount.
		/// Ammo will be clamped between 0 and the max ammo.
		/// </summary>
		/// <param name="amount">The amount of ammo to set.</param>
		public void SetAmmo(int amount)
		{
			if (_ammo == amount) return;
			_ammo = Mathf.Clamp(amount, 0, _maxAmmo);
		}
	
		// max ammo functions
		
		/// <summary>
		/// Get the max ammo.
		/// </summary>
		public int GetMaxAmmo()
		{
			return _maxAmmo;
		}
	
		/// <summary>
		/// Set the max ammo to a specific amount.
		/// Max ammo will be clamped to a minimum of 1.
		/// Ammo will be clamped between 0 and the max ammo.
		/// </summary>
		/// <param name="amount">The amount of max ammo to set.</param>
		/// <param name="fullAmmo">Whether to refill the ammo to the max ammo.</param>
		public void SetMaxAmmo(int amount, bool fullAmmo)
		{
			_maxAmmo = Mathf.Max(1, amount);
			if (fullAmmo)
			{
				RefillAmmo();
			}
			else
			{
				_ammo = Mathf.Min(_ammo, _maxAmmo);
			}
		}
	}
}