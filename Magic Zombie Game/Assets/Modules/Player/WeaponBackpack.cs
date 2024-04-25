using System;
using System.Collections.Generic;
using System.Linq;
using Core.Owning;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons;

namespace Player
{
	public class WeaponBackpack : MonoBehaviour, IOwnable<PlayerComponent>
	{
		#region Events

		public event Action<GunComponent> OnGunAdded;
		public event Action<GunComponent> OnGunRemoved;
		public event Action<GunComponent> OnGunEquipped;
		public event Action<GunComponent> OnGunUnequipped;

		#endregion
	
		public int CurrentGunIndex => _currentGunIndex;
	
		#region Fields

		[TitleGroup("Weapon Backpack Settings")]
		[SerializeField]
		private int maxGuns = 3;

		[TitleGroup("Weapon Backpack Settings")]
		[Tooltip("If true, the first gun in the backpack will be equipped on start.")]
		[SerializeField]
		private bool autoEquipOnStart = true;
	
		[TitleGroup("Weapon Backpack Settings")]
		[Tooltip("The starting gun to have equipped.")]
		[PropertyRange(1, nameof(GetInspectorWeaponCount))]
		[SerializeField]
		private int selectedGun = 1;

		[TitleGroup("Weapon Backpack Settings")]
		[AssetSelector(Paths = "Assets/Modules/BetterWeapons", Filter = "t:Prefab", FlattenTreeView = true)]
		[RequiredListLength(0, "@maxGuns")]
		[SerializeField]
		private List<GunComponent> equippedGuns = new List<GunComponent>();

		[TitleGroup("Attachment Settings")]
		[Tooltip("Meant to reference the root GameObject of the player.")]
		[Required]
		[SerializeField]
		private Transform playerRoot;

		[TitleGroup("Attachment Settings")]
		[Tooltip("Meant to contain a reference to the root of the characters 'skeleton' or 'model' hierarchy. " +
		         "This reference is passed to the weapon so it cn attach to the appropriate body part.")]
		[Required]
		[SerializeField]
		private Transform characterRoot;
	
		#endregion
	
		private int _currentGunIndex = -1;
		private PlayerComponent _owner;
		private InventoryComponent<GunComponent> _gunInventory;
	
		/// <summary>
		/// Gets the number of guns currently in the backpack.
		/// </summary>
		/// <returns></returns>
		public int GetWeaponCount()
		{
			return _gunInventory.GetItemCount();
		}

		/// <summary>
		/// Gets the currently equipped gun.
		/// </summary>
		/// <returns>Either the equipped gun or null if no gun is equipped.</returns>
		public GunComponent GetCurrentGun()
		{
			if (_currentGunIndex == -1)
			{
				Debug.LogWarning("No gun is currently equipped.");
				return null;
			}
		
			return _gunInventory.Items[_currentGunIndex];
		}
	
		/// <summary>
		/// Gets the gun at the specified index.
		/// </summary>
		/// <param name="index">The index of the gun to get.</param>
		/// <returns>Either the gun at the specified index or null if the index is out of range.</returns>
		public GunComponent GetGunAtIndex(int index)
		{
			if (_gunInventory.Items.InvalidIndex(index))
			{
				Debug.LogError($"Cannot get gun at index {index}. Index out of range.");
				return null;
			}
		
			return _gunInventory.Items[index];
		}

		/// <summary>
		/// Gets the gun at the specified index, if the index is out of range,
		/// it will be clamped to the nearest valid index.
		/// e.g. if the index is -1, it will return the first gun,
		/// if the index is 100, it will return the last gun, regardless of the actual count.
		/// </summary>
		/// <param name="index">The index of the gun to get.</param>
		/// <returns>The gun at the specified (or nearest) index.</returns>
		public GunComponent GetGunAtIndexSafe(int index)
		{
			index = Mathf.Clamp(index, 0, GetWeaponCount() - 1);
			return _gunInventory.Items[index];
		}
	
		/// <summary>
		/// Gets every currently equipped gun in the backpack.
		/// </summary>
		/// <returns>A read-only list of all equipped guns.</returns>
		public IReadOnlyList<GunComponent> GetEquippedGuns()
		{
			return _gunInventory.Items;
		}

		/// <summary>
		/// Adds the provided gun to the backpack.
		/// </summary>
		/// <param name="gun">The gun to add to the backpack.</param>
		/// <param name="equip">If true, the gun will be equipped immediately.</param>
		public void AddGun(GunComponent gun, bool equip)
		{
			if (_gunInventory.IsFull)
			{
				Debug.LogWarning($"Cannot add gun, the backpack is full. Max guns: {maxGuns}");
				return;
			}

			if (gun == null)
			{
				Debug.LogError("Cannot add gun, reference is null.");
				return;
			}

			GunComponent spawnedGun = InitializeGun(gun);
			if (spawnedGun != null)
			{
				spawnedGun.gameObject.SetActive(false);
				_gunInventory.AddItem(spawnedGun);
				OnGunAdded?.Invoke(spawnedGun);
			
				if (equip) EquipGun(spawnedGun);
			}
		}
		
		/// <summary>
		/// Adds the provided gun to the backpack at the specified index.
		/// </summary>
		/// <param name="gun">the gun to add to the backpack.</param>
		/// <param name="index">The index to add the gun at.</param>
		/// <param name="equip">If true, the gun will be equipped immediately.</param>
		public void AddGunAtIndex(GunComponent gun, int index, bool equip)
		{
			if (_gunInventory.IsFull)
			{
				Debug.LogWarning($"Cannot add gun, the backpack is full. Max guns: {maxGuns}");
				return;
			}

			if (gun == null)
			{
				Debug.LogError("Cannot add gun, reference is null.");
				return;
			}

			GunComponent spawnedGun = InitializeGun(gun);
			if (spawnedGun != null)
			{
				spawnedGun.gameObject.SetActive(false);
				_gunInventory.AddItemAt(index, spawnedGun);
				OnGunAdded?.Invoke(spawnedGun);
			
				if (equip) EquipGun(spawnedGun);
			}
		}
	
		/// <summary>
		/// Removes the provided gun from the backpack.
		/// If the removed gun is currently equipped, it will be un-equipped in the process.
		/// </summary>
		/// <param name="gun">The gun to remove from the backpack.</param>
		public void RemoveGun(GunComponent gun)
		{
			if (gun == null)
			{
				Debug.LogError("Cannot remove gun, reference is null.");
				return;
			}

			if (!_gunInventory.Items.Contains(gun))
			{
				Debug.LogWarning("Cannot remove gun, it is not in backpack.");
				return;
			}
		
			if (_currentGunIndex == _gunInventory.GetIndexOfItem(gun))
			{
				UnEquipCurrentGun();
			}
		
			_gunInventory.RemoveItem(gun);
			OnGunRemoved?.Invoke(gun);
			Destroy(gun);
		}
	
		public void RemoveGunAtIndex(int index)
		{
			if (_gunInventory.GetItemCount() == 0)
			{
				Debug.LogWarning("Cannot remove gun, no guns are equipped.");
				return;
			}
		
			if (!_gunInventory.Items.ValidIndex(index))
			{
				Debug.LogError($"Cannot remove gun at index {index}. Index out of range.");
				return;
			}
		
			GunComponent removedGun = _gunInventory.Items[index];
			_gunInventory.RemoveItemAt(index);
			OnGunRemoved?.Invoke(removedGun);
			Destroy(removedGun);
		}

		/// <summary>
		/// Equips the provided gun, if the gun is not in the backpack, it will not be equipped.
		/// A null gun will un-equip any currently equipped guns.
		/// </summary>
		/// <param name="gun">The gun to equip.</param>
		public void EquipGun(GunComponent gun)
		{
			if (gun == null)
			{
				Debug.LogError("Cannot equip gun, reference is null.");
				return;
			}
		
			if (!_gunInventory.ContainsItem(gun))
			{
				Debug.LogWarning("Cannot equip gun, it is not in backpack.");
				return;
			}
		
			// If the gun is not in the backpack, use "invalid" index.
			EquipGunAtIndex(_gunInventory.GetIndexOfItem(gun));
		}
	
		public void EquipGunAtIndex(int index)
		{
			if (_gunInventory.GetItemCount() == 0)
			{
				Debug.LogWarning("Cannot equip gun, no guns are equipped.");
				return;
			}

			if (_gunInventory.Items.InvalidIndex(index))
			{
				Debug.LogError($"Cannot equip gun at index {index}. Index out of range.");
				return;
			}

			if (_currentGunIndex == index)
			{
				Debug.Log($"Gun at index {index} is already equipped.");
				return;
			}

			UnEquipCurrentGun();
		
			_currentGunIndex = index;
			_gunInventory.Items[_currentGunIndex].gameObject.SetActive(true);
			OnGunEquipped?.Invoke(_gunInventory.Items[_currentGunIndex]);
		}
	
		/// <summary>
		/// Un-equips the currently equipped gun.
		/// Since the backpack can only have one gun equipped at a time,
		/// this is the only way to un-equip a gun.
		/// </summary>
		public void UnEquipCurrentGun()
		{
			if (_gunInventory.GetItemCount() == 0)
			{
				Debug.LogWarning("Cannot un-equip gun, no guns are equipped.");
				return;
			}
			
			if (_currentGunIndex == -1)
			{
				Debug.Log("No gun is currently equipped.");
				return;
			}
		
			int previousIndex = _currentGunIndex;
			_currentGunIndex = -1;
			_gunInventory.Items[previousIndex].gameObject.SetActive(false);
			OnGunUnequipped?.Invoke(_gunInventory.Items[previousIndex]);
		}
	
		#region Owner Methods

		public PlayerComponent GetOwner()
		{
			return _owner;
		}

		public void SetOwner(PlayerComponent owner)
		{
			_owner = owner;
		}

		#endregion
	
		#region Unity Methods

		private void OnValidate()
		{
			maxGuns = Mathf.Max(1, maxGuns);
			selectedGun = Mathf.Min(selectedGun, maxGuns);
		}

		private void Start()
		{
			// Initialize the gun inventory with the equipped guns.
			var spawnedGuns = InitializeGunCollection(equippedGuns);
			_gunInventory = new InventoryComponent<GunComponent>(spawnedGuns);
		
			// Equip the selected gun on start.
			if (autoEquipOnStart)
			{
				EquipGunAtIndex(selectedGun - 1);
			}

			return;

			IEnumerable<GunComponent> InitializeGunCollection(IList<GunComponent> gunCollection)
			{
				int count = Mathf.Min(gunCollection.Count, maxGuns);
				for (int i = 0; i < count; i++)
				{
					if (equippedGuns[i] == null) continue;
					GunComponent spawnedGun = InitializeGun(equippedGuns[i]);
					if (spawnedGun != null)
					{
						spawnedGun.gameObject.SetActive(false);
						yield return spawnedGun;
						OnGunAdded?.Invoke(spawnedGun);
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// Handles instantiating and initializing the provided gun asset.
		/// </summary>
		/// <param name="gun">The gun asset to instantiate and initialize.</param>
		/// <returns>The initialized gun component.</returns>
		private GunComponent InitializeGun(GunComponent gun)
		{
			if (gun == null)
			{
				Debug.LogError("Cannot initialize gun, reference is null.");
				return null;
			}
		
			var newGun = Instantiate(gun, _owner.transform.position, Quaternion.identity);
			newGun.SetOwner(_owner.gameObject);
			newGun.SetWeaponRootCopyTarget(playerRoot);
			newGun.SetWeaponVisualsCopyTarget(characterRoot);
		
			return newGun;
		}

		private int GetInspectorWeaponCount()
		{
			return equippedGuns.Count == 0 ? 1 : equippedGuns.Count;
		}
	}
}