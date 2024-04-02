using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class EnemyRegistryHandler : EnemySpawnerComponentBase
{
	public void Register(EnemyComponent enemy)
	{
		EnemyRegistry.Register(enemy.gameObject.GetInstanceID(), enemy);
	}
	
	public void Unregister(EnemyComponent enemy)
	{
		EnemyRegistry.Unregister(enemy.gameObject.GetInstanceID());
	}
}