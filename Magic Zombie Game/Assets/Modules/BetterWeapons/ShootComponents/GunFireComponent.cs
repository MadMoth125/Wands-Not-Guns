using System;
using Core.Owning;
using Mr_sB.UnityTimer;
using UnityEngine;

namespace Weapons
{
	[DisallowMultipleComponent]
	public class GunFireComponent : MonoBehaviour, IOwnable<GunComponent>
	{
		public event Action OnShoot;
		
		public bool IsFiring => isAutomatic ? _fullAutoShootComponent.IsFiring : _semiAutoShootComponent.IsFiring;
		
		[Tooltip("The number of time the gun can shoot per second.")]
		[SerializeField]
		private int fireRate = 5;

		[Tooltip("Whether the gun is automatic or not.\n" +
		         "(Hold to fire vs. press to fire)")]
		[SerializeField]
		private bool isAutomatic = true;
	
		private GunComponent _owner;
		private FullAutoShootComponent _fullAutoShootComponent;
		private SemiAutoShootComponent _semiAutoShootComponent;

		public void StartShoot()
		{
			if (isAutomatic) _fullAutoShootComponent.BeginFire();
			else _semiAutoShootComponent.BeginFire();
		}

		public void StopShoot()
		{
			if (isAutomatic) _fullAutoShootComponent.EndFire();
			// SemiAuto doesn't need to be stopped
		}

		public GunComponent GetOwner()
		{
			return _owner;
		}
		
		public void SetOwner(GunComponent owner)
		{
			_owner = owner;
		}

		#region Unity Methods

		private void Awake()
		{
			_fullAutoShootComponent = new FullAutoShootComponent(1f / fireRate, HandleFire);
			_semiAutoShootComponent = new SemiAutoShootComponent(1f / fireRate, HandleFire);
		}

		private void OnDestroy()
		{
			_fullAutoShootComponent?.Dispose();
			_semiAutoShootComponent?.Dispose();
		}

		#endregion

		private void HandleFire()
		{
			OnShoot?.Invoke();
		}
	}
}