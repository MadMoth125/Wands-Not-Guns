using System;
using UnityEngine;
using Obvious.Soap;

[CreateAssetMenu(fileName = "scriptable_variable_" + nameof(AmmoCollection), menuName = "Soap/ScriptableVariables/" + nameof(AmmoCollection))]
public class AmmoIntVariable : ScriptableVariable<AmmoCollection>
{
	public event Action<int> OnAmmoChanged
	{
		add => _value.OnAmmoChanged += value;
		remove => _value.OnAmmoChanged -= value;
	}
	
	public event Action<int> OnMaxAmmoChanged
	{
		add => _value.OnMaxAmmoChanged += value;
		remove => _value.OnMaxAmmoChanged -= value;
	}
	
	#region Ammo

	public void SetAmmo(int value)
	{
		_value.Ammo = value;
	}

	public void AddAmmo(int value)
	{
		_value.Ammo += value;
	}

	public void SubtractAmmo(int value)
	{
		_value.Ammo -= value;
	}

	public void IncrementAmmo()
	{
		_value.Ammo++;
	}

	public void DecrementAmmo()
	{
		_value.Ammo--;
	}
	
	public void RefillAmmo()
	{
		_value.Ammo = _value.MaxAmmo;
	}

	#endregion

	#region Max Ammo

	public void SetMaxAmmo(int value)
	{
		_value.MaxAmmo = value;
	}

	public void AddMaxAmmo(int value)
	{
		_value.MaxAmmo += value;
	}

	public void SubtractMaxAmmo(int value)
	{
		_value.MaxAmmo -= value;
	}

	public void IncrementMaxAmmo()
	{
		_value.MaxAmmo++;
	}

	public void DecrementMaxAmmo()
	{
		_value.MaxAmmo--;
	}

	#endregion
	
	/// <summary>
	/// Returns the percentage of the value between the minimum and maximum.
	/// </summary>
	public float AmmoRatio => Mathf.InverseLerp(0, _value.MaxAmmo, _value.Ammo);
	
	/// <summary>
	/// Returns whether there is any ammo left.
	/// </summary>
	public bool AmmoEmpty => _value.Ammo == 0;
	
	/// <summary>
	/// Returns whether the ammo is full.
	/// </summary>
	public bool AmmoFull => _value.Ammo == _value.MaxAmmo;

	protected override void OnValidate()
	{
		_value.ClampValues();
		base.OnValidate();
	}

	public override void Save()
	{
		PlayerPrefs.SetInt(Guid + "_a", Value.Ammo);
		PlayerPrefs.SetInt(Guid + "_ma", Value.MaxAmmo);
		base.Save();
	}

	public override void Load()
	{
		Value.Ammo = PlayerPrefs.GetInt(Guid + "_a", DefaultValue.Ammo);
		Value.MaxAmmo = PlayerPrefs.GetInt(Guid + "_ma", DefaultValue.MaxAmmo);
		base.Load();
	}
}

