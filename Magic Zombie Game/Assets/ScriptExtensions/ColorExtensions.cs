using UnityEngine;

namespace ScriptExtensions
{
	public static class ColorExtensions
	{
		public static Color ClassColor => new Color(0.756f, 0.568f, 1f);
		
		public static Color StructColor => new Color(0.867f, 0.686f, 0.792f);
		
		public static Color VariableColor => new Color(0.741f, 0.741f, 0.741f);
		
		public static Color KeywordColor => new Color(0.423f, 0.584f, 0.921f);
		
		public static Color WarningColor => new Color(1f, 0.756f, 0.027f);
		
		public static Color ErrorColor => new Color(0.815f, 0.333f, 0.247f);
	}
}