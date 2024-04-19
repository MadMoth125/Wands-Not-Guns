using System;
using Core.Owning;
using UnityEngine;

namespace Weapons
{
	/// <summary>
	/// Base class for handling logic behind projectiles fire from a weapon.
	/// Supports anything so long as it calls the appropriate callback when the projectile hits a target.
	/// </summary>
	public abstract class ProjectileHandler : MonoBehaviour, IOwnable<GunComponent>
	{
		private GunComponent _owner;
		
		public abstract void FireProjectile(Action<HitContext> onHitComplete);
		
		public GunComponent GetOwner()
		{
			return _owner;
		}
		
		public void SetOwner(GunComponent owner)
		{
			_owner = owner;
		}
	}
}