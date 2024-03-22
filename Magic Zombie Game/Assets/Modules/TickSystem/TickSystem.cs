using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomTickSystem.Components;
using UnityEngine;

namespace CustomTickSystem
{
	/// <summary>
	/// A global system that allows for the creation of tick layers
	/// that can be used to execute code at specific intervals.
	/// </summary>
	public class TickSystem : MonoBehaviour
	{
		private static TickSystem instance;
		private static TickSystemAsset dataAsset;

		#region Static Methods

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeTickSystem()
		{
			if (instance != null)
			{
				Debug.LogWarning("Tick System already exists. Stopping instantiation.");
				return;
			}
		
			// Create a new GameObject and add the TickSystemBehaviour component to it
			instance = new GameObject("Tick System (Auto-Generated)").AddComponent<TickSystem>();

			// Make the GameObject persist throughout the entire game
			DontDestroyOnLoad(instance.gameObject);
		}

		/// <summary>
		/// Subscribes a listener to a tick layer.
		/// </summary>
		/// <param name="layerName">The name of the tick layer.</param>
		/// <param name="listener">The listener to subscribe.</param>
		public static void AddListener(string layerName, Action listener)
		{
			var layer = GetLayer(layerName);
			layer?.eventContainer.AddListener(listener);
		}

		public static void AddListeners(string layerName, params Action[] listeners)
		{
			foreach (var listener in listeners)
			{
				TickSystem.AddListener(layerName, listener);
			}
		}

		/// <summary>
		/// Unsubscribes a listener from a tick layer.
		/// </summary>
		/// <param name="layerName">The name of the tick layer.</param>
		/// <param name="listener">The listener to unsubscribe.</param>
		public static void RemoveListener(string layerName, Action listener)
		{
			var layer = GetLayer(layerName);
			layer?.eventContainer.RemoveListener(listener);
		}
	
		public static void RemoveListeners(string layerName, params Action[] listeners)
		{
			foreach (var listener in listeners)
			{
				TickSystem.RemoveListener(layerName, listener);
			}
		}

		public static TickLayer GetLayer(string layerName)
		{
			if (dataAsset == null) return null;
			return dataAsset.GetAllTickGroups().FirstOrDefault(layer => CaseInsensitiveMatch(layerName, layer.parameters.name));
		}

		public static IEnumerable<TickLayer> GetLayers(params string[] layerNames)
		{
			IEnumerable<TickLayer> layers = Array.Empty<TickLayer>();
		
			foreach (var layerName in layerNames)
			{
				var layer = TickSystem.GetLayer(layerName);
				if (layer != null)
				{
					layers = layers.Append(layer);
				}
			}

			return layers;
		}

		private static bool CaseInsensitiveMatch(string a, string b)
		{
			return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
	
		#endregion

		#region Unity Methods

		private void Awake()
		{
			dataAsset = Resources.Load<TickSystemAsset>("TickSystemAsset");
		}

		private void Update()
		{
			if (dataAsset == null) return;
		
			if (dataAsset.useUnityUpdate)
			{
				dataAsset.GetUpdateTickGroup().eventContainer.Tick();
			}

			foreach (var layer in dataAsset.GetTickGroups())
			{
				if (!layer.parameters.enabled) continue;
				if (layer.timer.ShouldTick(layer.parameters.TickInterval(), Time.deltaTime))
				{
					layer.eventContainer.Tick();
				}
			}
		}

		private void FixedUpdate()
		{
			if (dataAsset == null) return;
		
			if (dataAsset.useUnityFixedUpdate)
			{
				dataAsset.GetFixedUpdateTickGroup().eventContainer.Tick();
			}
		}

		private void LateUpdate()
		{
			if (dataAsset == null) return;
		
			if (dataAsset.useUnityLateUpdate)
			{
				dataAsset.GetLateUpdateTickGroup().eventContainer.Tick();
			}
		}

		#endregion
	}
}