using UnityEngine;
using Object = UnityEngine.Object;

public interface IWeaponProperties: IWeaponAmmo, IReloadable
{
	/// <summary>
	/// The Object that owns this <see cref="IWeaponProperties"/>.
	/// </summary>
	/// <returns>The Object owner.</returns>
	public Object GetOwner();
	
	/// <summary>
	/// Sets the Object that owns this <see cref="IWeaponProperties"/>.
	/// </summary>
	/// <param name="owner">The Object owner.</param>
	public void SetOwner(Object owner);
	
	/// <summary>
	/// Gets the <see cref="WeaponData"/> of the weapon.
	/// </summary>
	public WeaponData GetWeaponData();
	
	/// <summary>
	/// Sets the <see cref="WeaponData"/> of the weapon.
	/// </summary>
	/// <param name="weaponData">The new <see cref="WeaponData"/> value.</param>
	public void SetWeaponData(WeaponData weaponData);
	
	/// <summary>
	/// Resets the <see cref="WeaponData"/> of the weapon to its 'default' values.
	/// </summary>
	public void ResetWeaponData();
	
	/// <summary>
	/// Gets the damage of the weapon at a given distance.
	/// If the weapon has no damage falloff, the damage will be the same at all distances.
	/// </summary>
	/// <param name="distance">The distance from the target.</param>
	public float GetDamage(float distance);
	
	/// <summary>
	/// Sets the flat damage value of the weapon.
	/// </summary>
	/// <param name="damage">The new damage value.</param>
	public void SetDamage(float damage);

	/// <summary>
	/// Sets the damage falloff values for near and far target distances.
	/// </summary>
	/// <param name="nearDamage">The damage value at the near distance.</param>
	/// <param name="farDamage">The damage value at the far distance.</param>
	public void SetDamage(float nearDamage, float farDamage);
	
	/// <summary>
	/// Sets the damage falloff distances for near and far target distances.
	/// </summary>
	/// <param name="nearDistance">The distance at which the damage falloff begins.</param>
	/// <param name="farDistance">The distance at which the damage falloff ends.</param>
	public void SetFalloffDistances(float nearDistance, float farDistance);
}