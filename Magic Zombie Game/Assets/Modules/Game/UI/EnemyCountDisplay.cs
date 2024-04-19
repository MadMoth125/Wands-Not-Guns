using System.Collections.Generic;
using Enemy.Registry;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class EnemyCountDisplay : MonoBehaviour
{
	[RegistryCategory]
	[Required]
	public EnemyRegistry enemyRegistry;

	[Required]
	public TextMeshProUGUI textMesh;

	#region Unity Methods

	private void Start()
	{
		ChangeDisplayedCount(enemyRegistry.Count);
	}
	
	private void OnEnable()
	{
		enemyRegistry.OnItemAdded += OnRegistryCountChanged;
		enemyRegistry.OnItemRemoved += OnRegistryCountChanged;
		enemyRegistry.OnRegistryCleared += OnRegistryCleared;
	}

	private void OnDisable()
	{
		enemyRegistry.OnItemAdded -= OnRegistryCountChanged;
		enemyRegistry.OnItemRemoved -= OnRegistryCountChanged;
		enemyRegistry.OnRegistryCleared -= OnRegistryCleared;
	}

	#endregion

	private void OnRegistryCountChanged(int enemyId, EnemyComponent enemy)
	{
		ChangeDisplayedCount(enemyRegistry.Count);
	}

	private void OnRegistryCleared(IEnumerable<int> enemyIds, IEnumerable<EnemyComponent> enemies)
	{
		ChangeDisplayedCount(0);
	}

	private void ChangeDisplayedCount(int count)
	{
		textMesh.text = $"Enemies: {count}";
	}
}