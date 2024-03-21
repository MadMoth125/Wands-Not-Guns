using System;
using UnityEngine;

namespace Core.TickSystem
{
	public class Tick : MonoBehaviour
	{
		private static Tick instance;
	
		private static TickSystemScriptableObject asset;

		#region Static Methods

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeTickSystem()
		{
			if (instance != null)
			{
				Debug.LogWarning("Tick System already exists. Cancelling instantiation.");
				return;
			}
		
			// Create a new GameObject and add the TickSystemBehaviour component to it
			instance = new GameObject("Tick System (Generated)").AddComponent<Tick>();

			// Make the GameObject persist throughout the entire game
			DontDestroyOnLoad(instance.gameObject);
		}

		/// <summary>
		/// Wrapper for <see cref="TickSystemScriptableObject"/>.<see cref="TickSystemScriptableObject.AddListener"/>
		/// </summary>
		public static void AddListener(string layerName, Action listener) => asset.AddListener(layerName, listener);

		/// <summary>
		/// Wrapper for <see cref="TickSystemScriptableObject"/>.<see cref="TickSystemScriptableObject.RemoveListener"/>
		/// </summary>
		public static void RemoveListener(string layerName, Action listener) => asset.RemoveListener(layerName, listener);

		#endregion

		#region Unity Methods

		private void Awake()
		{
			asset = Resources.Load<TickSystemScriptableObject>("TickSystemBackend");
			asset.Initialize();
		}

		private void Update()
		{
			// The first layer is reserved for Unity's Update method
			asset.GetUnityTickLayers()[0].Tick();

			foreach (var tick in asset.GetTickLayers())
			{
				// If the tick should tick, then tick it
				// (ticking ticks tick ticket and ticked tick it. I'm going insane)
				if (tick.ShouldTick())
				{
					tick.Tick();
				}
			}
		}

		private void FixedUpdate()
		{
			// The second layer is reserved for Unity's FixedUpdate method
			asset.GetUnityTickLayers()[1].Tick();
		}
	
		private void LateUpdate()
		{
			// The third layer is reserved for Unity's LateUpdate method
			asset.GetUnityTickLayers()[2].Tick();
		}

		#endregion
	}
}