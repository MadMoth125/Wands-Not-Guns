using System;
using System.Collections;
using System.Collections.Generic;
using Core.HealthSystem;
using UnityEngine;

public class Weapon
{
	public Weapon(int maxAmmo, float damage)
	{
		_maxAmmo = maxAmmo;
		_ammo = _maxAmmo;
		_damage = damage;
	}
	
	public Weapon(int maxAmmo, int startingAmmo, float damage)
	{
		_maxAmmo = maxAmmo;
		_ammo = Mathf.Clamp(startingAmmo, 0, _maxAmmo);
		_ammo = _maxAmmo;
		_damage = damage;
	}

	// public event Action OnDamageTarget;
	public event Action OnDamageChanged;
	public event Action OnAmmoChanged;
	public event Action OnMaxAmmoChanged;
	public event Action OnAmmoEmpty;

	private float _damage;
	private int _maxAmmo;
	private int _ammo;

	#region Getters and Setters

	/// <summary>
	/// Gets the damage of the weapon.
	/// </summary>
	public float GetDamage()
	{
		return _damage;
	}
	
	/// <summary>
	/// Sets the damage of the weapon.
	/// Damage will not go below 0.
	/// </summary>
	/// <param name="damage">The new damage value.</param>
	public void SetDamage(float damage)
	{
		if (Math.Abs(damage - _damage) < 0.001f) return;
		_damage = Mathf.Max(0f, damage);
		OnDamageChanged?.Invoke();
	}

	/// <summary>
	/// Gets the amount of ammo in the weapon.
	/// </summary>
	public int GetAmmo()
	{
		return _ammo;
	}

	/// <summary>
	/// Gets the maximum amount of ammo the weapon can hold.
	/// </summary>
	public int GetMaxAmmo()
	{
		return _maxAmmo;
	}

	/// <summary>
	/// Gets the normalized amount of ammo in the weapon.
	/// Clamped between 0 and 1 range.
	/// </summary>
	public float GetAmmoNormalized()
	{
		return Mathf.Clamp01((float)_ammo / _maxAmmo);
	}

	/// <summary>
	/// Sets the amount of ammo in the weapon.
	/// Clamped between 0 and the weapon's max ammo.
	/// </summary>
	/// <param name="amount">The new ammo count.</param>
	public void SetAmmo(int amount)
	{
		if (amount == _ammo) return;
		
		_ammo = Mathf.Clamp(amount, 0, _maxAmmo);
		OnAmmoChanged?.Invoke();
		
		if (_ammo == 0)
		{
			OnAmmoEmpty?.Invoke();
		}
	}

	/// <summary>
	/// Sets the maximum amount of ammo the weapon can hold.
	/// </summary>
	/// <param name="amount">The new max ammo count.</param>
	/// <param name="fullAmmo">Whether to fill the weapon's ammo to the max ammo count.</param>
	public void SetMaxAmmo(int amount, bool fullAmmo)
	{
		if (amount == _maxAmmo) return;
		
		_maxAmmo = Mathf.Max(1, amount);
		OnMaxAmmoChanged?.Invoke();
		
		if (fullAmmo)
		{
			SetAmmo(GetMaxAmmo());
		}
		
		if (GetAmmo() > GetMaxAmmo())
		{
			SetAmmo(GetMaxAmmo());
		}
	}

	#endregion

	/// <summary>
	/// Adds ammo to the weapon.
	/// Ammo will be clamped if count is above the max ammo or below 0.
	/// Will not add ammo if the given amount is less than 0.
	/// </summary>
	/// <param name="amount">The amount of ammo to add.</param>
	public void AddAmmo(int amount) => SetAmmo(GetAmmo() + Mathf.Max(0, amount));

	/// <summary>
	/// Removes ammo from the weapon.
	/// Ammo will be clamped if count is below 0.
	/// Will not remove ammo if the given amount is less than 0.
	/// </summary>
	/// <param name="amount">The amount of ammo to remove.</param>
	public void RemoveAmmo(int amount) => SetAmmo(GetAmmo() - Mathf.Max(0, amount));
}

