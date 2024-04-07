using MyCustomControls;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Gun))]
public class GunController : MonoBehaviour
{
	[SerializeField]
	private GameControlsAsset controls;

	private Gun _gun;

	#region Unity Methods

	private void Awake()
	{
		_gun = GetComponent<Gun>();
	}

	private void OnEnable()
	{
		controls.OnAttackCallback += OnGunFired;
	}

	private void OnDisable()
	{
		controls.OnAttackCallback -= OnGunFired;
	}

	#endregion

	private void OnGunFired(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			_gun.FireGun();
		}
	}
}