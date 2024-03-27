using Core.Utils;

namespace Core.HealthSystem
{
	public class HealthChangedArgs : FloatValueChangedArgs
	{
		public HealthChangedArgs(float currentValue, float previousValue) : base(currentValue, previousValue)
		{
		}
	}
}