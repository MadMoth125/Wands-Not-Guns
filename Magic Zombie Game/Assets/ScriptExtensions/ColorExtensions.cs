using UnityEngine;

namespace ScriptExtensions
{
	public class ColorExtensions
	{
		private static Color ClassColor => new Color(0.756f, 0.568f, 1f);
		
		private static Color StructColor => new Color(0.867f, 0.686f, 0.792f);
		
		private static Color VariableColor => new Color(0.741f, 0.741f, 0.741f);
		
		private static Color KeywordColor => new Color(0.423f, 0.584f, 0.921f);
		
		private static Color WarningColor => new Color(1f, 0.756f, 0.027f);
		
		private static Color ErrorColor => new Color(0.815f, 0.333f, 0.247f);
	}
}