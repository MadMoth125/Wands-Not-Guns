using Sirenix.OdinInspector;
using UnityEngine;

[DisallowMultipleComponent]
public class Gun : MonoBehaviour
{
	public int damage = 10;
	public float range = 20f;
	
	[SerializeField]
	private ProjectileHandler projectileHandler;

	[Button]
	public void FireGun()
	{
		projectileHandler.FireProjectile(TargetHitCallback);
	}
	
	private void TargetHitCallback(ProjectileHandler.HitContext hitContext)
	{
		Debug.Log($"Hit target: {hitContext.target.name} {hitContext.distance} units away.");
	}
}