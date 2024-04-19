using System;
using System.Collections;
using System.Collections.Generic;
using Core.CustomDebugger;
using Core.Registries;
using ScriptExtensions;
using UnityEngine;

namespace Player.Registry
{
	[CreateAssetMenu(fileName = "PlayerRegistryAsset", menuName = "Registry/Player Registry")]
	public class PlayerRegistry : RegistryScriptableObject<int, Transform>
	{
		public (Transform, float) GetClosestPlayer(Transform root)
		{
			if (Count == 0)
			{
				LogRegistryEmpty();
				return (null, Mathf.Infinity);
			}

			return root.FindClosestTransform(registry.Values);
		}
		
		protected void LogRegistryEmpty()
		{
			LogWrapper("Registry is empty. (No players in the registry.)", LoggerType.Warning);
		}
	}
}