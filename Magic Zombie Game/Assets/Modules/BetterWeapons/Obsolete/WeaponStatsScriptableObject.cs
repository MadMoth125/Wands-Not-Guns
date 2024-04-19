using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using StatSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "GunStats", menuName = "ScriptableObject/GunStats")]
public class WeaponStatsScriptableObject : ScriptableObject
{
	// private GunData gunData = new GunData(10f, 100, 10, 1f);
}

[Serializable]
public class GunData
{
	private readonly Stat _damage;
	private readonly Stat _maxAmmo;
	private readonly Stat _fireRate;
	private readonly Stat _reloadSpeed;
	private bool _isAutomatic;
	
	public GunData(float damage, int maxAmmo, int fireRate, float reloadSpeed, bool isAutomatic)
	{
		_damage = new Stat(damage);
		_maxAmmo = new Stat(maxAmmo);
		_fireRate = new Stat(fireRate);
		_reloadSpeed = new Stat(reloadSpeed);
		_isAutomatic = isAutomatic;
	}
	
	public class Builder
	{
		private float _damage = 20f;
		private int _maxAmmo = 30;
		private int _fireRate = 10;
		private float _reloadSpeed = 2f;
		private bool _isAutomatic = false;
		
		public Builder SetDamage(float damage)
		{
			_damage = damage;
			return this;
		}
		
		public Builder SetMaxAmmo(int maxAmmo)
		{
			_maxAmmo = maxAmmo;
			return this;
		}
		
		public Builder SetFireRate(int fireRate)
		{
			_fireRate = fireRate;
			return this;
		}
		
		public Builder SetReloadSpeed(float reloadSpeed)
		{
			_reloadSpeed = reloadSpeed;
			return this;
		}
		
		public Builder SetIsAutomatic()
		{
			_isAutomatic = true;
			return this;
		}
		
		public GunData Build()
		{
			return new GunData(_damage, _maxAmmo, _fireRate, _reloadSpeed, _isAutomatic);
		}
	}
}