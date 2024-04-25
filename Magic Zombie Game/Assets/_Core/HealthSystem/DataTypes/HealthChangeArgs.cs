using Core.Utils;

namespace Core.HealthSystem
{
	public class HealthChangedArgs : FloatValueChangedArgs
	{
		public HealthChangedArgs(float currentValue, float previousValue) : base(currentValue, previousValue)
		{
		}
		
		public void Set(float currentValue, float previousValue)
		{
			CurrentValue = currentValue;
			PreviousValue = previousValue;
		}
		
		public static HealthChangedArgs Make(float currentValue, float previousValue, ref HealthChangedArgs args)
		{
			if (args == null)
			{
				args = new HealthChangedArgs(currentValue, previousValue);
			}
			else
			{
				args.Set(currentValue, previousValue);
			}
			
			return args;
		}
	}
}