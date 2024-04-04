using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[DefaultExecutionOrder(-1)]
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
	
	private bool _allEnemiesSpawned = false;
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
		_manager.EnemyManager.EnemyCounter.OnMaxTotalCountReached += EnemyLimitReached;
		_manager.EnemyManager.Spawner.OnEnemyDie += FinalEnemyDeathListener;
	}

	private void OnDisable()
	{
		_manager.EnemyManager.EnemyCounter.OnMaxTotalCountReached -= EnemyLimitReached;
		_manager.EnemyManager.Spawner.OnEnemyDie -= FinalEnemyDeathListener;
	}

	#endregion

	private void FinalEnemyDeathListener(EnemyComponent enemy)
	{
		if (!_allEnemiesSpawned)
		{
			Debug.LogWarning("Not all enemies have finished spawning.");
			return;
		}
		
		if (_manager.EnemyManager.EnemyCounter.ConcurrentEnemyCount != 1)
		{
			// Debug.LogWarning("Final enemy died before all enemies spawned.");
			return;
		}
		
		TransitionNextRound();
	}

	private void EnemyLimitReached()
	{
		_allEnemiesSpawned = true;
	}

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
			_allEnemiesSpawned = false;
			OnRoundBegin?.Invoke();
			Debug.Log("Round One Begin");
		}
	}

	private void TransitionNextRound()
	{
		StartCoroutine(TransitionTimer(PreWaitAction, PostWaitAction));

		return;

		void PreWaitAction()
		{
			roundValueAsset.roundActive = false;
			OnRoundEnd?.Invoke();
			Debug.Log($"Round {roundValueAsset.Round} Ended...");
			roundValueAsset.IncrementRound();
		}

		void PostWaitAction()
		{
			roundValueAsset.roundActive = true;
			_allEnemiesSpawned = false;
			Debug.Log($"Round {roundValueAsset.Round} Begin!");
			OnRoundBegin?.Invoke();
		}
	}

	private IEnumerator TransitionTimer(Action preWaitAction, Action postWaitAction)
	{
		preWaitAction?.Invoke();
		yield return new WaitForSeconds(RoundTransitionTime);
		postWaitAction?.Invoke();
	}
}