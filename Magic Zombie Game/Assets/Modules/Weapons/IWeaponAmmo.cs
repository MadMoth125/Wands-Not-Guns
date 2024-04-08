
public interface IWeaponAmmo
{
	/// <summary>
	/// The current ammo of the weapon.
	/// </summary>
	public int GetAmmo();
	
	/// <summary>
	/// The maximum ammo the weapon can hold.
	/// </summary>
	public int GetMaxAmmo();
	
	/// <summary>
	/// The amount of ammo reserved for the weapon.
	/// </summary>
	public int GetReservedAmmo();
	
	/// <summary>
	/// Sets the amount of ammo in the weapon.
	/// Clamped between 0 and the weapon's max ammo.
	/// </summary>
	/// <param name="amount">The new ammo count.</param>
	public void SetAmmo(int amount);
	
	/// <summary>
	/// Sets the maximum amount of ammo the weapon can hold.
	/// </summary>
	/// <param name="amount">The new max ammo count.</param>
	/// <param name="fullAmmo">Should the ammo be set to the new maximum ammo.</param>
	public void SetMaxAmmo(int amount, bool fullAmmo);
	
	/// <summary>
	/// Sets the ammo reserve amount of the weapon.
	/// </summary>
	/// <param name="amount">The new ammo reserve amount.</param>
	public void SetReservedAmmo(int amount);
	
	/// <summary>
	/// Adds ammo to the weapon.
	/// </summary>
	/// <param name="amount">The ammo to add.</param>
	public void AddAmmo(int amount);
	
	/// <summary>
	/// Removes ammo from the weapon.
	/// </summary>
	/// <param name="amount">The ammo to remove.</param>
	public void RemoveAmmo(int amount);
	
	/// <summary>
	/// The current ammo of the weapon as a normalized value between 0 and 1.
	/// </summary>
	public float GetAmmoRatio();
}