using Player.Registry;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerRegistryHandler : MonoBehaviour
{
	[Required]
	[SerializeField]
	private PlayerRegistry playerRegistry;
	
	#region Unity Methods

	private void OnEnable()
	{
		playerRegistry.Register(gameObject.GetInstanceID(), transform);
	}

	private void OnDisable()
	{
		playerRegistry.Unregister(gameObject.GetInstanceID());
	}

	#endregion
}