using System;
using System.Collections;
using System.Collections.Generic;
using Core.Registries;
using UnityEngine;

namespace Enemy.Registry
{
	[CreateAssetMenu(fileName = "EnemyRegistryAsset", menuName = "Registry/Enemy Registry")]
	public class EnemyRegistry : RegistryScriptableObject<int, EnemyComponent>
	{
		
	}
}