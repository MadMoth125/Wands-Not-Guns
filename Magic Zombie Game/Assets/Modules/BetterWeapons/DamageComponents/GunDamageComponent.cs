using Core.Owning;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons
{
	[DisallowMultipleComponent]
	public class GunDamageComponent : MonoBehaviour, IOwnable<GunComponent>
	{
		[Tooltip("If true, adds randomness to the base damage.\n" +
		         "If false, the base damage is always the same.")]
		[SerializeField]
		private bool addRandomness = false;
		
		[Min(0)]
		[SerializeField]
		private float baseDamage = 20f;
		
		[ShowIf(nameof(addRandomness))]
		[Tooltip("The amount of variance to add to the base damage.\n" +
		         "The final damage is calculated as a random value between\n" +
		         "baseDamage - damageVariance and baseDamage + damageVariance.")]
		[Min(0)]
		[SerializeField]
		private float damageVariance = 5f;
		
		private GunComponent _owner;
		
		public float GetDamage()
		{
			if (addRandomness)
			{
				return baseDamage + (Random.Range(-damageVariance * 10f, damageVariance * 10f + 10f) / 10f);
			}
			
			return baseDamage;
		}

		#region Owner Methods

		public GunComponent GetOwner()
		{
			return _owner;
		}

		public void SetOwner(GunComponent owner)
		{
			_owner = owner;
		}

		#endregion
		
		#region Unity Methods

		private void OnValidate()
		{
			baseDamage = Mathf.Max(0, baseDamage);
			damageVariance = Mathf.Max(0, damageVariance);
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
}