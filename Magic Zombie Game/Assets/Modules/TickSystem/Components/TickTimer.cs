namespace CustomTickSystem.Components
{
	/// <summary>
	/// Simple tick component that manages a timer for ticking at a set interval.
	/// </summary>
	public class TickTimer
	{
		#region Constructors

		public TickTimer()
		{
			_time = 0f;
		}
	
		public TickTimer(bool tickFirstFrame)
		{
			_time = tickFirstFrame ? float.MaxValue : 0f;
		}

		#endregion
	
		private float _time = 0f;
	
		public bool ShouldTick(float interval, float deltaTime)
		{
			if (_time >= interval)
			{
				_time = 0;
				return true;
			}
		
			_time += deltaTime;
			return false;
		}
	
		public void Reset()
		{
			_time = 0;
		}
	}
}