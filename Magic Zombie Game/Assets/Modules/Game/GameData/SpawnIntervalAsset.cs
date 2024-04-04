using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SpawnIntervalAsset", menuName = "Gameplay/Spawn Interval Asset")]
public class SpawnIntervalAsset : ScriptableObject
{
	[Tooltip("This is the maximum interval that the enemies will spawn at.")]
	[LabelText("Maximum Interval")]
	[SuffixLabel("/s", Overlay = true)]
	[SerializeField]
	private float maxSpawnInterval = 2f;

	[Tooltip("This is the minimum interval that the enemies will spawn at.")]
	[LabelText("Minimum Interval")]
	[SuffixLabel("/s", Overlay = true)]
	[SerializeField]
	private float minSpawnInterval = 0.1f;

	[Tooltip("This is the percentage that the interval is decreased by every round.\n\n" +
	         "Value sheet:\n" +
	         "0.01 = 1% decrease,\n" +
	         "0.1 = 10% decrease,\n" +
	         "0.5 = 50% decrease,\n" +
	         "etc.")]
	[LabelText("Reduction Rate")]
	[SuffixLabel("%", Overlay = true)]
	[SerializeField]
	private float reductionRate = 0.05f;
	
	[Tooltip("If the resulting interval value should be rounded. (nearest hundredth)")]
	[SerializeField]
	private bool enableRounding = false;

	[Required]
	[SerializeField]
	private RoundValueAsset roundValue;

	public float GetInterval()
	{
		return GameFunction.GetSpawnInterval(roundValue.Round, maxSpawnInterval, minSpawnInterval, reductionRate, enableRounding);
	}
	
	public float GetInterval(int round)
	{
		return GameFunction.GetSpawnInterval(round, maxSpawnInterval, minSpawnInterval, reductionRate, enableRounding);
	}
}