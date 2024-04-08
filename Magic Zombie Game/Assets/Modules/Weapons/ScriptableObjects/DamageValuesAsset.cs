using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

// [CreateAssetMenu(fileName = "DamageValuesAsset", menuName = "Weapons/Damage Values Asset")]
[Obsolete]
public class DamageValuesAsset : ScriptableObject
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
		get => damageValues;
		set
		{
			if (damageValues == value) return;
			damageValues.far = Mathf.Max(0, value.y);
			damageValues.near = Mathf.Max(0, value.x);
			OnNearFarDamageChanged?.Invoke(damageValues);
		}
	}

	public Vector2 NearFarDistance
	{
		get => damageFalloff;
		set
		{
			if (damageFalloff == value) return;
			damageFalloff.far = Mathf.Max(0, value.y);
			damageFalloff.near = Mathf.Clamp(value.x, 0, damageFalloff.far);
			OnNearFarDistanceChanged?.Invoke(damageFalloff);
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
	
	public bool UsesFalloff => useDamageFalloff;
	
	public bool UsesVariation => useDamageVariation;

	#endregion

	#region Fields

	[HideIf(nameof(useDamageFalloff), Animate = false)]
	[SerializeField]
	private float damage = 40f;
	
	[ShowIf(nameof(useDamageFalloff), Animate = false)]
	[Tooltip("The damage values in accordance to the distance from the target.\n\n" +
	         "Near damage (Left) is the damage dealt when damage falloff is weakest.\n" +
	         "Far damage (Right) is the damage dealt when damage falloff is strongest.")]
	[LabelText("Damage")]
	[InlineProperty]
	[SerializeField]
	private NearFarValue damageValues = new NearFarValue(40f, 20f);

	[ShowIf(nameof(useDamageFalloff), Animate = false)]
	[Tooltip("The expected distance from the target to calculate the damage falloff.\n\n" +
	         "Near distance (Left) is the point where the damage falloff begins.\n" +
	         "Far distance (Right) is the point where the damage falloff ends.")]
	[LabelText("Falloff")]
	[InlineProperty]
	[SerializeField]
	private NearFarValue damageFalloff = new NearFarValue(20f, 40f);
	
	[ShowIf(nameof(useDamageVariation))]
	[Tooltip("The amount of variation in the damage dealt.")]
	[SerializeField]
	private float damageVariation = 1f;
	
	[Tooltip("Use damage falloff to change the damage dealt over distance.")]
	[SerializeField]
	private bool useDamageFalloff = false;
	
	[Tooltip("Use damage variation to randomize the damage dealt.")]
	[SerializeField]
	private bool useDamageVariation = false;

	#endregion

	private void OnValidate()
	{
		damage = Mathf.Max(0, damage);
		damageValues.near = Mathf.Max(0f, damageValues.near);
		damageValues.far = Mathf.Max(0, damageValues.far);
		damageFalloff.far = Mathf.Max(0, damageFalloff.far);
		damageFalloff.near = Mathf.Clamp(damageFalloff.near, 0, damageFalloff.far);
		damageVariation = Mathf.Max(0, damageVariation);
	}
}