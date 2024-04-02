using Enemy.Registry;
using Player.Registry;

public interface IEnemySpawnerComponent
{
	public void SetSpawnManager(EnemySpawnManager manager);
	public void SetRegistryAssets(EnemyRegistryAsset enemyRegistry, PlayerRegistryAsset playerRegistry);
}