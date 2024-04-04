using Enemy.Registry;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class EnemyCountDisplay : MonoBehaviour
{
	[Required]
	public EnemyManager manager;
	
	[Required]
	public TextMeshProUGUI text;
	
	#region Unity Methods

	private void Update()
	{
		text.text = $"Enemies: {manager.EnemyCounter.ConcurrentEnemyCount}";
	}

	#endregion
}