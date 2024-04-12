using UnityEngine;
using Core.ObjectPool;

namespace Weapons.Projectiles
{
	public class ProjectileObjectPool : ObjectPoolBase<ParticleHandler>
	{
		public override ParticleHandler GetElement(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
		{
			var element = base.GetElement(position, rotation, scale);
			element.OverrideTransform(element.transform);
			element.OnGet();
			element.HandleRelease(() => ReleaseElement(element));
			return element;
		}

		public override void ReleaseElement(ParticleHandler element)
		{
			base.ReleaseElement(element);
			element.OnRelease();
		}

		private void FixedUpdate()
		{
			foreach (var ph in GetAllActiveObjects())
			{
				ph.MovementUpdate();
			}
		}
	}
}