using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Core.HealthSystem
{
	/// <summary>
	/// Class that listens to the events of a <see cref="HealthComponent"/>
	/// and invokes UnityEvents for each event.
	/// </summary>
	[RequireComponent(typeof(HealthComponent))]
	public class HealthComponentEvents : MonoBehaviour
	{
		#region Events

		[FoldoutGroup("Events", Order = 200)]
		public UnityEvent<HealthChangedArgs> OnHealthChanged;
		
		[FoldoutGroup("Events")]
		public UnityEvent<HealthChangedArgs> OnMaxHealthChanged;
		
		[FoldoutGroup("Events")]
		public UnityEvent<HealthChangedArgs> OnDamaged;
		
		[FoldoutGroup("Events")]
		public UnityEvent<HealthChangedArgs> OnHealed;
		
		[FoldoutGroup("Events")]
		public UnityEvent OnDie;
		
		#endregion
		
		[TitleGroup("Required Components", Order = 100)]
		[Required]
		[SerializeField]
		private HealthComponent healthComponent;
		
		#region Unity Methods

		private void Awake()
		{
			if (healthComponent == null)
			{
				healthComponent = GetComponent<HealthComponent>();
			}
		}

		private void OnEnable()
		{
			healthComponent.OnHealthChanged += OnHealthChanged.Invoke;
			healthComponent.OnMaxHealthChanged += OnMaxHealthChanged.Invoke;
			healthComponent.OnDamaged += OnDamaged.Invoke;
			healthComponent.OnHealed += OnHealed.Invoke;
			healthComponent.OnDie += OnDie.Invoke;
		}

		private void OnDisable()
		{
			healthComponent.OnHealthChanged -= OnHealthChanged.Invoke;
			healthComponent.OnMaxHealthChanged -= OnMaxHealthChanged.Invoke;
			healthComponent.OnDamaged -= OnDamaged.Invoke;
			healthComponent.OnHealed -= OnHealed.Invoke;
			healthComponent.OnDie -= OnDie.Invoke;
		}

		#endregion
	}
}