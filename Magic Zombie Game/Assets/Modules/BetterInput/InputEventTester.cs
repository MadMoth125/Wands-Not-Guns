using UnityEngine;
using UnityEngine.Serialization;

namespace GameInput
{
	public class InputEventTester : MonoBehaviour
	{
		// [FormerlySerializedAs("inputParserAsset")] public PlayerInputAsset playerInputAsset;
		
		#region Unity Methods

		private void Start()
		{
		
		}

		private void Update()
		{
		
		}

		private void OnEnable()
		{
			// playerInputAsset.EnableInput(true);
			
			// inputParserAsset.OnMove += OnMove;
		}

		private void OnDisable()
		{
			// playerInputAsset.DisableInput();
		}

		#endregion
	}
}