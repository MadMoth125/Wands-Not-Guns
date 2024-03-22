using UnityEngine;

namespace CustomTickSystem.Example
{
	public class TickTester : MonoBehaviour
	{
		#region Unity Methods

		private void OnEnable()
		{
			TickSystem.AddListener("Main", TestFunc);
		}

		private void OnDisable()
		{
			TickSystem.RemoveListener("Main", TestFunc);
		}

		#endregion

		private void TestFunc()
		{
			Debug.Log("Main Ticked...");
		}
	}
}