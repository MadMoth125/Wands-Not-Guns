using MyCustomControls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Weapons
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(GunComponent))]
	public class GunController : MonoBehaviour
	{
		public bool enableControls = true;

		[Required]
		[SerializeField]
		private ScriptableObjectGameControls gameControls;

		private GunComponent _gunComponent;
		private bool _hasPressed = false;

		public GunComponent GetGunComponent()
		{
			return _gunComponent;
		}
		
		#region Unity Methods

		private void Awake()
		{
			_gunComponent = GetComponent<GunComponent>();
			_gunComponent.SetOwnerController(this);
		}

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
			if (!enableControls) return;
			
			if (ctx.performed)
			{
				_hasPressed = true;
				_gunComponent.FireGun();
			}
			else if (ctx.canceled)
			{
				if (_hasPressed) _gunComponent.StopFiring();
				_hasPressed = false;
			}
		}
		
		private void OnDeviceChange(Device device)
		{
			Debug.Log($"Device changed to {device}");
		}
	}
}