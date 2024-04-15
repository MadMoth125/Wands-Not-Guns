using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoComponent
{
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
	
	private int _ammo;
	private int _maxAmmo;
	
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
	
	// ammo functions
	public int GetAmmo()
	{
		return _ammo;
	}
	
	public void AddAmmo(int amount)
	{
		if (_ammo == _maxAmmo) return;
		
		_ammo += amount;
		
		if (_ammo > _maxAmmo) _ammo = _maxAmmo;
	}
	
	public void RemoveAmmo(int amount)
	{
		if (_ammo == 0) return;
		
		_ammo -= amount;
		
		if (_ammo < 0) _ammo = 0;
	}
	
	public void SetAmmo(int amount)
	{
		_ammo = Mathf.Clamp(amount, 0, _maxAmmo);
	}
	
	public void RefillAmmo()
	{
		_ammo = _maxAmmo;
	}
	
	// max ammo functions
	public int GetMaxAmmo()
	{
		return _maxAmmo;
	}
	
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