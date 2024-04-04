using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class RoundDisplay : MonoBehaviour
{
	public string prefix = "Round: ";

	[Required]
	public TextMeshProUGUI text;

	[Required]
	public RoundValueAsset roundValueAsset;

	#region Unity Methods

	private void Start()
	{
		
	}

	private void Update()
	{
		text.text = prefix + roundValueAsset.Round;
	}

	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		
	}

	#endregion
}