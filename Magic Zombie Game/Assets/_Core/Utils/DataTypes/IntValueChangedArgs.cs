using System;

namespace Core.Utils
{
	public class IntValueChangedArgs
	{
		public IntValueChangedArgs(int currentValue, int previousValue)
		{
			CurrentValue = currentValue;
			PreviousValue = previousValue;
		}

		public int CurrentValue { get; }

		public int PreviousValue { get; }

		public int Difference => Math.Abs(CurrentValue - PreviousValue);

		public int LargerValue => Math.Max(CurrentValue, PreviousValue);

		public int SmallerValue => Math.Min(CurrentValue, PreviousValue);
	}
}