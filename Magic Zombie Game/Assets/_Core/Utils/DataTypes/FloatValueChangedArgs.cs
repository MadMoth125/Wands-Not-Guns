using System;

namespace Core.Utils
{
	public class FloatValueChangedArgs
	{
		public FloatValueChangedArgs(float currentValue, float previousValue)
		{
			CurrentValue = currentValue;
			PreviousValue = previousValue;
		}

		public float CurrentValue { get; }

		public float PreviousValue { get; }

		public float Difference => Math.Abs(CurrentValue - PreviousValue);

		public float LargerValue => Math.Max(CurrentValue, PreviousValue);

		public float SmallerValue => Math.Min(CurrentValue, PreviousValue);
	}
}