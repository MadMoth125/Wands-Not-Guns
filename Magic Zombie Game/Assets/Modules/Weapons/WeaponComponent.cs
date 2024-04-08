using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

public class WeaponComponent : MonoBehaviour, IWeaponProperties
{
	protected WeaponData WeaponData => _weaponData;

	[InlineEditor(InlineEditorObjectFieldModes.Boxed, Expanded = true)]
	[SerializeField]
	private WeaponAsset parameters;
	
	private WeaponData _weaponData;
	private WeaponData _defaultWeaponData;
	private Object _owner;

	#region Getters and Setters

	public Object GetOwner()
	{
		return _owner;
	}

	public void SetOwner(Object owner)
	{
		_owner = owner;
	}

	public WeaponData GetWeaponData()
	{
		return _weaponData;
	}

	public void SetWeaponData(WeaponData weaponData)
	{
		_weaponData = weaponData;
	}

	public int GetAmmo()
	{
		return _weaponData.Ammo;
	}

	public int GetMaxAmmo()
	{
		return _weaponData.MaxAmmo;
	}

	public int GetReservedAmmo()
	{
		return _weaponData.ReservedAmmo;
	}

	public void SetAmmo(int amount)
	{
		_weaponData.Ammo = Mathf.Clamp(amount, 0, _weaponData.MaxAmmo);
	}

	public void SetMaxAmmo(int amount, bool fullAmmo)
	{
		_weaponData.MaxAmmo = amount;
		if (fullAmmo) _weaponData.Ammo = _weaponData.MaxAmmo;
	}

	public void SetReservedAmmo(int amount)
	{
		_weaponData.ReservedAmmo = amount;
	}

	public float GetAmmoRatio()
	{
		return _weaponData.Ammo / (float)_weaponData.MaxAmmo;
	}

	public float GetDamage(float distance)
	{
		return _weaponData.GetDamageAtDistance(distance);
	}

	public void SetDamage(float value)
	{
		_weaponData.Damage = Mathf.Max(0f, value);
	}

	public void SetDamage(float nearDamage, float farDamage)
	{
		_weaponData.NearFarDamage = new Vector2(nearDamage, farDamage);
	}

	public void SetFalloffDistances(float nearDistance, float farDistance)
	{
		_weaponData.NearFarDistance = new Vector2(nearDistance, farDistance);
	}

	#endregion

	#region Public Methods

	public void AddAmmo(int amount)
	{
		_weaponData.Ammo += Mathf.Max(0, amount);
	}

	public void RemoveAmmo(int amount)
	{
		_weaponData.Ammo -= Mathf.Max(0, amount);
	}

	public void Reload()
	{
		int magDiff = _weaponData.MaxAmmo - _weaponData.Ammo;
		if (magDiff >= _weaponData.ReservedAmmo)
		{
			AddAmmo(_weaponData.ReservedAmmo);
			_weaponData.ReservedAmmo = 0;
		}
		else
		{
			AddAmmo(magDiff);
			_weaponData.ReservedAmmo -= magDiff;
		}
	}

	public bool CanReload()
	{
		return _weaponData.ReservedAmmo > 0 && _weaponData.Ammo < _weaponData.MaxAmmo;
	}

	public void ResetWeaponData()
	{
		_weaponData = _defaultWeaponData;
	}

	#endregion

	#region Unity Methods

	private void Awake()
	{
		_weaponData = parameters.GetWeaponData();
		_defaultWeaponData = _weaponData;
		AfterAwake();
	}

	protected virtual void AfterAwake()
	{
		
	}
	
	private void Update()
	{
		
	}

	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		
	}

	#endregion
}