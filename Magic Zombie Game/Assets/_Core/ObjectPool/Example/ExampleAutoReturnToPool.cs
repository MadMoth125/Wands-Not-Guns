using System;
using UnityEngine;

namespace ObjectPool.Example
{
	/* Exposed items:
	 *
	 * Serialized
	 * - float timeToReturn
	 *
	 * Methods
	 * - void SetPool(ExampleComponentPool pool)
	 */
	
	/// <summary>
	/// Example of a component that returns itself to the pool after a set amount of time.
	///
	/// When the object is spawned, the pool is cached.
	/// When the timer expires, the object is returned to the pool.
	///
	/// We use an action to reset the object's state when it is returned to the pool.
	/// There are alternative ways to return items to the pool and reset their state,
	/// but this is the most efficient method for this use-case.
	///
	/// We use Update to handle the timer rather than a coroutine,
	/// as coroutines ALONE create unnecessary garbage (regardless if using cached yields.)
	/// </summary>
	public class ExampleAutoReturnToPool : MonoBehaviour
	{
		[Tooltip("The timer duration (in seconds) until the object is returned to the pool.")]
		[SerializeField]
		private float timeToReturn = 3f;
	
		private ExampleComponentPool _pool;
		private Rigidbody _rigidbody;
		private Action _resetAction;
		
		private float _elapsedTime;
		
		// public function called externally to cache the "owning" pool
		// when the object is spawned (enabled or instantiated)
		public void SetPool(ExampleComponentPool pool)
		{
			_pool = pool;
		}

		#region Unity Methods

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			
			_resetAction = () => 
			{
				// Cache the action to avoid creating a new one every time in the ReturnToPool function.
				// Recommended to always use cached actions when calling the ReturnToPool method as to avoid GC allocations.
				_rigidbody.velocity = Vector3.zero;
				_rigidbody.angularVelocity = Vector3.zero;
			};
		}

		private void Update()
		{
			// Handle the timer via Update rather than a coroutine.
			// Coroutines create unnecessary garbage and are not needed for this simple task.
			if (_elapsedTime >= timeToReturn)
			{
				_elapsedTime = 0;
				_pool.ReleaseElement(this, _resetAction);
			}
			else
			{
				_elapsedTime += Time.deltaTime;
			}
		}
		
		#endregion
		
	}
}