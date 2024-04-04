using Enemy.Registry;
using Player.Registry;

namespace Enemy.Spawner.Components
{
	public interface IEnemySpawnerComponent
	{
		public void SetSpawnManager(EnemySpawnManager manager);
		public void SetRegistryAssets(EnemyRegistryAsset enemyRegistry, PlayerRegistryAsset playerRegistry, RoundValueAsset roundValue);
	}
}