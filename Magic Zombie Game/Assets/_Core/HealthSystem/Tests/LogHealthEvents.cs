using UnityEngine;

namespace Core.HealthSystem
{
	public class LogHealthEvents : MonoBehaviour
	{
		public void OnHealthChanged(HealthChangedArgs args)
		{
			Debug.Log($"Health changed: {args.PreviousValue} -> {args.CurrentValue}");
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
		
		}

		private void OnDisable()
		{
		
		}

		#endregion
	}
}