using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RoundValueAsset", menuName = "Gameplay/Round Value Asset")]
public class RoundValueAsset : ScriptableObject
{
	/// <summary>
	/// Invoked when the round is incremented.
	/// </summary>
	public event Action OnRoundIncremented;
	
	/// <summary>
	/// Invoked when the round is decremented.
	/// </summary>
	public event Action OnRoundDecremented;
	
	/// <summary>
	/// Invoked when the round is changed.
	/// </summary>
	public event Action OnRoundChange;
	
	public int Round => round;
	
	[SerializeField]
	private int round = 1;
	
	public void SetRound(int value)
	{
		round = value;
		OnRoundChange?.Invoke();
	}
	
	public void ResetRound()
	{
		round = 1;
		OnRoundChange?.Invoke();
	}
	
	public void IncrementRound()
	{
		round++;
		OnRoundIncremented?.Invoke();
	}
	
	public void DecrementRound()
	{
		round--;
		OnRoundDecremented?.Invoke();
	}
}