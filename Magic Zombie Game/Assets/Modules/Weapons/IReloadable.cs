
public interface IReloadable
{
	/// <summary>
	/// Reloads the weapon.
	/// </summary>
	public void Reload();
	
	/// <summary>
	/// Whether the weapon can be reloaded.
	/// </summary>
	public bool CanReload();
}