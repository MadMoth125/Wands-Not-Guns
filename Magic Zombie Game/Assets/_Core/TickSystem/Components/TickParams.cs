using System;
using Sirenix.OdinInspector;

namespace Core.CustomTickSystem.Components
{
	/// <summary>
	/// Simple class to store various parameters related to a tick group.
	/// Most code is for inspector display purposes.
	/// </summary>
	[Serializable]
	public class TickParams
	{
		#region Constructors

		public TickParams()
		{
			name = defaultName;
			tickRate = defaultTickRate;
		}

		public TickParams(bool canRename, bool canDisable)
		{
			_canRename = canRename;
			_canDisable = canDisable;
		
			name = defaultName;
			tickRate = defaultTickRate;
		}

		#endregion

		#region Static Defaults

		public static string defaultName = "NewTickGroup";
		public static int defaultTickRate = 20;
		public static int minTickRate = 1;
		public static int maxTickRate = 120;

		#endregion

		#region Public Fields

		// tick name
		[VerticalGroup("Layer")]
		[EnableIf(nameof(CanEditName))]
		[HideLabel]
		public string name;

		// tick enabled
		[VerticalGroup("Layer")]
		[HorizontalGroup("Layer/Parameters", 100)]
		[EnableIf(nameof(CanEditEnabled))]
		[ToggleLeft]
		[LabelWidth(50)]
		public bool enabled = true;

		// tick rate
		[VerticalGroup("Layer")]
		[HorizontalGroup("Layer/Parameters")]
		[EnableIf(nameof(CanEditTickRate))]
		[PropertyRange(nameof(minTickRate), nameof(maxTickRate))]
	
		[SuffixLabel("tick/s", Icon = SdfIconType.Clock)]
		[LabelWidth(60)]
		public int tickRate;

		#endregion
	
		private bool _canRename = true;
		private bool _canDisable = true;
	
		private float _cachedTickInterval = 0f;
		private bool _calculateTickInterval = false;
	
		public float TickInterval()
		{
			if (!_calculateTickInterval) _cachedTickInterval = 1f / tickRate;
			return _cachedTickInterval;
		}

		#region Validation

		private bool CanEditName() => _canRename && enabled;

		private bool CanEditEnabled() => _canDisable;

		private bool CanEditTickRate() => enabled;

		#endregion
	}
}