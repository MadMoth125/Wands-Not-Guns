using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[DefaultExecutionOrder(-10)]
public class GameManager : MonoBehaviour
{
	#region Properties

	public EnemyManager EnemyManager => enemyManager;
	public RoundManager RoundManager => roundManager;

	#endregion

	#region Fields

	[RequiredExternalComponent]
	[SerializeField]
	private EnemyManager enemyManager;
	
	[RequiredExternalComponent]
	[SerializeField]
	private RoundManager roundManager;

	#endregion

	private List<IManagerComponent<GameManager>> _components;
	
	#region Unity Methods

	private void Awake()
	{
		_components = new List<IManagerComponent<GameManager>>
		{
			enemyManager,
			roundManager,
		};

		foreach (var component in _components)
		{
			component.SetParentManager(this);
		}
	}

	#endregion

}