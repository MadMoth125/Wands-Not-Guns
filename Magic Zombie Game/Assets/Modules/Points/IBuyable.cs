namespace Points
{
	public interface IBuyable
	{
		/// <summary>
		/// Buy the object.
		/// </summary>
		/// <param name="buyer">The point system that will buy the object.</param>
		/// <returns>Whether the object was bought or not.</returns>
		public bool Buy(IGetPointSystem buyer);
	
		/// <summary>
		/// Get the cost of the object.
		/// </summary>
		public int GetCost();
	}
}