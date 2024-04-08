using System;
using UnityEngine;
using Obvious.Soap;
using Obvious.Soap.Attributes;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "scriptable_variable_" + nameof(DamageCollection), menuName = "Soap/ScriptableVariables/"+ nameof(DamageCollection))]
public class DamageCollectionVariable : ScriptableVariable<DamageCollection>
{
	#region Events

	public event Action<float> OnDamageChanged
	{
		add => _value.OnDamageChanged += value;
		remove => _value.OnDamageChanged -= value;
	}
	
	public event Action<Vector2> OnNearFarDamageChanged
	{
		add => _value.OnNearFarDamageChanged += value;
		remove => _value.OnNearFarDamageChanged -= value;
	}

	public event Action<Vector2> OnNearFarDistanceChanged
	{
		add => _value.OnNearFarDistanceChanged += value;
		remove => _value.OnNearFarDistanceChanged -= value;
	}
	
	public event Action<float> OnDamageVariationChanged
	{
		add => _value.OnDamageVariationChanged += value;
		remove => _value.OnDamageVariationChanged -= value;
	}

	#endregion

	#region Properties
	
	public bool UsesDamageFalloff => useDamageFalloff;
	
	public bool UsesRandomVariation => randomVariation;

	#endregion
	
	[Header("Damage Settings")]
	[Tooltip("Whether to use different damage values based on distance to the target.")]
	[SerializeField]
	private bool useDamageFalloff = false;
	
	[Tooltip("Whether to add a random variation to the damage.")]
	[SerializeField]
	private bool randomVariation = false;

	public float GetDamage(float distance)
	{
		float falloffDistanceRatio = Mathf.Max(0f, 1 - GetNearFarDistanceRatio(distance));
		float rdm = randomVariation
			? Random.Range(-_value.DamageVariation * 0.5f, _value.DamageVariation) * falloffDistanceRatio
			: 0;
		
		if (useDamageFalloff)
		{
			float falloffDmg = Mathf.Lerp(_value.NearFarDamage.y, _value.NearFarDamage.x,
				GetNearFarDistanceRatio(distance)) + rdm;
			return Mathf.Max(0f, falloffDmg);
		}
		
		float flatDmg = _value.Damage + rdm;
		return Mathf.Max(0f, flatDmg);
	}
	
	public float GetNearFarDistanceRatio(float distance)
	{
		return Mathf.InverseLerp(_value.NearFarDistance.x, _value.NearFarDistance.y, distance);
	}
	
	public float GetNearFarDamageRatio(float damage)
	{
		return Mathf.InverseLerp(_value.NearFarDamage.y, _value.NearFarDamage.x, damage);
	}

	protected override void OnValidate()
	{
		_value.ClampValues();
		base.OnValidate();
	}

	public override void Save()
	{
		PlayerPrefs.SetFloat(Guid + "_dmg", _value.Damage);
		PlayerPrefs.SetFloat(Guid + "_ndmg", _value.NearFarDamage.x);
		PlayerPrefs.SetFloat(Guid + "_fdmg", _value.NearFarDamage.y);
		PlayerPrefs.SetFloat(Guid + "_ndist", _value.NearFarDistance.x);
		PlayerPrefs.SetFloat(Guid + "_fdist", _value.NearFarDistance.y);
		base.Save();
	}

	public override void Load()
	{
		Value.Damage = PlayerPrefs.GetFloat(Guid + "_dmg", DefaultValue.Damage);
		float nearDmg = PlayerPrefs.GetFloat(Guid + "_ndmg", DefaultValue.NearFarDamage.x);
		float farDmg = PlayerPrefs.GetFloat(Guid + "_fdmg", DefaultValue.NearFarDamage.y);
		Value.NearFarDamage = new Vector2(nearDmg, farDmg);
		float nearDist = PlayerPrefs.GetFloat(Guid + "_ndist", DefaultValue.NearFarDistance.x);
		float farDist = PlayerPrefs.GetFloat(Guid + "_fdist", DefaultValue.NearFarDistance.y);
		Value.NearFarDistance = new Vector2(nearDist, farDist);
		Value.DamageVariation = PlayerPrefs.GetFloat(Guid + "_rdmdmg", DefaultValue.DamageVariation);
		base.Load();
	}
}

