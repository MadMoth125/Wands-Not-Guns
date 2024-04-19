using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

public class WeaponBackpack : MonoBehaviour
{
	public int CurrentGunIndex => _currentGunIndex;
	
	#region Fields

	[SerializeField]
	private int maxGuns = 3;

	[Tooltip("The starting gun to have equipped.")]
	[PropertyRange(1, nameof(GetWeaponCount))]
	[SerializeField]
	private int selectedGun = 1;

	[InfoBox("Currently more than 3 guns equipped, exceeding the limit.\n" +
	         "Any additional guns will be ignored as if they didn't exist.",
		InfoMessageType.Warning, VisibleIf = "@equippedGuns.Count > maxGuns")]
	[AssetSelector(Filter = "t:Prefab", FlattenTreeView = true)]
	[SerializeField]
	private List<GunComponent> equippedGuns = new List<GunComponent>();

	#endregion
	
	private int _currentGunIndex = 0;
	private readonly List<GunComponent> _equippedGuns = new List<GunComponent>();
	
	public int GetWeaponCount()
	{
		return Mathf.Min(equippedGuns.Count, maxGuns);
	}

	public GunComponent GetGun(int index)
	{
		if (index < 0 || index >= equippedGuns.Count)
		{
			Debug.LogError($"Cannot return gun at index {index}. Index out of range.");
			return null;
		}
		
		_currentGunIndex = index;
		return _equippedGuns[index];
	}
	
	public IReadOnlyList<GunComponent> GetEquippedGuns()
	{
		return _equippedGuns;
	}

	#region Unity Methods

	private void OnValidate()
	{
		maxGuns = Mathf.Max(1, maxGuns);
		selectedGun = Mathf.Min(selectedGun, maxGuns);
	}

	private void Awake()
	{
		_currentGunIndex = selectedGun - 1;
		InitializeGunCollection(equippedGuns);

		return;

		void InitializeGunCollection(IList<GunComponent> gunCollection)
		{
			gunCollection.Clear();
			
			for (int i = 0; i < Mathf.Min(equippedGuns.Count, maxGuns); i++)
			{
				GunComponent gun = equippedGuns[i];
				if (gun == null) continue;
				gunCollection.Add(Instantiate(gun));

				gun.SetOwner(gameObject);
			}
		}
	}

	private void Update()
	{
		
	}

	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		
	}

	#endregion
}