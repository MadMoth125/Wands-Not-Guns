using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.TickSystem
{
	[CreateAssetMenu(fileName = "TickSystemBackend", menuName = "Game/Tick System/Tick Backend")]
	public class TickSystemScriptableObject : ScriptableObject
	{
		public bool EnableUpdateTick => enableUpdateTick;
	
		public bool EnableFixedUpdateTick => enableFixedUpdateTick;
	
		public bool EnableLateUpdateTick => enableLateUpdateTick;

		#region Serialized Fields

		[BoxGroup("Required Assets")]
		[Required]
		[SerializeField] 
		private TickSystemParametersScriptableObject systemSettings;
		
		[BoxGroup("Tick System")]
		[TitleGroup("Tick System/Unity Tick")]
		[ToggleLeft]
		[Tooltip("Enable tick that matches Unity's Update() loop.")]
		[LabelText("Update")]
		[SerializeField]
		private bool enableUpdateTick = false;
	
		[BoxGroup("Tick System")]
		[TitleGroup("Tick System/Unity Tick")]
		[ToggleLeft]
		[Tooltip("Enable tick that matches Unity's FixedUpdate() loop.")]
		[LabelText("Fixed Update")]
		[SerializeField]
		private bool enableFixedUpdateTick = false;

		[BoxGroup("Tick System")]
		[TitleGroup("Tick System/Unity Tick")]
		[ToggleLeft]
		[Tooltip("Enable tick that matches Unity's LateUpdate() loop.")]
		[LabelText("Late Update")]
		[SerializeField]
		private bool enableLateUpdateTick = false;

		[BoxGroup("Tick System")]
		[TitleGroup("Tick System/Main Tick")]
		[PropertyRange("@TickGroup.tickRateMin", "@TickGroup.tickRateMax")]
		[LabelWidth(60)]
		[SuffixLabel("tick/s", Icon = SdfIconType.Clock)]
		[SerializeField]
		private int tickRate = 20;
		
		[BoxGroup("Tick System")]
		[PropertySpace(10, 0)]
		[LabelText("Groups")]
		[SerializeField]
		private List<TickGroup> tickGroups = new();

		#endregion
	
		private List<TickGroup> _combinedTicks;
		private List<TickGroup> _unityTicks;
	
		/// <summary>
		/// Initializes the tick system and creates the necessary tick layers.
		/// Required to be called before any listeners are added.
		/// </summary>
		public void Initialize()
		{
			_combinedTicks = new List<TickGroup>
			{
				new TickGroup(systemSettings.MainGroupName, tickRate),
			};
			_combinedTicks.AddRange(tickGroups);
		
			_unityTicks = new List<TickGroup>
			{
				new TickGroup(systemSettings.UpdateGroupName, 0, enableUpdateTick),
				new TickGroup(systemSettings.FixedUpdateGroupName, 0, enableFixedUpdateTick),
				new TickGroup(systemSettings.LateUpdateGroupName, 0, enableLateUpdateTick)
			};
		}
	
		/// <summary>
		/// Adds a listener to the specified tick layer.
		/// The layer name is case-insensitive, and the listener will be added to the first layer that matches the name.
		/// </summary>
		/// <param name="layerName">The name of the tick layer.</param>
		/// <param name="action">The listener to add.</param>
		public void AddListener(string layerName, Action action)
		{
			var layer = GetTickLayer(layerName);
			layer?.AddListener(action);
		}
	
		/// <summary>
		/// Removes a listener from the specified tick layer.
		/// The layer name is case-insensitive, and the listener will be removed from the first layer that matches the name.
		/// </summary>
		/// <param name="layerName">The name of the tick layer.</param>
		/// <param name="action">The listener to remove.</param>
		public void RemoveListener(string layerName, Action action)
		{
			var layer = GetTickLayer(layerName);
			layer?.RemoveListener(action);
		}

		/// <summary>
		/// Returns the tick layer with the specified name.
		/// </summary>
		/// <param name="layerName">The name of the tick layer.</param>
		public TickGroup GetTickLayer(string layerName)
		{
			return GetTickLayers().Find(l => StringsMatchIgnoreCaps(l.Name, layerName)) ??
			       GetUnityTickLayers().Find(l => StringsMatchIgnoreCaps(l.Name, layerName));
		}

		/// <summary>
		/// Returns all currently assigned tick layers.
		/// Adds a main tick layer if it doesn't exist in the list.
		/// </summary>
		public List<TickGroup> GetTickLayers()
		{
			return _combinedTicks;
		}
	
		/// <summary>
		/// Returns all currently assigned Unity tick layers.
		/// </summary>
		/// <returns></returns>
		public List<TickGroup> GetUnityTickLayers()
		{
			return _unityTicks;
		}

		/// <summary>
		/// Simple case-insensitive string comparison for layer names.
		/// </summary>
		private bool StringsMatchIgnoreCaps(string a, string b)
		{
			return string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
		}
	}
}