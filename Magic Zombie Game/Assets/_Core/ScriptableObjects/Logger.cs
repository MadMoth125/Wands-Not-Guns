using UnityEngine;

namespace Core.CustomDebugger
{
	[CreateAssetMenu(fileName = "Logger", menuName = "Debug/Logger")]
	public class Logger : ScriptableObject
	{
		public enum LogType
		{
			Info = 0,
			Warning = 1,
			Error = 2
		}

		public string Identifier => identifier;
		
		public bool Enabled => enabled;
		
		#region Serialized Fields

		[SerializeField]
		private string identifier = "Logger Instance";

		[SerializeField]
		private bool enabled = true;

		#endregion

		public void LogIfNull(object objRef, string message, UnityEngine.Object caller, LogType logType = LogType.Warning)
		{
			// only log if in editor
			#if UNITY_EDITOR
		
			// early return if disabled
			if (!enabled) return;
		
			// only log if object is null
			if (objRef == null)
			{
				Log(message, caller, logType);
			}
		
			#endif
		}
	
		public void LogConditional(bool condition, string message, UnityEngine.Object caller, LogType logType = LogType.Info)
		{
			// only log if in editor
			#if UNITY_EDITOR
		
			// early return if disabled
			if (!enabled) return;
		
			// only log if condition is true
			if (condition)
			{
				// no need to format, since it gets formatted in Log method
				Log(message, caller, logType);
			}
		
			#endif
		}
	
		public void Log(string message, UnityEngine.Object caller, LogType logType = LogType.Info)
		{
			// only log if in editor
			#if UNITY_EDITOR
		
			// early return if disabled
			if (!enabled) return;

			switch (logType)
			{
				default:
				case LogType.Info:
				{
					UnityEngine.Debug.Log(FormatMessage(message), caller);
					break;
				}
				case LogType.Warning:
				{
					UnityEngine.Debug.LogWarning(FormatMessage(message), caller);
					break;
				}
				case LogType.Error:
				{
					UnityEngine.Debug.LogError(FormatMessage(message), caller);
					break;
				}
			}
		
			#endif
		}
	
		private string FormatMessage(string message)
		{
			return $"{typeof(Logger)} - {identifier}: {message}";
		}
	}
}