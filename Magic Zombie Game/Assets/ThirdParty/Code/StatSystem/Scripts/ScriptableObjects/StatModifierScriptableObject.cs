using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StatSystem
{
	/// <summary>
	/// Custom scriptable object for wrapping stat modifiers in assets rather than in code.
	/// </summary>
	[CreateAssetMenu(fileName = "StatModifier", menuName = "Stat System/Stat Modifier")]
	public class StatModifierScriptableObject : ScriptableObject
	{
		#region Tooltips
		
		#pragma warning disable CS0414
		
		private static readonly string flatTooltip = "Modifier values are summed and directly added to the base value.\n" +
		                                             "Values are calculated <b>before</b> additive and multiplicative modifiers.\n\n" +
		                                             "Click for more info on how the calculation works.";
		private static readonly string flatEquationTooltip = "stat = stat + modifierSum\n\n" +
		                                                     "stat = 100\n" +
		                                                     "modifier = 10\n\n" +
		                                                     "<b>100 + 10 = 110</b>\n\n" +
		                                                     "stat = 110";

		private static readonly string additiveTooltip = "Modifier values are summed and multiply the base value by the sum.\n" +
		                                                 "Values are calculated <b>after</b> flat modifiers, but <b>before</b> multiplicative modifiers.\n\n" +
		                                                 "Click for more info on how the calculation works.";
		private static readonly string additiveEquationTooltip = "stat = stat x (1 + modifierSum)\n\n" +
		                                                         "stat = 50\n" +
		                                                         "modifier = 0.2\n\n" +
		                                                         "<b>50 x (1 + 0.2) = 60</b>\n\n" +
		                                                         "stat = 60";

		private static readonly string multiplicativeTooltip = "Modifiers are applied relative to the given stat value, compounding the effect of multiple modifiers multiplicatively.\n" +
		                                                       "Values are calculated <b>after</b> flat and additive modifiers.\n\n" +
		                                                       "Click for more info on how the calculation works.";
		private static readonly string multiplicativeEquationTooltip = "stat = stat x (1 + modifier) x (1 + modifier)...\n\n" +
		                                                               "stat = 100\n" +
		                                                               "modifierA = 0.1\n" +
		                                                               "modifierB = 0.2\n\n" +
		                                                               "<b>100 x (1 + 0.1) x (1 + 0.2) = 132</b>\n\n" +
		                                                               "stat = 132";

		#pragma warning restore CS0414
		
		#endregion

		// Multiple DetailedInfoBox attributes cannot be used on the same variable,
		// so we need to create separate, hidden, variables to hold the tooltips.
		#region Tooltip Holders

		#pragma warning disable CS0414
		
		[DetailedInfoBox("@flatTooltip",
			"@flatEquationTooltip",
			VisibleIf = "@modifierType == ModifierType.Flat")]
		[ShowIf("@modifierType == ModifierType.Flat", Animate = false)]
		[HideLabel]
		[ShowInInspector]
		[DisplayAsString]
		private readonly string _flatToolTipHolder = "";
		
		[DetailedInfoBox("@additiveTooltip",
			"@additiveEquationTooltip",
			VisibleIf = "@modifierType == ModifierType.Additive")]
		[ShowIf("@modifierType == ModifierType.Additive", Animate = false)]
		[HideLabel]
		[ShowInInspector]
		[DisplayAsString]
		private readonly string _additiveToolTipHolder = "";
		
		[DetailedInfoBox("@multiplicativeTooltip",
			"@multiplicativeEquationTooltip",
			VisibleIf = "@modifierType == ModifierType.Multiplicative")]
		[ShowIf("@modifierType == ModifierType.Multiplicative", Animate = false)]
		[HideLabel]
		[ShowInInspector]
		[DisplayAsString]
		private readonly string _multiplicativeToolTipHolder = "";
		
		#pragma warning restore CS0414
		
		#endregion
		
		[DisableInPlayMode]
		[SerializeField]
		private float modifierValue = 10;
	
		[DisableInPlayMode]
		[SerializeField]
		private ModifierType modifierType = ModifierType.Flat;

		private Modifier _modifier;
	
		/// <summary>
		/// Gets the modifier defined by the scriptable object.
		/// </summary>
		public Modifier GetModifier()
		{
			if (_modifier.Type != modifierType)
			{
				_modifier = new Modifier(modifierValue, modifierType, this);
			}
		
			if (Math.Abs(_modifier.Value - modifierValue) > float.Epsilon)
			{
				_modifier = new Modifier(modifierValue, modifierType, this);
			}
		
			return _modifier;
		}
		
		// Implicit conversion to Modifier when using the scriptable object in code.
		public static implicit operator Modifier(StatModifierScriptableObject scriptableObject)
		{
			return scriptableObject.GetModifier();
		}
	}
}