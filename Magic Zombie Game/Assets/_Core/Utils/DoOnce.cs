using System;

namespace Core.Utils
{
	/* Exposed items:
	 *
	 * public variables:
	 * - bool startClosed
	 * 
	 * Methods:
	 * - void Do(Action)
	 * - void Reset()
	 */
	
	/// <summary>
	/// A struct meant to be used to execute a block of code only once.
	/// Can be manually reset to execute the block of code again.
	/// </summary>
	public struct DoOnce
	{
		public DoOnce(bool startClosed = false)
		{
			_hasExecuted = startClosed;
		}
	
		private bool _hasExecuted;

		/* Example usage:
		 * doOnce.Do(() =>
		 * {
		 *	Debug.Log("This will be executed only once.");
		 * });
		 */
		public void Do(Action action)
		{
			if (!_hasExecuted)
			{
				action.Invoke();
				_hasExecuted = true;
			}
		}

		public void Reset()
		{
			_hasExecuted = false;
		}
	}
}