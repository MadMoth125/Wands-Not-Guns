using UnityEngine;

namespace ScriptExtensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Returns the string as the given color.
		/// </summary>
		public static string Color(this string text, Color color)
		{
			return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
		}
		
		/// <summary>
		/// Returns the string as bold.
		/// </summary>
		public static string Bold(this string text)
		{
			return $"<b>{text}</b>";
		}
		
		/// <summary>
		/// Returns the string as italic.
		/// </summary>
		public static string Italic(this string text)
		{
			return $"<i>{text}</i>";
		}
		
		/// <summary>
		/// Returns the string as underlined.
		/// </summary>
		public static string Underline(this string text)
		{
			return $"<u>{text}</u>";
		}
		
		/// <summary>
		/// Returns the string as strikethrough.
		/// </summary>
		public static string Strikethrough(this string text)
		{
			return $"<s>{text}</s>";
		}
	}
}