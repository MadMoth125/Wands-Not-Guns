using System;
using UnityEngine;

namespace Core
{
	/// <summary>
	/// Simple class that invokes events for the update and fixed update ticks.
	/// An instance should not be created manually, as it is created automatically and
	/// will persist throughout the entire game.
	/// </summary>
	[SelectionBase]
	[DisallowMultipleComponent]
	public class TickSystem : MonoBehaviour
	{
		public static event Action<float> OnUpdateTick;
		public static event Action<float> OnFixedUpdateTick;
		
		private static TickSystem instance;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void InitializeTickSystem()
		{
			if (instance != null)
			{
				Debug.LogWarning("Tick System already exists. Cancelling instantiation.");
				return;
			}
			instance = new GameObject("Tick System (Generated)").AddComponent<TickSystem>();
			DontDestroyOnLoad(instance.gameObject);
		}
		
		#region Unity Methods

		private void Update()
		{
			OnUpdateTick?.Invoke(Time.deltaTime);
		}
		
		private void FixedUpdate()
		{
			OnFixedUpdateTick?.Invoke(Time.fixedDeltaTime);
		}

		#endregion
	}
}