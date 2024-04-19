using Core.HealthSystem;
using Core.Owning;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons
{
	[DisallowMultipleComponent]
	public class GunComponent : MonoBehaviour, IOwnable<GameObject>
	{
		public bool enableGun = true;
		
		public int damage = 10;
		
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private ProjectileHandler projectileHandler;

		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private ShootComponent shootComponent;

		private GameObject _owner;
		private GunController _ownerController;

		public void FireGun()
		{
			if (!enableGun) return;
			
			shootComponent.OrNull()?.StartShoot();
		}

		public void StopFiring()
		{
			if (!enableGun) return;
			
			shootComponent.OrNull()?.StopShoot();
		}

		#region Owner Methods

		public GameObject GetOwner()
		{
			return _owner;
		}

		public void SetOwner(GameObject owner)
		{
			_owner = owner;
		}

		/// <summary>
		/// Gets the gun controller that owns this gun.
		/// </summary>
		/// <returns>The assigned GunController owner.</returns>
		public GunController GetOwnerController()
		{
			return _ownerController;
		}

		/// <summary>
		/// Sets the gun controller that owns this gun.
		/// </summary>
		/// <param name="controller">The GunController owner to assign.</param>
		public void SetOwnerController(GunController controller)
		{
			_ownerController = controller;
		}

		#endregion

		protected virtual void OnGunFired()
		{
			projectileHandler.OrNull()?.FireProjectile(TargetHitCallback);
		}

		#region Unity Methods

		private void Awake()
		{
			if (shootComponent == null)
			{
				shootComponent = this.GetOrAddComponent<ShootComponent>();
				shootComponent.SetOwner(this);
			}

			if (projectileHandler != null)
			{
				projectileHandler.SetOwner(this);
			}
		}

		private void OnEnable()
		{
			shootComponent.OnShoot += OnGunFired;
		}

		private void OnDisable()
		{
			shootComponent.OnShoot -= OnGunFired;
		}

		#endregion
		
		private void TargetHitCallback(HitContext hitContext)
		{
			if (hitContext.hitTarget == null) return;
			if (hitContext.hitTarget.TryGetComponent(out IDamageable damageable))
			{
				damageable.Damage(damage);
			}
		}
	}
}