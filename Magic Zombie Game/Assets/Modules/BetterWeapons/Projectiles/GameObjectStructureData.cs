using System.Collections.Generic;
using UnityEngine;

public struct GameObjectStructureData
{
	public GameObjectStructureData(List<GameObject> instances)
	{
		_states = new (Transform parent, Vector3 localPosition, bool active)[instances.Count];
		SaveGameObjectState(instances);
	}
	
	public GameObjectStructureData(List<Transform> instances)
	{
		_states = new (Transform parent, Vector3 localPosition, bool active)[instances.Count];
		SaveGameObjectState(instances);
	}

	public void SaveGameObjectState(List<Transform> instances)
	{
		_states = new (Transform parent, Vector3 localPosition, bool active)[instances.Count];
		
		for (int i = 0; i < _states.Length; i++)
		{
			_states[i] = (instances[i].parent,
				instances[i].localPosition,
				instances[i].gameObject.activeSelf);
		}
	}
	
	public void SaveGameObjectState(List<GameObject> instances)
	{
		_states = new (Transform parent, Vector3 localPosition, bool active)[instances.Count];
		
		for (int i = 0; i < _states.Length; i++)
		{
			_states[i] = (instances[i].transform.parent,
				instances[i].transform.localPosition,
				instances[i].activeSelf);
		}
	}
		
	public void RetrieveGameObjectState(List<Transform> instances)
	{
		for (int i = 0; i < _states.Length; i++)
		{
			instances[i].SetParent(_states[i].parent, true);
			instances[i].localPosition = _states[i].localPosition;
			instances[i].gameObject.SetActive(_states[i].active);
		}
	}
		
	public void RetrieveGameObjectState(List<GameObject> instances)
	{
		for (int i = 0; i < _states.Length; i++)
		{
			instances[i].transform.SetParent(_states[i].parent, true);
			instances[i].transform.localPosition = _states[i].localPosition;
			instances[i].SetActive(_states[i].active);
		}
	}

	private (Transform parent, Vector3 localPosition, bool active)[] _states;
}