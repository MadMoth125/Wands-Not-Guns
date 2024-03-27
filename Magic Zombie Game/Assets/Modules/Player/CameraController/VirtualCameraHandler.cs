using Cinemachine;
using UnityEngine;

public class VirtualCameraHandler : MonoBehaviour
{
	public Camera replacementCamera;
	public CinemachineVirtualCamera virtualCamera;
	
	private Camera mainCamera;
	
	#region Unity Methods

	private void Start()
	{
		HandleCameraReplacement();
	}

	#endregion
	
	private void HandleCameraReplacement()
	{
		mainCamera = Camera.main;
		
		replacementCamera.gameObject.SetActive(true);
		
		Debug.LogWarning("Main camera not found, replacing with replacement camera.", this);
		
		if (!replacementCamera.CompareTag("MainCamera"))
		{
			replacementCamera.tag = "MainCamera";
			Debug.LogWarning("Replacement camera is not tagged as MainCamera, tagging it as MainCamera.", this);
		}

		if (replacementCamera.gameObject.GetComponent<CinemachineBrain>() == null)
		{
			replacementCamera.gameObject.AddComponent<CinemachineBrain>();
			Debug.LogWarning("Replacement camera does not have a CinemachineBrain component, adding one.", this);
		}
	}
}