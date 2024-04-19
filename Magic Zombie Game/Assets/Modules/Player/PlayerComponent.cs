using Core.HealthSystem;
using Player.Controller;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
	public MovementController ControllerComponent => controllerComponent;
	public HealthComponent HealthComponent => healthComponent;
	public int PlayerId => gameObject.GetInstanceID();
	
	[ExternalComponentCategory]
	[SerializeField]
	private HealthComponent healthComponent;
	
	[ExternalComponentCategory]
	[SerializeField]
	private MovementController controllerComponent;
	
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