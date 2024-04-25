using Core.CustomTickSystem;
using Core.HealthSystem;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Enemy.AnimationHandling
{
	public class EnemyAnimationHandler : MonoBehaviour
	{
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private EnemyComponent enemyComponent;
		
		[ExternalComponentCategory]
		[Required]
		[SerializeField]
		private Animator animator;

		[TitleGroup("Locomotion Animation Settings")]
		[SerializeField]
		private float blendSharpness = 10f;
		
		private float _cachedSpeed;
		private bool _isSprinting;
		
		private static readonly int verticalHash = Animator.StringToHash("Vertical");
		private static readonly int attackHash = Animator.StringToHash("Attack");
		
		#region Unity Methods

		private void Awake()
		{
			animator ??= GetComponent<Animator>();
		}

		private void Update()
		{
			if (animator == null) return;
			if (enemyComponent == null) return;
		
			float lerpTime = 1 - Mathf.Exp(-blendSharpness * Time.deltaTime);
			float verticalVelocity = Mathf.Clamp(_cachedSpeed, -1, 1);

			float currentVertical = animator.GetFloat(verticalHash);
			animator.SetFloat(verticalHash, Mathf.Lerp(currentVertical.ApproxEquals(0f, 0.01f) ? 0f : currentVertical, verticalVelocity, lerpTime));
		}

		private void OnEnable()
		{
			TickSystem.AddListener("Animator", CheckVelocity);
			enemyComponent.DamageComponent.OnDamageTarget += HandleAttack;
		}

		private void OnDisable()
		{
			TickSystem.RemoveListener("Animator", CheckVelocity);
			enemyComponent.DamageComponent.OnDamageTarget -= HandleAttack;
		}

		#endregion

		private void CheckVelocity()
		{
			if (animator == null) return;
			if (enemyComponent == null) return;
			
			float velocity = enemyComponent.PathfindingComponent.Pathfinder.velocity.magnitude * (_isSprinting ? 1f : 0.5f);
			float maxSpeed = enemyComponent.PathfindingComponent.Pathfinder.maxSpeed;
			_cachedSpeed =  velocity / maxSpeed;
		}

		private void HandleAttack(IDamageable target)
		{
			if (animator == null) return;
			
			animator.SetTrigger(attackHash);
		}
	}
}