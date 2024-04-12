using System;
using UnityEngine;

namespace Weapons
{
	/// <summary>
	/// Base class for handling logic behind projectiles fire from a weapon.
	/// Supports anything so long as it calls the appropriate callback when the projectile hits a target.
	/// </summary>
	public abstract class ProjectileHandler : MonoBehaviour
	{
		public abstract void FireProjectile(Action<HitContext> onHitComplete);
	}
}