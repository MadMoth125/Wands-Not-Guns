using Enemy.Registry;
using Obvious.Soap;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[TypeInfoBox("This component is responsible for setting the health of an enemy when it is spawned.\n" +
             "It listens for the onEnemySpawnedEvent and sets the max health of the enemy to the value from the RoundHealthScriptableObject.\n" +
             "The RoundHealthScriptableObject already has the logic and references to the RoundNumberScriptableObject to calculate the max health of the enemy.")]
[DisallowMultipleComponent]
public class EnemyHealthSetter : MonoBehaviour, IManagerComponent<EnemyManager>
{
	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventInt enemySpawnedEvent;
	
	[RegistryCategory]
	[Required]
	[SerializeField]
	private EnemyRegistry enemyRegistry;

	[Required]
	[SerializeField]
	private RoundHealthScriptableObject roundHealth;

	private EnemyManager _manager;
	
	public void SetParentManager(EnemyManager manager)
	{
		_manager = manager;
	}

	#region Unity Methods

	private void Start()
	{
		
	}

	private void Update()
	{
		
	}

	private void OnEnable()
	{
		enemySpawnedEvent.OnRaised += UpdateEnemyMaxHealth;
	}

	private void OnDisable()
	{
		enemySpawnedEvent.OnRaised -= UpdateEnemyMaxHealth;
	}

	#endregion

	private void UpdateEnemyMaxHealth(int enemyId)
	{
		if (enemyRegistry == null) return;

		var enemy = enemyRegistry.GetValue(enemyId);
		
		if (enemy != null)
		{
			enemy.HealthComponent.SetMaxHealth(roundHealth.GetMaxHealth(), true);
		}
	}
}