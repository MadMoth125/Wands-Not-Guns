using System;
using System.Collections;
using UnityEngine;

namespace Core.ObjectPool.Example
{
	/* Exposed items:
	 *
	 * Serialized Fields
	 * - Vector3 randomSpawnRange
	 * - float spawnInterval
	 */
	
	/// <summary>
	/// Example of a component that spawns objects from a pool at a set interval.
	/// </summary>
	public class SpawnPoolObjects : MonoBehaviour
	{
		[SerializeField]
		private Vector3 randomSpawnRange = Vector3.one;
		
		[SerializeField]
		private float spawnInterval = 1f;
		
		private ExampleComponentPool _pool;
		private float _elapsedTime;
		
		#region Unity Methods

		private void Awake()
		{
			_pool = GetComponent<ExampleComponentPool>();
		}

		private void Update()
		{
			// Handle the timer via Update rather than a coroutine.
			// Coroutines create unnecessary garbage and are not needed for this simple task.
			if (_elapsedTime >= spawnInterval)
			{
				_elapsedTime = 0;
				SpawnObjects();
			}
			else
			{
				_elapsedTime += Time.deltaTime;
			}
		}
		
		#endregion
		
		private void SpawnObjects()
		{
			_pool.GetElement(RandomPosition(randomSpawnRange) + transform.position)
				.SetPool(_pool);
		}
		
		private static Vector3 RandomPosition(Vector3 range)
		{
			return new Vector3(
				UnityEngine.Random.Range(-range.x, range.x),
				UnityEngine.Random.Range(-range.y, range.y),
				UnityEngine.Random.Range(-range.z, range.z));
		}
	}
}