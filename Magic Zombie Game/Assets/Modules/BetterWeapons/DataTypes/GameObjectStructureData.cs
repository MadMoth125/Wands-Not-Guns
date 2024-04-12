using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
	/// <summary>
	/// Stores the state of a list of GameObjects.
	/// State attributes include things like parent, local position, and enabled.
	/// </summary>
	public struct GameObjectStructureData
	{
		#region Constructors

		public GameObjectStructureData(IList<Transform> instances)
		{
			_states = new (Transform parent, Vector3 localPosition, bool active)[instances.Count];
			SaveGameObjectState(instances);
		}

		public GameObjectStructureData(IList<GameObject> instances)
		{
			_states = new (Transform parent, Vector3 localPosition, bool active)[instances.Count];
			SaveGameObjectState(instances);
		}

		#endregion

		private (Transform parent, Vector3 localPosition, bool active)[] _states;

		#region Public Methods

		/// <summary>
		/// Saves the state of the GameObjects in the list.
		/// Overwrites the current saved state.
		/// </summary>
		public void SaveGameObjectState(IList<Transform> instances)
		{
			_states = new (Transform parent, Vector3 localPosition, bool active)[instances.Count];
		
			for (int i = 0; i < _states.Length; i++)
			{
				_states[i] = (instances[i].parent,
					instances[i].localPosition,
					instances[i].gameObject.activeSelf);
			}
		}
	
		/// <summary>
		/// Saves the state of the GameObjects in the list.
		/// Overwrites the current saved state.
		/// </summary>
		public void SaveGameObjectState(IList<GameObject> instances)
		{
			_states = new (Transform parent, Vector3 localPosition, bool active)[instances.Count];
		
			for (int i = 0; i < _states.Length; i++)
			{
				_states[i] = (instances[i].transform.parent,
					instances[i].transform.localPosition,
					instances[i].activeSelf);
			}
		}
		
		/// <summary>
		/// Updates the state of the GameObjects in the list.
		/// Only updates the state of GameObjects that are still in the list.
		/// </summary>
		/// <param name="instances"></param>
		public void RetrieveGameObjectState(IList<Transform> instances)
		{
			for (int i = 0; i < _states.Length; i++)
			{
				if (i >= instances.Count)
				{
					break;
				}
				
				instances[i].SetParent(_states[i].parent, true);
				instances[i].localPosition = _states[i].localPosition;
				instances[i].gameObject.SetActive(_states[i].active);
			}
		}
		
		/// <summary>
		/// Updates the state of the GameObjects in the list.
		/// Only updates the state of GameObjects that are still in the list.
		/// </summary>
		public void RetrieveGameObjectState(IList<GameObject> instances)
		{
			for (int i = 0; i < _states.Length; i++)
			{
				if (i >= instances.Count)
				{
					break;
				}
				
				instances[i].transform.SetParent(_states[i].parent, true);
				instances[i].transform.localPosition = _states[i].localPosition;
				instances[i].SetActive(_states[i].active);
			}
		}

		#endregion
	}
}