using MyCustomControls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Gun))]
	public class GunController : MonoBehaviour
	{
		[Required]
		[SerializeField]
		private ScriptableObjectGameControls gameControls;

		[OnValueChanged(nameof(OnGunFiredDummy))]
		[SerializeField]
		private bool setInputHeld = false;
		
		[SerializeField]
		private bool disableInput = false;
		
		[Required]
		[SerializeField]
		private Gun gun;

		#region Unity Methods

		private void OnEnable()
		{
			gameControls.OnAttackCallback += OnGunFired;
			gameControls.OnDeviceChange += OnDeviceChange;
		}

		private void OnDisable()
		{
			gameControls.OnAttackCallback -= OnGunFired;
			gameControls.OnDeviceChange -= OnDeviceChange;
		}

		#endregion

		private void OnGunFired(InputAction.CallbackContext ctx)
		{
			if (disableInput) return;
			
			if (ctx.performed)
			{
				gun.FireGun();
			}
			else if (ctx.canceled)
			{
				gun.StopFiring();
			}
		}

		private void OnGunFiredDummy()
		{
			if (setInputHeld)
			{
				gun.FireGun();
			}
			else if (!setInputHeld)
			{
				gun.StopFiring();
			}
		}
		
		private void OnDeviceChange(Device device)
		{
			Debug.Log($"Device changed to {device}");
		}
	}
}