using Sirenix.OdinInspector;
using StatSystem;
using UnityEngine;
using UnityEngine.Serialization;

public class GunAmmoComponent : MonoBehaviour, IGetAmmoComponent
{
	public AmmoReserveComponent AmmoReserve => ammoReserve;
	
	[SerializeField]
	private int ammo = 0; // Starting value, true value is stored in AmmoComponent
	
	[SerializeField]
	private int maxAmmo = 30; // Starting value, true value is stored in AmmoComponent
	
	[TitleGroup("Reserve Ammo")]
	[Tooltip("If true, the reserve ammo for the weapon is referenced from a '" + nameof(AmmoReserveComponent) + "' component.\n" +
	         "If false, the reserve ammo is stored locally and handled in this component.")]
	[SerializeField]
	private bool useReferencedReserve = false;
	
	[TitleGroup("Reserve Ammo")]
	[HideIf(nameof(useReferencedReserve))]
	[SerializeField]
	private int reserveAmmo = 240; // Starting value, true value is stored in AmmoComponent
	
	[TitleGroup("Reserve Ammo")]
	[ShowIf(nameof(useReferencedReserve))]
	[SerializeField]
	private AmmoReserveComponent ammoReserve;
	
	private Stat _maxAmmoStat;
	private AmmoComponent _ammoComponent;
	
	public void SetReserveAmmo(int amount)
	{
		if (useReferencedReserve)
		{
			Debug.LogWarning("Cannot set reserve ammo when using referenced reserve.");
			return;
		}
		
		reserveAmmo = Mathf.Max(0, amount);
	}
	
	public int GetReserveAmmo()
	{
		if (useReferencedReserve)
		{
			return ammoReserve.GetAmmoComponent().GetAmmo();
		}
		
		return reserveAmmo;
	}

	public AmmoComponent GetAmmoComponent()
	{
		return _ammoComponent;
	}

	#region Unity Methods

	private void Awake()
	{
		_ammoComponent = new AmmoComponent(maxAmmo);
		
		if (ammo != 0)
		{
			_ammoComponent.SetAmmo(ammo);
		}
		
		_maxAmmoStat = new Stat(maxAmmo);
	}

	private void OnEnable()
	{
		_maxAmmoStat.ValueChanged += OnMaxAmmoStatChanged;
	}

	private void OnDisable()
	{
		_maxAmmoStat.ValueChanged -= OnMaxAmmoStatChanged;
	}

	#endregion

	private void OnMaxAmmoStatChanged()
	{
		_ammoComponent.SetMaxAmmo((int)_maxAmmoStat.Value, false);
	}
}