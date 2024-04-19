using System;
using Obvious.Soap.Attributes;
using UnityEngine;

[Serializable]
public class DamageCollection
{
	#region Events

	public event Action<float> OnDamageChanged;
	public event Action<Vector2> OnNearFarDamageChanged;
	public event Action<Vector2> OnNearFarDistanceChanged;
	public event Action<float> OnDamageVariationChanged;

	#endregion

	#region Properties

	public float Damage
	{
		get => damage;
		set
		{
			if (Math.Abs(damage - value) < 0.0001f) return;
			damage = Mathf.Max(0, value);
			OnDamageChanged?.Invoke(damage);
		}
	}

	public Vector2 NearFarDamage
	{
		get => nearFarDamage;
		set
		{
			if (nearFarDamage == value) return;
			nearFarDamage.y = Mathf.Max(0, value.y);
			nearFarDamage.x = Mathf.Max(0, value.x);
			OnNearFarDamageChanged?.Invoke(nearFarDamage);
		}
	}

	public Vector2 NearFarDistance
	{
		get => nearFarDistance;
		set
		{
			if (nearFarDistance == value) return;
			nearFarDistance.y = Mathf.Max(0, value.y);
			nearFarDistance.x = Mathf.Clamp(value.x, 0, nearFarDistance.y);
			OnNearFarDistanceChanged?.Invoke(nearFarDistance);
		}
	}
	
	public float DamageVariation
	{
		get => damageVariation;
		set
		{
			if (Math.Abs(damageVariation - value) < 0.0001f) return;
			damageVariation = Mathf.Max(0, value);
			OnDamageVariationChanged?.Invoke(damageVariation);
		}
	}

	#endregion

	[ShowIf("useDamageFalloff", false)]
	[SerializeField]
	private float damage = 40f;

	[ShowIf("useDamageFalloff", true)]
	[Tooltip("The damage values in accordance to the distance from the target.\n\n" +
	         "Near damage (Left) is the damage dealt when damage falloff is weakest.\n" +
	         "Far damage (Right) is the damage dealt when damage falloff is strongest.")]
	[SerializeField]
	private Vector2 nearFarDamage = new Vector2(40f, 20f);

	[ShowIf("useDamageFalloff", true)]
	[Tooltip("The expected distance from the target to calculate the damage falloff.\n\n" +
	         "Near distance (Left) is the point where the damage falloff begins.\n" +
	         "Far distance (Right) is the point where the damage falloff ends.")]
	[SerializeField]
	private Vector2 nearFarDistance = new Vector2(20f, 40f);
	
	[ShowIf("randomVariation", true)]
	[SerializeField]
	private float damageVariation = 1f;
	
	public void ClampValues()
	{
		damage = Mathf.Max(0, damage);
		nearFarDamage.y = Mathf.Max(0, nearFarDamage.y);
		nearFarDamage.x = Mathf.Max(0, nearFarDamage.x);
		nearFarDistance.y = Mathf.Max(0, nearFarDistance.y);
		nearFarDistance.x = Mathf.Clamp(nearFarDistance.x, 0, nearFarDistance.y);
		damageVariation = Mathf.Max(0, damageVariation);
	}
}

