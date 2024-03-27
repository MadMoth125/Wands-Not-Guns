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
	public class PlayerRegistryAsset : RegistryScriptableObject<int, Transform>
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
		
		public Transform GetRandomPlayer()
		{
			if (Count == 0)
			{
				LogRegistryEmpty();
				return null;
			}

			int randomIndex = UnityEngine.Random.Range(0, Count);
			var randomPlayer = registry[randomIndex];
			return randomPlayer;
		}
		
		protected void LogRegistryEmpty()
		{
			LogWrapper("Registry is empty. (No players in the registry.)", LoggerAsset.LogType.Warning);
		}
	}
}