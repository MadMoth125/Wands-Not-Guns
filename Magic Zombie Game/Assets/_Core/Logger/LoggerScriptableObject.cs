using System;
using ScriptExtensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.CustomDebugger
{
	[CreateAssetMenu(fileName = "Logger", menuName = "Debug/Logger")]
	public class LoggerScriptableObject : ScriptableObject
	{
		public string Identifier => identifier;
		
		public bool Enabled => enabled;
		
		#region Serialized Fields

		[TitleGroup("Logger Parameters", Order = -1)]
		[ValidateInput("@!String.IsNullOrEmpty(identifier) || !String.IsNullOrWhiteSpace(identifier)", "Identifier cannot be empty.")]
		[SerializeField]
		private string identifier = "Logger Instance";

		[TitleGroup("Logger Parameters", Order = -1)]
		[SerializeField]
		private bool enabled = true;

		#endregion

		/// <summary>
		/// Logs a message to the console if the object reference is null.
		/// Formats the message to include the identifier of the logger.
		/// </summary>
		/// <param name="objRef">The object reference to check.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="caller">The object that is calling the method.</param>
		/// <param name="loggerType">The type of log to output. (Info, Warning, etc.)</param>
		public void LogIfNull(object objRef, string message, UnityEngine.Object caller, LoggerType loggerType = LoggerType.Warning)
		{
			// only log if in editor
			#if UNITY_EDITOR
		
			// early return if disabled
			if (!enabled) return;
		
			// only log if object is null
			if (objRef == null)
			{
				Log(message, caller, loggerType);
			}
		
			#endif
		}
	
		/// <summary>
		/// Logs a message to the console if the condition is true.
		/// Formats the message to include the identifier of the logger.
		/// </summary>
		/// <param name="condition">The condition to check.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="caller">The object that is calling the method.</param>
		/// <param name="loggerType">The type of log to output. (Info, Warning, etc.)</param>
		public void LogConditional(bool condition, string message, UnityEngine.Object caller, LoggerType loggerType = LoggerType.Info)
		{
			// only log if in editor
			#if UNITY_EDITOR
		
			// early return if disabled
			if (!enabled) return;
		
			// only log if condition is true
			if (condition)
			{
				// no need to format, since it gets formatted in Log method
				Log(message, caller, loggerType);
			}
		
			#endif
		}
	
		/// <summary>
		/// Logs a message to the console.
		/// Formats the message to include the identifier of the logger.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="caller">The object that is calling the method.</param>
		/// <param name="loggerType">The type of log to output. (Info, Warning, etc.)</param>
		public void Log(string message, UnityEngine.Object caller, LoggerType loggerType = LoggerType.Info)
		{
			// only log if in editor
			#if UNITY_EDITOR
		
			// early return if disabled
			if (!enabled) return;

			switch (loggerType)
			{
				default:
				case LoggerType.Info:
				{
					UnityEngine.Debug.Log(FormatMessage(message), caller);
					break;
				}
				case LoggerType.Warning:
				{
					UnityEngine.Debug.LogWarning(FormatMessage(message), caller);
					break;
				}
				case LoggerType.Error:
				{
					UnityEngine.Debug.LogError(FormatMessage(message), caller);
					break;
				}
			}
		
			#endif
		}
	
		private string FormatMessage(string message)
		{
			return $"{identifier.Color(ColorExtensions.ClassColor)}: " + message;
		}
	}
}