using MyCustomControls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class WeaponController : MonoBehaviour
{
	public WeaponComponent Weapon => weaponComponent;
	
	[Required]
	[SerializeField]
	private WeaponComponent weaponComponent;
	
	[Required]
	[SerializeField]
	private ScriptableObjectGameControls gameControls;
	
	public void SetWeaponComponent(WeaponComponent target)
	{
		weaponComponent = target;
	}
	
	#region Unity Methods

	private void Start()
	{
		
	}

	private void Update()
	{
		
	}

	private void OnEnable()
	{
		// controls.EnableControls();
		gameControls.OnAttackCallback += OnAttack;
	}

	private void OnDisable()
	{
		// controls.DisableControls();
		gameControls.OnAttackCallback -= OnAttack;
	}

	#endregion

	private void OnAttack(InputAction.CallbackContext ctx)
	{
		
	}
}