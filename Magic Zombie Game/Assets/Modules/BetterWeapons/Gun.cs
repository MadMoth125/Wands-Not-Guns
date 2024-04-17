using System;
using System.Collections;
using ScriptExtensions;
using UnityEngine;

namespace Weapons
{
	[DisallowMultipleComponent]
	public class Gun : MonoBehaviour
	{
		public int damage = 10;
		public bool enabled = true;
		
		[ExternalComponentCategory]
		[SerializeField]
		private ProjectileHandler projectileHandler;
		
		[ExternalComponentCategory]
		[SerializeField]
		private ShootComponent shootComponent;
		
		public void FireGun()
		{
			if (!enabled) return;
			
			shootComponent.OrNull()?.StartShoot();
		}

		public void StopFiring()
		{
			if (!enabled) return;
			
			shootComponent.OrNull()?.StopShoot();
		}

		protected virtual void OnGunFired()
		{
			projectileHandler.OrNull()?.FireProjectile(TargetHitCallback);
		}

		#region Unity Methods

		private void Awake()
		{
			if (shootComponent == null) shootComponent = this.GetOrAddComponent<ShootComponent>();
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
			Debug.Log($"Hit target: {hitContext.target.name} {hitContext.distance} units away.");
		}
	}
}