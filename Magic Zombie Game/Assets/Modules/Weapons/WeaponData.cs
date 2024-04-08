using System;
using Obvious.Soap;
using UnityEngine;

[Serializable]
public struct WeaponData
{
	#region Constructors

	public WeaponData(WeaponData weaponData)
	{
		_damage = weaponData.Damage;
		_nearFarDamage = weaponData.NearFarDamage;
		_nearFarDistance = weaponData.NearFarDistance;
		_ammo = weaponData.Ammo;
		_maxAmmo = weaponData.MaxAmmo;
		_reservedAmmo = weaponData.ReservedAmmo;
		_reservedAmmoReference = weaponData._reservedAmmoReference;
		_fireInterval = weaponData.FireInterval;
		_reloadDuration = weaponData.ReloadDuration;
		_isAutomatic = weaponData.IsAutomatic;
		_usesDamageFalloff = weaponData.UsesDamageFalloff;

		ClampValues();
	}

	// constructor for flat damage weapons
	public WeaponData(
		float damage,
		int maxAmmo,
		int reservedAmmo,
		float fireInterval,
		float reloadDuration,
		bool isAutomatic)
	{
		_damage = damage;
		_ammo = maxAmmo;
		_maxAmmo = maxAmmo;
		_reservedAmmo = reservedAmmo;
		_fireInterval = fireInterval;
		_reloadDuration = reloadDuration;
		
		_isAutomatic = isAutomatic;
		_usesDamageFalloff = false;
		_nearFarDamage = Vector2.zero;
		_nearFarDistance = Vector2.zero;
		_reservedAmmoReference = null;
		
		ClampValues();
	}
	
	public WeaponData(
		float damage,
		int maxAmmo,
		IntVariable reservedAmmo,
		float fireInterval,
		float reloadDuration,
		bool isAutomatic)
	{
		_damage = damage;
		_ammo = maxAmmo;
		_maxAmmo = maxAmmo;
		_reservedAmmoReference = reservedAmmo;
		_fireInterval = fireInterval;
		_reloadDuration = reloadDuration;
		
		_isAutomatic = isAutomatic;
		_usesDamageFalloff = false;
		_nearFarDamage = Vector2.zero;
		_nearFarDistance = Vector2.zero;
		_reservedAmmo = 0;
		
		ClampValues();
	}

	public WeaponData(
		float damage,
		int startAmmo,
		int maxAmmo,
		int reservedAmmo,
		float fireInterval,
		float reloadDuration,
		bool isAutomatic)
	{
		_damage = damage;
		_ammo = startAmmo;
		_maxAmmo = maxAmmo;
		_reservedAmmo = reservedAmmo;
		_fireInterval = fireInterval;
		_reloadDuration = reloadDuration;
		
		_isAutomatic = isAutomatic;
		_usesDamageFalloff = false;
		_nearFarDamage = Vector2.zero;
		_nearFarDistance = Vector2.zero;
		_reservedAmmoReference = null;
		
		ClampValues();
	}
	
	public WeaponData(
		float damage,
		int startAmmo,
		int maxAmmo,
		IntVariable reservedAmmo,
		float fireInterval,
		float reloadDuration,
		bool isAutomatic)
	{
		_damage = damage;
		_ammo = startAmmo;
		_maxAmmo = maxAmmo;
		_reservedAmmoReference = reservedAmmo;
		_fireInterval = fireInterval;
		_reloadDuration = reloadDuration;
		
		_isAutomatic = isAutomatic;
		_usesDamageFalloff = false;
		_nearFarDamage = Vector2.zero;
		_nearFarDistance = Vector2.zero;
		_reservedAmmo = 0;
		
		ClampValues();
	}

	// constructor for damage falloff weapons
	public WeaponData(
		Vector2 nearFarDamage,
		Vector2 nearFarDistance,
		int maxAmmo,
		int reservedAmmo,
		float fireInterval,
		float reloadDuration,
		bool isAutomatic)
	{
		_nearFarDamage = nearFarDamage;
		_nearFarDistance = nearFarDistance;
		_ammo = maxAmmo;
		_maxAmmo = maxAmmo;
		_reservedAmmo = reservedAmmo;
		_fireInterval = fireInterval;
		_reloadDuration = reloadDuration;
		
		_isAutomatic = isAutomatic;
		_usesDamageFalloff = true;
		_damage = nearFarDamage.x;
		_reservedAmmoReference = null;
		
		ClampValues();
	}
	
	public WeaponData(
		Vector2 nearFarDamage,
		Vector2 nearFarDistance,
		int maxAmmo,
		IntVariable reservedAmmo,
		float fireInterval,
		float reloadDuration,
		bool isAutomatic)
	{
		_nearFarDamage = nearFarDamage;
		_nearFarDistance = nearFarDistance;
		_ammo = maxAmmo;
		_maxAmmo = maxAmmo;
		_reservedAmmoReference = reservedAmmo;
		_fireInterval = fireInterval;
		_reloadDuration = reloadDuration;
		
		_isAutomatic = isAutomatic;
		_usesDamageFalloff = true;
		_damage = nearFarDamage.x;
		_reservedAmmo = 0;
		
		ClampValues();
	}

	public WeaponData(
		Vector2 nearFarDamage,
		Vector2 nearFarDistance,
		int startAmmo,
		int maxAmmo,
		int reservedAmmo,
		float fireInterval,
		float reloadDuration,
		bool isAutomatic)
	{
		_nearFarDamage = nearFarDamage;
		_nearFarDistance = nearFarDistance;
		_ammo = startAmmo;
		_maxAmmo = maxAmmo;
		_reservedAmmo = reservedAmmo;
		_fireInterval = fireInterval;
		_reloadDuration = reloadDuration;
		
		_isAutomatic = isAutomatic;
		_usesDamageFalloff = true;
		_damage = nearFarDamage.x;
		_reservedAmmoReference = null;
		
		ClampValues();
	}
	
	public WeaponData(
		Vector2 nearFarDamage,
		Vector2 nearFarDistance,
		int startAmmo,
		int maxAmmo,
		IntVariable reservedAmmo,
		float fireInterval,
		float reloadDuration,
		bool isAutomatic)
	{
		_nearFarDamage = nearFarDamage;
		_nearFarDistance = nearFarDistance;
		_ammo = startAmmo;
		_maxAmmo = maxAmmo;
		_reservedAmmoReference = reservedAmmo;
		_fireInterval = fireInterval;
		_reloadDuration = reloadDuration;
		
		_isAutomatic = isAutomatic;
		_usesDamageFalloff = true;
		_damage = nearFarDamage.x;
		_reservedAmmo = 0;
		
		ClampValues();
	}

	#endregion

	#region Properties

	/// <summary>
	/// Whether the weapon is automatic.
	/// </summary>
	public bool IsAutomatic => _isAutomatic;
	
	/// <summary>
	/// Whether the weapon uses damage falloff.
	/// </summary>
	public bool UsesDamageFalloff => _usesDamageFalloff;
	
	/// <summary>
	/// The flat damage of the weapon.
	/// </summary>
	public float Damage
	{
		get => _damage;
		set => _damage = Mathf.Max(0, value);
	}
	
	/// <summary>
	/// The near and far damage falloff values.
	/// </summary>
	public Vector2 NearFarDamage
	{
		get => _nearFarDamage;
		set
		{
			_nearFarDamage.x = Mathf.Max(0, value.x);
			_nearFarDamage.y = Mathf.Max(0, value.y);
		}
	}
	
	/// <summary>
	/// The near and far damage falloff distances.
	/// </summary>
	public Vector2 NearFarDistance
	{
		get => _nearFarDistance;
		set
		{
			_nearFarDistance.x = Mathf.Max(0f, value.x);
			_nearFarDistance.y = Mathf.Max(0, value.y);
		}
	}
	
	/// <summary>
	/// The ammo in the weapon.
	/// </summary>
	public int Ammo
	{
		get => _ammo;
		set => _ammo = Mathf.Clamp(value, 0, MaxAmmo);
	}

	/// <summary>
	/// The max ammo capacity of the weapon.
	/// </summary>
	public int MaxAmmo
	{
		get => _maxAmmo;
		set
		{
			_maxAmmo = Mathf.Max(1, value);
			_ammo = Mathf.Clamp(_ammo, 0, _maxAmmo);
		}
	}

	/// <summary>
	/// The total ammo for the weapon.
	/// Depending on the constructor used, this value may be a reference to an <see cref="IntVariable"/>.
	/// </summary>
	public int ReservedAmmo
	{
		get => _reservedAmmoReference != null ? _reservedAmmoReference.Value : _reservedAmmo;
		set
		{
			if (_reservedAmmoReference != null)
			{
				_reservedAmmoReference.Value = Mathf.Max(0, value);
			}
			
			_reservedAmmo = Mathf.Max(0, value);
		}
	}

	/// <summary>
	/// The interval between shots.
	/// </summary>
	public float FireInterval
	{
		get => _fireInterval;
		set => _fireInterval = Mathf.Max(0f, value);
	}

	/// <summary>
	/// The time it takes to reload the weapon.
	/// </summary>
	public float ReloadDuration
	{
		get => _reloadDuration;
		set => _reloadDuration = Mathf.Max(0f, value);
	}

	#endregion

	private readonly bool _isAutomatic; // is the weapon automatic
	private readonly bool _usesDamageFalloff; // does the weapon have damage falloff
	private float _damage; // flat damage value
	private Vector2 _nearFarDamage; // near and far damage falloff values
	private Vector2 _nearFarDistance; // near and far damage falloff distances
	private int _ammo; // ammo in clip
	private int _maxAmmo; // max ammo in clip
	private int _reservedAmmo; // total ammo for weapon
	private IntVariable _reservedAmmoReference; // total ammo for weapon reference
	private float _fireInterval; // time between shots
	private float _reloadDuration; // time to reload

	/// <summary>
	/// Calculates the damage for a given distance based on the weapon's damage falloff.
	/// If the weapon does not use damage falloff, the flat damage value will be returned regardless of distance.
	/// </summary>
	/// <param name="distance">The distance from the target.</param>
	public float GetDamageAtDistance(float distance)
	{
		if (_usesDamageFalloff)
		{
			return Mathf.Lerp(NearFarDamage.x, NearFarDamage.y, GetDistanceRatio(distance));
		}
		
		return _damage;
	}
	
	/// <summary>
	/// Calculates the ratio of the given distance in relation to the <see cref="NearFarDistance"/> value.
	/// </summary>
	/// <param name="distance">The distance to calculate the ratio for.</param>
	/// <returns>A value between 0 and 1 representing the distance.</returns>
	public float GetDistanceRatio(float distance)
	{
		// https://docs.unity3d.com/ScriptReference/Mathf.InverseLerp.html
		return Mathf.InverseLerp(NearFarDistance.x, NearFarDistance.y, distance);
	}

	private void ClampValues()
	{
		Damage = _damage;
		NearFarDamage = _nearFarDamage;
		NearFarDistance = _nearFarDistance;
		Ammo = _ammo;
		MaxAmmo = _maxAmmo;
		ReservedAmmo = _reservedAmmo;
		FireInterval = _fireInterval;
		ReloadDuration = _reloadDuration;
	}
}