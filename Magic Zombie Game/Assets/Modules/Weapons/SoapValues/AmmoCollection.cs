using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AmmoCollection
{
	public event Action<int> OnAmmoChanged;
	public event Action<int> OnMaxAmmoChanged;
	
	#region Properties

	public int Ammo
	{
		get => ammo;
		set
		{
			if (ammo == value) return;
			ammo = Mathf.Clamp(value, 0, maxAmmo);
			OnAmmoChanged?.Invoke(ammo);
		}
	}

	public int MaxAmmo
	{
		get => maxAmmo;
		set
		{
			if (maxAmmo == value) return;
			maxAmmo = Mathf.Max(1, value);
			OnMaxAmmoChanged?.Invoke(maxAmmo);
			Ammo = Mathf.Clamp(ammo, 0, maxAmmo);
		}
	}

	#endregion

	[SerializeField]
	private int ammo = 0; // ammo in the mag
	
	[SerializeField]
	private int maxAmmo = 30; // max ammo in the mag

	public void ClampValues()
	{
		maxAmmo = Mathf.Max(1, maxAmmo);
		ammo = Mathf.Clamp(ammo, 0, maxAmmo);
	}
}

