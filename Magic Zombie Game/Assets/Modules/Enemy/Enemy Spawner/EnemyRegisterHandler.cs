using Enemy.Registry;
using Sirenix.OdinInspector;
using UnityEngine;

public class EnemyRegisterHandler : MonoBehaviour
{
	[Required]
	[SerializeField]
	private EnemySpawnManager spawnManager;
	
	[SerializeField]
	private EnemyRegistryAsset enemyRegistry;

	public void HandleRegister(EnemyComponent enemy)
	{
		enemyRegistry.Register(enemy.gameObject.GetInstanceID(), enemy);
	}

	public void HandleUnregister(EnemyComponent enemy)
	{
		enemyRegistry.Unregister(enemy.gameObject.GetInstanceID());
	}
}