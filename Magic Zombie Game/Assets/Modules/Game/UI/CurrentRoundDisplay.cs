using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CurrentRoundDisplay : MonoBehaviour
{
	[Required]
	public RoundValueAsset roundValueAsset;
	
	[Required]
	public TextMeshProUGUI text;
	
	#region Unity Methods

	private void Update()
	{
		text.text = $"Round: {roundValueAsset.Round}";
	}

	#endregion
}