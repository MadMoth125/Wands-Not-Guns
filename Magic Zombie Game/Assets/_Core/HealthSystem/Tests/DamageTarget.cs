using UnityEngine;

namespace Core.HealthSystem
{
	public class DamageTarget : MonoBehaviour
	{
		public GameObject target;
	
		#region Unity Methods

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				target.GetComponent<IDamageable>().Damage(10f, this);
			}
			
			if (Input.GetKeyDown(KeyCode.E))
			{
				target.GetComponent<IHealable>().HealComplete(this);
			}
		}

		#endregion
	}
}