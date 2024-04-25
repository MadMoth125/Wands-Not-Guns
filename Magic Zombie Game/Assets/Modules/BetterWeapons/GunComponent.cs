using Core.HealthSystem;
using Core.Owning;
using Core.Utils;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons
{
	[DisallowMultipleComponent]
	public class GunComponent : MonoBehaviour, IOwnable<GameObject>
	{
		#region Properties

		public Rigidbody Rigidbody => rb;
		public ProjectileHandler ProjectileHandler => projectileHandler;
		public GunFireComponent GunFireComponent => gunFireComponent;
		public GunDamageComponent GunDamageComponent => gunDamageComponent;

		#endregion

		#region Fields

		public bool enableGun = true;

		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private ProjectileHandler projectileHandler;

		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private GunFireComponent gunFireComponent;
		
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private GunDamageComponent gunDamageComponent;
		
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private CopyPosition copyPosition;
		
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private CopyRotation copyRotation;

		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private CopyTransformByName weaponCopyTransform;
		
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private Rigidbody rb;

		#endregion
		
		private GameObject _owner;
		private GunController _ownerController;

		public void FireGun()
		{
			if (!enableGun) return;
			
			gunFireComponent.OrNull()?.StartShoot();
		}

		public void StopFiring()
		{
			if (!enableGun) return;
			
			gunFireComponent.OrNull()?.StopShoot();
		}

		#region Owner Methods

		/// <summary>
		/// Gets the owner of this gun.
		/// </summary>
		public GameObject GetOwner()
		{
			return _owner;
		}

		/// <summary>
		/// Sets the new owner of this gun.
		/// </summary>
		/// <param name="owner"></param>
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

		/// <summary>
		/// Sets the weapon's base transform to copy the position and rotation of the target.
		/// </summary>
		/// <param name="target">The target transform to copy the position and rotation of.</param>
		public void SetWeaponRootCopyTarget(Transform target)
		{
			copyPosition.target = target;
			copyRotation.target = target;
		}
		
		/// <summary>
		/// Sets the weapon's visual to copy the position and rotation of the target.
		/// Meant to be used to attach to the body part that will hold the weapon. (e.g. Hand_R, Hand_L, etc.)
		/// </summary>
		/// <param name="rootTarget">The target transform to search the children of.</param>
		/// <param name="targetName">The name of the child transform to copy the position and rotation of.</param>
		public void SetWeaponVisualsCopyTarget(Transform rootTarget, string targetName)
		{
			weaponCopyTransform.target = rootTarget;
			weaponCopyTransform.SetTargetName(targetName);
		}
		
		/// <summary>
		/// Sets the weapon's visual to copy the position and rotation of the target.
		/// Uses the weapon's already defined target name to find and attach to the body part that will hold the weapon. (e.g. Hand_R, Hand_L, etc.)
		/// </summary>
		/// <param name="rootTarget">The target transform to search the children of.</param>
		public void SetWeaponVisualsCopyTarget(Transform rootTarget)
		{
			weaponCopyTransform.target = rootTarget;
			weaponCopyTransform.SetTargetName(weaponCopyTransform.GetTargetName());
		}
		
		protected virtual void OnGunFired()
		{
			projectileHandler.OrNull()?.FireProjectile(TargetHitCallback);
		}

		#region Unity Methods

		private void Awake()
		{
			if (gunFireComponent == null)
			{
				gunFireComponent = this.GetOrAddComponent<GunFireComponent>();
				gunFireComponent.SetOwner(this);
			}

			if (gunDamageComponent == null)
			{
				gunDamageComponent = this.GetOrAddComponent<GunDamageComponent>();
				gunDamageComponent.SetOwner(this);
			}
			
			if (projectileHandler != null)
			{
				projectileHandler.SetOwner(this);
			}
		}

		private void OnEnable()
		{
			gunFireComponent.OnShoot += OnGunFired;
		}

		private void OnDisable()
		{
			gunFireComponent.OnShoot -= OnGunFired;
		}

		#endregion
		
		private void TargetHitCallback(HitContext hitContext)
		{
			if (hitContext.hitTarget == null) return;
			if (hitContext.hitTarget.TryGetComponent(out IDamageable damageable))
			{
				damageable.Damage(gunDamageComponent.GetDamage(), _owner);
			}
		}
	}
}