using Enemy.Registry;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;

public class EnemyRegistryDisplay : MonoBehaviour
{
	public string prefix = "Enemies: ";
	
	[Required]
	public TextMeshProUGUI text;
	
	[Required]
	public EnemyRegistryAsset registry;
	
	#region Unity Methods

	private void Update()
	{
		text.text = prefix + registry.Count;
	}

	#endregion
}