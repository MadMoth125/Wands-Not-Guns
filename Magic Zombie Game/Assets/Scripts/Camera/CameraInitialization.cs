using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Core
{
	public static class CameraInitialization
	{
		private static Camera mainCamera;
		
		private static void ManageCameraInitialization()
		{
			mainCamera = Camera.main;
			
			if (mainCamera == null)
			{
				mainCamera = new GameObject("Main Camera (replacement)")
					.AddComponent<CinemachineBrain>().gameObject
					.AddComponent<AudioListener>().gameObject
					// .AddComponent<UniversalAdditionalCameraData>()
					.AddComponent<Camera>();
				mainCamera.gameObject.AddComponent<CinemachineBrain>();
				mainCamera.tag = "MainCamera";
				// mainCamera.gameObject.SetActive(true);
				Debug.LogWarning("Main camera not found, replacing with replacement camera.", mainCamera);
			}
			else
			{
				
			}
		}
	}
}