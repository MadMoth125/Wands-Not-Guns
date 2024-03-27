namespace Core.Utils
{
	/* Exposed items:
	 *
	 * Properties:
	 * - bool HasChanged
	 * 
	 * public variables:
	 * - T current
	 * - T previous
	 *
	 * Methods:
	 * - void UpdateBothValues(T)
	 * - void UpdatePreviousValue()
	 * - void UpdateBothValues(T)
	 */
	
	/// <summary>
	/// Represents a pair of values, with the ability to compare the current value to the previous.
	/// </summary>
	/// <typeparam name="T">The type of values to be stored.</typeparam>
	public struct ValueChange<T>
	{
		/// <summary>
		/// The current value.
		/// </summary>
		public T current;
		
		/// <summary>
		/// The previous value.
		/// </summary>
		public T previous;
	
		/// <summary>
		/// Initializes a new instance of the <see cref="ValueChange{T}"/> struct with the specified initial value.
		/// </summary>
		/// <param name="initialValue">The initial value for both current and previous.</param>
		public ValueChange(T initialValue)
		{
			current = initialValue;
			previous = initialValue;
		}
		
		/// <summary>
		/// Assigns the same value to both <see cref="current"/> and <see cref="previous"/>.
		/// </summary>
		/// <param name="newValue">The new value for both current and previous.</param>
		public void UpdateBothValues(T newValue)
		{
			current = newValue;
			previous = current;
		}
		
		/// <summary>
		/// Returns true if <see cref="current"/> is different from <see cref="previous"/>.
		/// </summary>
		public bool HasChanged => !current.Equals(previous);
		
		/// <summary>
		/// Assigns the value of <see cref="current"/> to <see cref="previous"/>.
		/// </summary>
		public void UpdatePreviousValue() => previous = current;
	}
}