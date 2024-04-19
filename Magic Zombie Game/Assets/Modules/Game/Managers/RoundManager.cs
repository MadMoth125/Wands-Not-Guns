using System;
using System.Collections;
using Obvious.Soap;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[DefaultExecutionOrder(-5)]
public class RoundManager : MonoBehaviour, IManagerComponent<GameManager>
{
	#region Properties

	public GameManager ParentManager => _manager;
	public RoundNumberScriptableObject RoundValue => roundNumber;
	public bool ValidSpawnPeriod => roundNumber.roundActive;
	public int Round => roundNumber;
	public float RoundTransitionTime => transitionTime;

	#endregion

	#region Fields
	
	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventInt roundBeginEvent;
	
	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventInt roundEndEvent;

	[ScriptableEventCategory]
	[Required]
	[SerializeField]
	private ScriptableEventInt enemyDieEvent;

	[Tooltip("The time in seconds to wait before starting the next round.")]
	[SerializeField]
	private float transitionTime = 5f;

	[TitleGroup("Scriptable Objects", Alignment = TitleAlignments.Centered)]
	[Required]
	[SerializeField]
	private RoundNumberScriptableObject roundNumber;

	#endregion
	
	private GameManager _manager;

	public void SetParentManager(GameManager manager)
	{
		_manager = manager;
	}

	#region Unity Methods

	private void Start()
	{
		RoundOneInit();
		roundNumber.SetRound(1);
	}

	private void OnEnable()
	{
		// _manager.EnemyManager.EnemyCounter.OnMaxTotalCountReached += EnemyLimitReached;
		enemyDieEvent.OnRaised += FinalEnemyDeathListener;
	}

	private void OnDisable()
	{
		// _manager.EnemyManager.EnemyCounter.OnMaxTotalCountReached -= EnemyLimitReached;
		enemyDieEvent.OnRaised -= FinalEnemyDeathListener;
	}

	#endregion

	/// <summary>
	/// Listener for the final enemy death event.
	/// Responsible for transitioning to the next round upon the final enemy's death.
	/// </summary>
	private void FinalEnemyDeathListener(int enemyId)
	{
		if (!_manager.EnemyManager.EnemyCounter.ReachedMaxTotalEnemies())
		{
			return;
		}
		
		// using greater than 1 to account for the final enemy that just died
		// (strange order of events, idk)
		if (_manager.EnemyManager.EnemyCounter.ConcurrentEnemyCount > 1)
		{
			return;
		}
		
		TransitionNextRound();
	}

	/// <summary>
	/// Initializes the first round by setting the round active to false,
	/// waiting for the transition time, then setting the round active to true.
	/// Invokes the round begin event after the transition time.
	/// </summary>
	private void RoundOneInit()
	{
		StartCoroutine(TransitionTimer(PreWaitAction, PostWaitAction));

		return;
		
		void PreWaitAction()
		{
			roundNumber.roundActive = false;
			Debug.Log("Round One Initiated");
		}
		
		void PostWaitAction()
		{
			roundNumber.roundActive = true;
			roundBeginEvent.Raise(roundNumber);
			Debug.Log("Round One Begin");
		}
	}

	/// <summary>
	/// Handles the transition to the next round by setting the current round to inactive,
	/// waiting for the transition time, incrementing the round, and setting the round to active.
	/// Invokes the <see cref="OnRoundEnd"/> event before the transition time and the <see cref="OnRoundBegin"/> event after.
	/// </summary>
	private void TransitionNextRound()
	{
		StartCoroutine(TransitionTimer(PreWaitAction, PostWaitAction));

		return;

		void PreWaitAction()
		{
			roundNumber.roundActive = false;
			roundEndEvent.Raise(roundNumber);
			roundNumber.IncrementRound();
			Debug.Log($"Round {roundNumber} Ended...");
		}

		void PostWaitAction()
		{
			roundNumber.roundActive = true;
			roundBeginEvent.Raise(roundNumber);
			Debug.Log($"Round {roundNumber.Round} Begin!");
		}
	}

	private IEnumerator TransitionTimer(Action preWaitAction, Action postWaitAction)
	{
		preWaitAction?.Invoke();
		yield return new WaitForSeconds(RoundTransitionTime);
		postWaitAction?.Invoke();
	}
}