using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class RoundManager : MonoBehaviour, IManagerComponent<GameManager>
{
	#region Events

	public event Action OnRoundBegin;
	public event Action OnRoundEnd;

	#endregion

	#region Properties

	public GameManager ParentManager => _manager;
	public RoundValueAsset RoundValue => roundValueAsset;
	public bool ValidSpawnPeriod => roundValueAsset.roundActive;
	public int Round => roundValueAsset.Round;
	public float RoundTransitionTime => transitionTime;

	#endregion

	#region Fields

	[Required]
	[SerializeField]
	private RoundValueAsset roundValueAsset;

	[Tooltip("The time in seconds to wait before starting the next round.")]
	[SerializeField]
	private float transitionTime = 5f;

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
		roundValueAsset.SetRound(1);
	}

	private void OnEnable()
	{
		// _manager.EnemyManager.EnemyCounter.OnMaxTotalCountReached += EnemyLimitReached;
		_manager.EnemyManager.Spawner.OnEnemyDie += FinalEnemyDeathListener;
	}

	private void OnDisable()
	{
		// _manager.EnemyManager.EnemyCounter.OnMaxTotalCountReached -= EnemyLimitReached;
		_manager.EnemyManager.Spawner.OnEnemyDie -= FinalEnemyDeathListener;
	}

	#endregion

	/// <summary>
	/// Listener for the final enemy death event.
	/// Responsible for transitioning to the next round upon the final enemy's death.
	/// </summary>
	private void FinalEnemyDeathListener(EnemyComponent enemy)
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
	/// Invokes the <see cref="OnRoundBegin"/> event after the transition time.
	/// </summary>
	private void RoundOneInit()
	{
		StartCoroutine(TransitionTimer(PreWaitAction, PostWaitAction));

		return;
		
		void PreWaitAction()
		{
			roundValueAsset.roundActive = false;
			Debug.Log("Round One Initiated");
		}
		
		void PostWaitAction()
		{
			roundValueAsset.roundActive = true;
			OnRoundBegin?.Invoke();
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
			roundValueAsset.roundActive = false;
			roundValueAsset.IncrementRound();
			OnRoundEnd?.Invoke();
			Debug.Log($"Round {roundValueAsset.Round} Ended...");
		}

		void PostWaitAction()
		{
			roundValueAsset.roundActive = true;
			OnRoundBegin?.Invoke();
			Debug.Log($"Round {roundValueAsset.Round} Begin!");
		}
	}

	private IEnumerator TransitionTimer(Action preWaitAction, Action postWaitAction)
	{
		preWaitAction?.Invoke();
		yield return new WaitForSeconds(RoundTransitionTime);
		postWaitAction?.Invoke();
	}
}