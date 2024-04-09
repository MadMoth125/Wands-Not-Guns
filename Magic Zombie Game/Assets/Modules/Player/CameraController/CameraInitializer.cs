using Cinemachine;
using ScriptExtensions;
using UnityEngine;

namespace Player
{
	public class CameraInitializer : MonoBehaviour
	{
		private Camera _mainCamera;
	
		#region Unity Methods

		private void Awake()
		{
			_mainCamera = Camera.main.OrNull();
			
			if (_mainCamera == null)
			{
				_mainCamera = new GameObject("Main Camera (Generated)")
					.GetOrAddComponent<AudioListener>()
					.GetOrAddComponent<Camera>();
			}
			
			_mainCamera.GetOrAddComponent<CinemachineBrain>();
		}

		#endregion
	}
}