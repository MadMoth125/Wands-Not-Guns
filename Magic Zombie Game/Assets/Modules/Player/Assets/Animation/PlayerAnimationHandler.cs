using Core.CustomTickSystem;
using Core.HealthSystem;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

namespace Player.AnimHandling
{
	[DefaultExecutionOrder(10)] // Make sure this runs after the PlayerComponent and any of its dependencies
	[DisallowMultipleComponent]
	public class PlayerAnimationHandler : MonoBehaviour
	{
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private PlayerComponent playerComponent;
	
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private Animator animator;
	
		[TitleGroup("Locomotion Animation Settings")]
		[SerializeField]
		private float blendSharpness = 10f;
		
		private bool _isFiring;
		private Vector3 _cachedVelocity;
		private Vector3 _cachedLocalVelocity;
		private const float HOLDING_GUN_LAYER_WEIGHT = 0.5f;
		private const float FIRING_GUN_LAYER_WEIGHT = 1f;
	
		private static readonly int verticalHash = Animator.StringToHash("Vertical"); // forward/backward anim blends
		private static readonly int horizontalHash = Animator.StringToHash("Horizontal"); // left/right anim blends
		private static readonly int oneHandedWeaponHash = Animator.StringToHash("OneHandedWeapon"); // bool for one handed weapon equipped
		private static readonly int hitEventHash = Animator.StringToHash("Hit"); // trigger for hit animation
		private static readonly int dieEventHash = Animator.StringToHash("Die"); // trigger for death animation
		private static readonly int reviveEventHash = Animator.StringToHash("Revive"); // trigger for exiting death animation state

		#region Unity Methods

		private void Awake()
		{
			animator ??= GetComponent<Animator>();
		}

		private void Update()
		{
			if (animator == null) return;
			if (playerComponent == null) return;
		
			float lerpTime = 1 - Mathf.Exp(-blendSharpness * Time.deltaTime);
			float verticalVelocity = Mathf.Clamp(_cachedLocalVelocity.z / playerComponent.ControllerComponent.MovementComponent.moveSpeed, -1, 1);
			float horizontalVelocity = Mathf.Clamp(_cachedLocalVelocity.x / playerComponent.ControllerComponent.MovementComponent.moveSpeed, -1, 1);

			float currentVertical = animator.GetFloat(verticalHash);
			float currentHorizontal = animator.GetFloat(horizontalHash);
			float currentFiringLayerWeight = animator.GetLayerWeight(1);
			
			// Lerp the values to smooth out the animation transitions
			animator.SetFloat(verticalHash, Mathf.Lerp(
				currentVertical.ApproxEquals(0f, 0.01f) ? 0f : currentVertical,
				verticalVelocity, lerpTime));
			
			animator.SetFloat(horizontalHash, Mathf.Lerp(
				currentHorizontal.ApproxEquals(0f, 0.01f) ? 0f : currentHorizontal,
				horizontalVelocity, lerpTime));
			
			animator.SetLayerWeight(1, Mathf.Lerp(
				currentFiringLayerWeight,
				_isFiring ? FIRING_GUN_LAYER_WEIGHT : HOLDING_GUN_LAYER_WEIGHT,
				lerpTime));
		}

		private void OnEnable()
		{
			TickSystem.AddListeners("Animator", CheckVelocity, CheckFiringState);
			playerComponent.HealthComponent.OnDamaged += HandleHit;
			playerComponent.HealthComponent.OnDie += HandleDie;
			playerComponent.WeaponBackpackComponent.OnGunEquipped += HandleGunEquipped;
			playerComponent.WeaponBackpackComponent.OnGunUnequipped += HandleGunUnEquipped;

			if (!playerComponent.HealthComponent.IsDead())
			{
				animator.SetTrigger(reviveEventHash);
			}
			
			animator.ResetTrigger(reviveEventHash);
			animator.ResetTrigger(dieEventHash);
		}

		private void OnDisable()
		{
			TickSystem.RemoveListeners("Animator", CheckVelocity, CheckFiringState);
			playerComponent.HealthComponent.OnDamaged -= HandleHit;
			playerComponent.HealthComponent.OnDie -= HandleDie;
			playerComponent.WeaponBackpackComponent.OnGunEquipped -= HandleGunEquipped;
			playerComponent.WeaponBackpackComponent.OnGunUnequipped -= HandleGunUnEquipped;
		}

		#endregion

		private void CheckVelocity()
		{
			if (animator == null) return;
			if (playerComponent == null) return;
		
			_cachedVelocity = playerComponent.ControllerComponent.BaseVelocity;
			_cachedLocalVelocity = transform.InverseTransformDirection(_cachedVelocity);
		}

		private void CheckFiringState()
		{
			if (animator == null) return;
			if (playerComponent == null) return;
			if (playerComponent.HealthComponent.IsDead()) return;
			var gun = playerComponent.WeaponBackpackComponent.GetCurrentGun();
			_isFiring = gun != null && gun.GunFireComponent.IsFiring;
		}

		private void HandleHit(HealthChangedArgs healthArgs, Object source)
		{
			if (animator == null) return;
			
			animator.SetTrigger(hitEventHash);
			animator.ResetTrigger(hitEventHash);
		}

		private void HandleDie()
		{
			if (animator == null) return;
			
			animator.SetTrigger(dieEventHash);
		}

		private void HandleGunEquipped(GunComponent gun)
		{
			if (animator == null) return;
			
			animator.SetBool(oneHandedWeaponHash, true);
		}

		private void HandleGunUnEquipped(GunComponent gun)
		{
			animator.SetBool(oneHandedWeaponHash, false);
		}
	}
}