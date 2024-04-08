using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RoundValueAsset", menuName = "Gameplay/Round Value Asset")]
public class RoundNumberScriptableObject : ScriptableObject
{
	/// <summary>
	/// Implicitly converts the reference to the round number.
	/// </summary>
	public static implicit operator int(RoundNumberScriptableObject roundNumber) => roundNumber.Round;
	
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
	
	[HideInInspector]
	public bool roundActive = true;
	
	[SerializeField]
	private int round = 1;
	
	public void SetRound(int value)
	{
		round = Mathf.Max(1, value);
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
		round = Mathf.Max(1, round - 1);
		OnRoundDecremented?.Invoke();
	}
}