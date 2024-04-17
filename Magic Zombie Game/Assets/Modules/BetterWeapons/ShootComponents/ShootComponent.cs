using System;
using Mr_sB.UnityTimer;
using UnityEngine;

[DisallowMultipleComponent]
public class ShootComponent : MonoBehaviour
{
	public event Action OnShoot;
	
	[SerializeField]
	private int fireRate = 5;

	[SerializeField]
	private bool isAutomatic = true;
	
	private FullAutoShootComponent _fullAutoShootComponent;
	private SemiAutoShootComponent _semiAutoShootComponent;
	
	public void StartShoot()
	{
		if (isAutomatic) _fullAutoShootComponent.BeginFire();
		else _semiAutoShootComponent.BeginFire();
	}
	
	public void StopShoot()
	{
		if (isAutomatic) _fullAutoShootComponent.EndFire();
		// SemiAuto doesn't need to be stopped
	}
	
	#region Unity Methods

	private void Awake()
	{
		_fullAutoShootComponent = new FullAutoShootComponent(1f / fireRate, HandleFire);
		_semiAutoShootComponent = new SemiAutoShootComponent(1f / fireRate, HandleFire);
	}

	private void OnDestroy()
	{
		_fullAutoShootComponent?.Dispose();
		_semiAutoShootComponent?.Dispose();
	}

	#endregion
	
	private void HandleFire()
	{
		OnShoot?.Invoke();
	}
}