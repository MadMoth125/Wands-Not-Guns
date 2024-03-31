using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RoundValueAsset", menuName = "Gameplay/Round Value Asset")]
public class RoundValueAsset : ScriptableObject
{
	public int Round => round;
	
	[SerializeField]
	private int round = 1;
	
	public void SetRound(int value)
	{
		round = value;
	}
	
	public void ResetRound()
	{
		round = 1;
	}
	
	public void IncrementRound()
	{
		round++;
	}
	
	public void DecrementRound()
	{
		round--;
	}
}