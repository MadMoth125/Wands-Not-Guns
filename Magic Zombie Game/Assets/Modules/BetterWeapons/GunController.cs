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
			if (ctx.performed)
			{
				gun.FireGun();
			}
			else if (ctx.canceled)
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