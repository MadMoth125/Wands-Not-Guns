using Player.Registry;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Player
{
	public class PlayerRegistryHandler : MonoBehaviour
	{
		[RegistryCategory]
		[Required]
		[SerializeField]
		private PlayerRegistry playerRegistry;
		
		[Required]
		[SerializeField]
		private PlayerComponent playerComponent;
	
		#region Unity Methods

		private void OnEnable()
		{
			playerRegistry.Register(playerComponent.PlayerId, transform);
			playerComponent.HealthComponent.OnDie += HandleDie;
		}

		private void OnDisable()
		{
			playerRegistry.Unregister(playerComponent.PlayerId);
			playerComponent.HealthComponent.OnDie -= HandleDie;
		}

		#endregion

		private void HandleDie()
		{
			playerRegistry.Unregister(playerComponent.PlayerId);
		}
	}
}