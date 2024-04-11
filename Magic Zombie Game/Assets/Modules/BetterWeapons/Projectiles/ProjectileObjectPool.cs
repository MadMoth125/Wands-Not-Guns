using System;
using UnityEngine;
using Core.ObjectPool;
using UnityEngine.Pool;

public class ProjectileObjectPool : ObjectPoolBase<ParticelHandler>
{
	private void Update()
	{
		/*foreach (var ph in GetAllActiveObjects())
		{
			ph.StateMachineLogicUpdate();
			Debug.Log(ph.transform.position);
		}*/
	}
	
	private void FixedUpdate()
	{
		foreach (var ph in GetAllActiveObjects())
		{
			ph.MovementUpdate();
		}
	}

	public override ParticelHandler GetElement(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
	{
		var ph = base.GetElement(position, rotation, scale);
		ph.SetReferencedTransform(ph.transform);
		return ph;
	}

	public override void ReleaseElement(ParticelHandler element)
	{
		base.ReleaseElement(element);
	}
}