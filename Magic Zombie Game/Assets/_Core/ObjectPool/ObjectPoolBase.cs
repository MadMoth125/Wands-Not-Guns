using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.ObjectPool
{
	/// <summary>
	/// Generic wrapper for Unity's <see cref="ObjectPool{T}"/>.
	/// Gives some QOL features and a few extra methods for handling 'Get' and 'Release' actions in the pool.
	/// </summary>
	/// <typeparam name="T">The type of <see cref="Component"/> to pool.</typeparam>
	public abstract class ObjectPoolBase<T> : MonoBehaviour where T : Component
	{
		protected T Item => item;
		protected int DefaultCapacity => defaultCapacity;
		protected int MaximumCapacity => maximumCapacity;

		#region Pool Settings
		
		[Title("Pooled Item")]
		[Required]
		[AssetsOnly]
		[DisableInPlayMode]
		[SerializeField]
		private T item;
		
		[Title("Pool Settings")]
		[Tooltip("The default capacity of the pool.\n" +
		         "Recommended to keep slightly above the expected amount of spawned items.\n\n" +
		         "<b>Parameter cannot be changed during runtime.</b>")]
		[DisableInPlayMode]
		[PropertyRange(10, "maximumCapacity")]
		[SerializeField]
		private int defaultCapacity = 10;

		[Tooltip("The maximum capacity of the pool.\n\n" +
		         "<b>Parameter cannot be changed during runtime.</b>")]
		[DisableInPlayMode]
		[PropertyRange("defaultCapacity", 2500)]
		[SerializeField]
		private int maximumCapacity = 20;

		[Space(20)]
		
		[Tooltip("Whether or not to allow the destruction of elements when they are returned to the pool.\n" +
		         "Primarily happens when an element is returned to the pool while it is above the max capacity.\n\n" +
		         "<b>Parameter cannot be changed during runtime.</b>")]
		[DisableInPlayMode]
		[SerializeField]
		private bool allowElementDestruction = true;
		
		[Tooltip("Whether or not to enable collection checks for the pool.\n" +
		         "If enabled, The pool will throw a fit if an already returned element is being returned again.\n\n" +
		         "<b>Parameter cannot be changed during runtime.</b>")]
		[DisableInPlayMode]
		[SerializeField]
		private bool enableCollectionChecks = true;
		
		#endregion
		
		private ObjectPool<T> _pool;
		private readonly Dictionary<T, bool> _poolCollection = new();
		private bool _cachedAllowElementDestruction = true;

		#region Public Methods

		public virtual T GetElement(Transform spawnTransform)
		{
			Vector3 position = spawnTransform.position;
			Quaternion rotation = spawnTransform.rotation;
			return GetElement(position, rotation);
		}
		
		/// <summary>
		/// Gets an element from the pool.
		/// Allows you to optionally set the element's new position, rotation, and scale.
		/// </summary>
		/// <param name="position">The element's new position.</param>
		/// <param name="rotation">The element's new rotation.</param>
		/// <param name="scale">The element's new scale.</param>
		/// <returns>The element retrieved from the pool.</returns>
		public virtual T GetElement(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null)
		{
			T obj = _pool.Get();
			Transform cachedTransform = obj.transform;
			
			// Set the position, rotation, and scale of the object.
			if (position != null && rotation != null)
			{
				// if the pos and rot are both valid, use the
				// dedicated method for setting the position and rotation.
				obj.transform.SetPositionAndRotation(position.Value, rotation.Value);
			}
			else
			{
				// otherwise, set the position and rotation separately.
				cachedTransform.position = position ?? cachedTransform.position;
				cachedTransform.rotation = rotation ?? cachedTransform.rotation;
			}
			
			// set the scale of the object standalone.
			cachedTransform.localScale = scale ?? cachedTransform.localScale;
			
			return obj;
		}

		/// <summary>
		/// Return a specified object to the pool.
		/// </summary>
		/// <param name="element">The element to return to the pool.</param>
		public virtual void ReleaseElement(T element) => _pool.Release(element);

		public virtual void ReleaseElement(T element, bool resetState)
		{
			_pool.Release(element);
			
			// Add any functionality to reset the state
			// of the object from the overriding pool.
		}

		/// <summary>
		/// Method to return an object to the pool and reset its state via an action.
		/// 
		/// Important to note that using anonymous methods or lambda expressions cause GC allocations,
		/// so it's best to use a cached action when calling this method overload.
		/// </summary>
		/// <param name="element">The element to return to the pool.</param>
		/// <param name="resetStateAction">The action responsible for resetting the state of the element.</param>
		public virtual void ReleaseElement(T element, Action resetStateAction)
		{
			ReleaseElement(element);
			
			// no need to use the ?. operator here, as the action is required and cannot be null.
			resetStateAction.Invoke();
		}

		/// <summary>
		/// Gets a collection of all active objects in the pool.
		/// </summary>
		public virtual IEnumerable<T> GetAllActiveObjects() => from obj in _poolCollection where (obj.Value == true) select obj.Key;

		/// <summary>
		/// Gets a collection of all inactive objects in the pool.
		/// </summary>
		public virtual IEnumerable<T> GetAllInactiveObjects() => (from obj in _poolCollection where (obj.Value == false) select obj.Key);

		/// <summary>
		/// Gets the number of active objects in the pool.
		/// </summary>
		public int GetActiveObjectCount() => _pool.CountActive;

		/// <summary>
		/// Gets the number of inactive objects in the pool.
		/// </summary>
		public int GetInactiveObjectCount() => _pool.CountInactive;

		/// <summary>
		/// Returns all objects back to the pool.
		/// </summary>
		/// <param name="resetState">Whether or not to reset the state of the elements when returned to the pool.</param>
		public virtual void ReturnAllObjects(bool resetState)
		{
			foreach (var obj in _poolCollection)
			{
				ReleaseElement(obj.Key, resetState);
			}
		}

		#endregion

		/// <summary>
		/// Called when an element is either instantiated or retrieved from the pool.
		/// </summary>
		/// <param name="element">The element retrieved from the pool.</param>
		/// <param name="wasInstantiated">Whether or not the element was instantiated.</param>
		protected virtual void OnElementGet(T element, bool wasInstantiated) { }

		/// <summary>
		/// Called when an element is either released to the pool or destroyed.
		/// The destruction of the element is handled immediately after this method is called.
		/// </summary>
		/// <param name="element">The element returned to the pool.</param>
		/// <param name="wasDestroyed">Whether or not the element was destroyed.</param>
		protected virtual void OnElementRelease(T element, bool wasDestroyed) { }

		#region Object Pool Methods

		/// <summary>
		/// Pool function called to create an element for the pool.
		/// </summary>
		protected virtual T OnCreatePoolElement()
		{
			T obj = Instantiate(item);
			_poolCollection.Add(obj, true);
					
			OnElementGet(obj, true);
			return obj;
		}

		/// <summary>
		/// Pool function called to retrieve an element from the pool.
		/// </summary>
		protected virtual void OnGetPoolElement(T component)
		{
			component.gameObject.SetActive(true);
			_poolCollection[component] = true;
					
			OnElementGet(component, false);
		}
		
		/// <summary>
		/// Pool function called to release an element back to the pool.
		/// </summary>
		protected virtual void OnReleasePoolElement(T component)
		{
			OnElementRelease(component, false);
					
			component.gameObject.SetActive(false);
			_poolCollection[component] = false;
		}
		
		/// <summary>
		/// Pool function called to destroy an element in the pool.
		/// </summary>
		protected virtual void OnDestroyPoolElement(T component)
		{
			if (_cachedAllowElementDestruction)
			{
				OnElementRelease(component, true);
						
				_poolCollection.Remove(component);
				Destroy(component.gameObject);
			}
			else
			{
				OnElementRelease(component, false);
						
				component.gameObject.SetActive(false);
				_poolCollection[component] = false;
			}
		}

		#endregion

		#region Unity Methods

		protected virtual void Awake()
		{
			if (item == null)
			{
				Debug.LogAssertion("The prefab for the object pool is null.", gameObject);
				return;
			}
			
			_cachedAllowElementDestruction = allowElementDestruction;
			InitializeObjectPool();
		}

		protected virtual void OnDestroy()
		{
			if (_pool != null)
			{
				foreach (var component in _poolCollection.Keys)
				{
					if (component != null) Destroy(component.gameObject);
				}
			}
			
			_poolCollection.Clear();
		}

		#endregion

		/// <summary>
		/// Called in the <see cref="Awake"/> method to initialize the object pool.
		/// Uses <see cref="OnCreatePoolElement"/>, <see cref="OnGetPoolElement"/>,
		/// <see cref="OnReleasePoolElement"/>, and <see cref="OnDestroyPoolElement"/> for the pool functions.
		/// </summary>
		private void InitializeObjectPool()
		{
			_pool = new ObjectPool<T>(
				OnCreatePoolElement,
				OnGetPoolElement,
				OnReleasePoolElement,
				OnDestroyPoolElement,
				enableCollectionChecks,
				defaultCapacity,
				maximumCapacity);
		}
	}
}