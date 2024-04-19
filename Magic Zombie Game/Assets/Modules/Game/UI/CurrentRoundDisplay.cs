using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class CurrentRoundDisplay : MonoBehaviour
{
	[Required]
	public TextMeshProUGUI textMesh;

	[Required]
	public RoundNumberScriptableObject roundNumber;

	#region Unity Methods

	private void OnEnable()
	{
		roundNumber.OnRoundChange += OnRoundChanged;
	}

	private void OnDisable()
	{
		roundNumber.OnRoundChange -= OnRoundChanged;
	}

	#endregion

	private void OnRoundChanged()
	{
		textMesh.text = $"Round: {roundNumber.Round}";
	}
}