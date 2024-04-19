using System;
using ScriptExtensions;
using Sirenix.OdinInspector;
using StatSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
	public class GunAmmoComponent : MonoBehaviour, IGetAmmoComponent
	{
		public event Action OnReloadSuccess;
		public event Action OnReloadFailed;

		public Stat MaxAmmoStat => _maxAmmoStat;
	
		[SerializeField]
		private int ammo = 0; // Starting value, true value is stored in AmmoComponent
	
		[SerializeField]
		private int maxAmmo = 30; // Starting value, true value is stored in AmmoComponent
	
		[TitleGroup("Reserve Ammo")]
		[Tooltip("If true, the reserve ammo for the weapon is referenced from a separate '" + nameof(AmmoReserveComponent) + "' component.\n" +
		         "If false, an '" + nameof(AmmoReserveComponent) + "' component is added and used only for this component.")]
		[SerializeField]
		private bool useReferencedReserve = false;

		[TitleGroup("Reserve Ammo")]
		[ShowIf(nameof(useReferencedReserve))]
		[SerializeField]
		private AmmoReserveComponent ammoReserve;

		[TitleGroup("Reserve Ammo")]
		[HideIf(nameof(useReferencedReserve))]
		[SerializeField]
		private int reserveAmmo = 240; // Starting value, true value is stored in AmmoReserveComponent

		[TitleGroup("Reserve Ammo")]
		[HideIf(nameof(useReferencedReserve))]
		[SerializeField]
		private int maxReserveAmmo = 360; // Starting value, true value is stored in AmmoReserveComponent

		private Stat _maxAmmoStat;
		private AmmoComponent _ammoComponent;
	
		public void SetReserveAmmo(int amount)
		{
			ammoReserve.GetAmmoComponent().SetAmmo(amount);
		}
	
		public int GetReserveAmmo()
		{
			return ammoReserve.GetAmmoComponent().GetAmmo();
		}

		/// <summary>
		/// Reloads the weapon with ammo from the reserve, refilling the weapon's ammo to its maximum capacity.
		/// If there isn't enough to fully reload the weapon, the weapon is filled with the remaining ammo.
		/// If the reserve is empty, the reload fails.
		/// </summary>
		public void Reload()
		{
			// branch functionality based on whether the reserve is valid
			if (ammoReserve != null)
			{
				if (!CanReload())
				{
					OnReloadFailed?.Invoke();
					return;
				}
				
				int neededAmmo = _ammoComponent.MaxAmmo - _ammoComponent.Ammo;
				int ammoTaken = ammoReserve.GetAmmoFromReserve(neededAmmo);
				_ammoComponent.AddAmmo(ammoTaken);
				OnReloadSuccess?.Invoke();
			}
			else
			{
				// will always fail if there is no reserve component
				OnReloadFailed?.Invoke();
			}
		}

		/// <summary>
		/// Whether the weapon can be reloaded.
		/// Checks various conditions such as if the weapon is full, if the reserve is empty, etc.
		/// </summary>
		/// <returns>If the weapon can be reloaded.</returns>
		public bool CanReload()
		{
			if (ammoReserve == null) return false;
			if (_ammoComponent == null) return false;
			if (ammoReserve.GetAmmoComponent().GetAmmo() <= 0) return false;
			if (_ammoComponent.Ammo >= _ammoComponent.MaxAmmo) return false;

			return true;
		}
		
		public AmmoComponent GetAmmoComponent()
		{
			return _ammoComponent;
		}
		
		public AmmoComponent GetReserveAmmoComponent()
		{
			return ammoReserve.GetAmmoComponent();
		}

		#region Unity Methods

		private void OnValidate()
		{
			maxAmmo = Mathf.Max(1, maxAmmo);
			ammo = Mathf.Clamp(ammo, 0, maxAmmo);
			maxReserveAmmo = Mathf.Max(0, maxReserveAmmo);
			reserveAmmo = Mathf.Clamp(reserveAmmo, 0, maxReserveAmmo);
		}

		private void Awake()
		{
			_ammoComponent = new AmmoComponent(maxAmmo);
		
			if (ammo != 0)
			{
				_ammoComponent.SetAmmo(ammo);
			}
		
			_maxAmmoStat = new Stat(maxAmmo);

			if (!useReferencedReserve)
			{
				ammoReserve = this.GetOrAddComponent<AmmoReserveComponent>();
				ammoReserve.GetAmmoComponent().SetMaxAmmo(maxReserveAmmo, false);
				ammoReserve.GetAmmoComponent().SetAmmo(reserveAmmo);
			}
			else if (ammoReserve == null)
			{
				Debug.LogError($"No '{nameof(AmmoReserveComponent)}' reference found for ammo reserve, adding defaulted component.", this);
				ammoReserve = this.GetOrAddComponent<AmmoReserveComponent>();
				ammoReserve.GetAmmoComponent().SetMaxAmmo(0, true);
			}
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
}