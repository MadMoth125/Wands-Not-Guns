using System;
using System.Collections;
using System.Collections.Generic;
using Obvious.Soap;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponAsset", menuName = "Weapons/Weapon Asset")]
public class WeaponAsset : ScriptableObject
{
	#region Fields

	[TabGroup("Damage Settings")]
	[SerializeField]
	private bool useFalloff = false;

	[TabGroup("Damage Settings")]
	[HideIf(nameof(useFalloff), Animate = true)]
	[Tooltip("The damage dealt to the target.")]
	[LabelText("Damage")]
	[SerializeField]
	private float flatDamage = 40f;

	[TabGroup("Damage Settings")]
	[ShowIf(nameof(useFalloff), Animate = true)]
	[Tooltip("The damage values in accordance to the distance from the target.\n\n" +
	         "Near damage (Left) is the damage dealt when damage falloff is weakest.\n" +
	         "Far damage (Right) is the damage dealt when damage falloff is strongest.")]
	[LabelText("Damage Falloff")]
	[InlineProperty]
	[SerializeField]
	private NearFarValue nearFarDamage = new NearFarValue(20f, 5f);

	[TabGroup("Damage Settings")]
	[ShowIf(nameof(useFalloff), Animate = true)]
	[Tooltip("The expected distance from the target to calculate the damage falloff.\n\n" +
	         "Near distance (Left) is the point where the damage falloff begins.\n" +
	         "Far distance (Right) is the point where the damage falloff ends.")]
	[LabelText("Distance Falloff")]
	[InlineProperty]
	[SerializeField]
	private NearFarValue nearFarDistance = new NearFarValue(20f, 60f);

	[TabGroup("Fire Settings")]
	[SerializeField]
	private bool isAutomatic = false;

	[TabGroup("Fire Settings")]
	[SerializeField]
	private float fireInterval = 0.2f;
	
	[TabGroup("Ammo Settings")]
	[Tooltip("Enabling this will have the weapon use a reference to a reserved ammo count.\n" +
	         "This will allow the weapon to use a shared ammo pool rather than its own instance.")]
	[SerializeField]
	private bool useReservedAmmoReference = false;
	
	[TabGroup("Ammo Settings")]
	[PropertySpace(SpaceBefore = 0, SpaceAfter = 20)]
	[Tooltip("By default, the ammo count is set to the max ammo count when constructed.\n" +
	         "Enabling this will allow you to set the starting ammo count to a specific value.")]
	[SerializeField]
	private bool overrideAmmoCount = false;

	[TabGroup("Ammo Settings")]
	[ShowIf(nameof(overrideAmmoCount), Animate = true)]
	[SerializeField]
	private int ammo = 0;

	[TabGroup("Ammo Settings")]
	[SerializeField]
	private int maxAmmo = 30;
	
	[TabGroup("Ammo Settings")]
	[HideIf(nameof(useReservedAmmoReference), Animate = false)]
	[SerializeField]
	private int reservedAmmo = 120;
	
	[TabGroup("Ammo Settings")]
	[ShowIf(nameof(useReservedAmmoReference), Animate = false)]
	[LabelText("Reserved Ammo")]
	[SerializeField]
	private IntVariable reservedAmmoReference;

	[TabGroup("Ammo Settings")]
	[PropertySpace(SpaceBefore = 20)]
	[SerializeField]
	private float reloadDuration = 1.5f;

	#endregion

	/// <summary>
	/// Creates a new weapon data object based on the weapon asset's values.
	/// </summary>
	/// <returns>A new weapon data object.</returns>
	public WeaponData GetWeaponData()
	{
		if (useFalloff)
		{
			if (overrideAmmoCount && useReservedAmmoReference)
			{
				return new WeaponData(
					nearFarDamage,
					nearFarDistance,
					ammo,
					maxAmmo,
					reservedAmmoReference,
					fireInterval,
					reloadDuration,
					isAutomatic);
			}
			
			if (overrideAmmoCount)
			{
				return new WeaponData(
					nearFarDamage,
					nearFarDistance,
					ammo,
					maxAmmo,
					reservedAmmo,
					fireInterval,
					reloadDuration,
					isAutomatic);
			}
			
			if (useReservedAmmoReference)
			{
				return new WeaponData(
					nearFarDamage,
					nearFarDistance,
					maxAmmo,
					reservedAmmoReference,
					fireInterval,
					reloadDuration,
					isAutomatic);
			}
			
			return new WeaponData(
				nearFarDamage, 
				nearFarDistance, 
				maxAmmo,
				reservedAmmo,
				fireInterval,
				reloadDuration,
				isAutomatic);
		} // End if (useFalloff)
		
		if (overrideAmmoCount && useReservedAmmoReference)
		{
			return new WeaponData(
				flatDamage,
				ammo,
				maxAmmo,
				reservedAmmoReference,
				fireInterval,
				reloadDuration,
				isAutomatic);
		}
		
		if (overrideAmmoCount)
		{
			return new WeaponData(
				flatDamage,
				ammo,
				maxAmmo,
				reservedAmmo,
				fireInterval,
				reloadDuration,
				isAutomatic);
		}
			
		if (useReservedAmmoReference)
		{
			return new WeaponData(
				flatDamage,
				maxAmmo,
				reservedAmmoReference,
				fireInterval,
				reloadDuration,
				isAutomatic);
		}
		
		return new WeaponData(
			flatDamage,
			maxAmmo,
			reservedAmmo,
			fireInterval,
			reloadDuration,
			isAutomatic);
	}
	
	
	private void OnValidate()
	{
		flatDamage = Mathf.Max(0, flatDamage);
		nearFarDamage.near = Mathf.Max(0, nearFarDamage.near);
		nearFarDamage.far = Mathf.Max(0, nearFarDamage.far);
		nearFarDistance.far = Mathf.Max(0, nearFarDistance.far);
		nearFarDistance.near = Mathf.Clamp(nearFarDistance.near, 0, nearFarDistance.far);
		fireInterval = Mathf.Max(0.01f, fireInterval);
		maxAmmo = Mathf.Max(1, maxAmmo);
		ammo = Mathf.Clamp(ammo, 0, maxAmmo);
		reloadDuration = Mathf.Max(0f, reloadDuration);
	}
}