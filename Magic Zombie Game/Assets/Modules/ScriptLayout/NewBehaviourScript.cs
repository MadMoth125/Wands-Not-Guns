using MyCustomControls;
using UnityEngine;
using UnityEngine.Events;

public class NewBehaviourScript : MonoBehaviour
{
	[UnityEventsCategory]
	public UnityEvent myUnityEvent1 = new UnityEvent();
	[UnityEventsCategory]
	public UnityEvent myUnityEvent2 = new UnityEvent();
	[UnityEventsCategory]
	public UnityEvent myUnityEvent3 = new UnityEvent();
	[UnityEventsCategory]
	public UnityEvent myUnityEvent4 = new UnityEvent();
	
	[RegistryCategory]
	public int myInt = 0;
	[ScriptableEventCategory]
	public float myFloat = 0.0f;
	[RegistryCategory]
	public string myString = "Hello World!";
	[ScriptableEventCategory]
	public bool myBool = false;
	[ScriptableEventCategory]
	public Vector3 myVector3 = Vector3.zero;
	[RegistryCategory]
	public GameObject myGameObject = null;
	[ScriptableEventCategory]
	public Transform myTransform = null;
	[RegistryCategory]
	public Material myMaterial = null;
	[RegistryCategory]
	public Mesh myMesh = null;
	[RegistryCategory]
	public Texture myTexture = null;

	[ScriptableEventCategory]
	public ScriptableObjectGameControls gameControls;
	[ShowPropertyInlineEditor]
	[ScriptableEventCategory]
	public RoundNumberScriptableObject roundNumber;
	
	[ExternalComponentCategory]
	public EnemyCounter enemyCounter;
	[ExternalComponentCategory]
	public EnemyManager enemyManager;
	[ExternalComponentCategory]
	public RoundManager roundManager;
	[ExternalComponentCategory]
	public GameManager gameManager;
	[ExternalComponentCategory]
	public EnemyComponent enemyComponent;
	
	#region Unity Methods

	private void Start()
	{
		
	}

	private void Update()
	{
		
	}

	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		
	}

	#endregion
}