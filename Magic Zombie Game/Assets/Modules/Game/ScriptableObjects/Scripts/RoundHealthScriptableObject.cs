using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundHealthAsset", menuName = "Gameplay/Round Health Asset")]
public class RoundHealthScriptableObject : ScriptableObject
{
	[TitleGroup("Required Components", Order = 100)]
	[Required]
	[SerializeField]
	private RoundNumberScriptableObject round;
	
	[SuffixLabel("hp", Overlay = true)]
	[SerializeField]
	private float startingHealth = 150f;
	
	[Tooltip("This is the value added to the health every round, for the first 9 rounds.")]
	[LabelText("Additive Increase")]
	[SuffixLabel("hp", Overlay = true)]
	[SerializeField]
	private float flatIncreaseAmount = 100f;
	
	[Tooltip("This is the percentage that the health is increased by every round, from round 10 and beyond.\n\n" +
	         "Value sheet:\n" +
	         "0.01 = 1% increase,\n" +
	         "0.1 = 10% increase,\n" +
	         "0.5 = 50% increase,\n" +
	         "etc.")]
	[LabelText("Percentage Increase")]
	[SuffixLabel("%", Overlay = true)]
	[SerializeField]
	private float percentageIncreaseAmount = 0.1f;
	
	public float GetMaxHealth()
	{
		return GameFunction.GetMaxHealthFloat(round, startingHealth, flatIncreaseAmount, percentageIncreaseAmount);
	}
}