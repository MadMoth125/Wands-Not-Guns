using UnityEngine;


[RequireComponent(typeof(EnemySpawnHandler))]
[RequireComponent(typeof(EnemySpawnTimer))]
[RequireComponent(typeof(EnemyCountHandler))]
[RequireComponent(typeof(EnemyRegisterHandler))]
[RequireComponent(typeof(EnemyObjectPool))]
public class EnemySpawnManager : MonoBehaviour
{
	public EnemySpawnHandler Spawner => _spawnerHandler;
	public EnemySpawnTimer Timer => _spawnTimer;
	public EnemyCountHandler Counter => _countHandler;
	public EnemyRegisterHandler Register => _registerHandler;
	public EnemyObjectPool Pool => _pool;
	
	private EnemySpawnHandler _spawnerHandler;
	private EnemySpawnTimer _spawnTimer;
	private EnemyCountHandler _countHandler;
	private EnemyRegisterHandler _registerHandler;
	private EnemyObjectPool _pool;
	
	#region Unity Methods

	private void Awake()
	{
		_spawnerHandler = GetComponent<EnemySpawnHandler>();
		_spawnTimer = GetComponent<EnemySpawnTimer>();
		_countHandler = GetComponent<EnemyCountHandler>();
		_registerHandler = GetComponent<EnemyRegisterHandler>();
		_pool = GetComponent<EnemyObjectPool>();
	}

	private void OnEnable()
	{
		_spawnTimer.OnTimerTick += SpawnTick;
		_spawnerHandler.OnEnemySpawn += _registerHandler.HandleRegister;
		_spawnerHandler.OnEnemyDie += _registerHandler.HandleUnregister;
		_spawnerHandler.OnEnemySpawn += _countHandler.EnemyCountIncreased;
		_spawnerHandler.OnEnemyDie += _countHandler.EnemyCountDecreased;
	}

	private void OnDisable()
	{
		_spawnTimer.OnTimerTick -= SpawnTick;
		_spawnerHandler.OnEnemySpawn -= _registerHandler.HandleRegister;
		_spawnerHandler.OnEnemyDie -= _registerHandler.HandleUnregister;
		_spawnerHandler.OnEnemySpawn -= _countHandler.EnemyCountIncreased;
		_spawnerHandler.OnEnemyDie -= _countHandler.EnemyCountDecreased;
	}

	#endregion

	private void SpawnTick(float time)
	{
		if (_countHandler.ReachedMaxConcurrentEnemies() || _countHandler.ReachedMaxTotalEnemies()) return;
		var spawnPoint = _spawnerHandler.spawnPoints[Random.Range(0, _spawnerHandler.spawnPoints.Count)];
		_spawnerHandler.SpawnEnemy(spawnPoint);
	}
}