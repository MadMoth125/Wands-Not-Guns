using System;
using System.Collections;
using System.Collections.Generic;
using Core.CustomTickSystem.Components;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Core.CustomTickSystem
{
	[CreateAssetMenu(fileName = "TickSystemAsset", menuName = "ScriptableObject/TickSystemAsset")]
	public class TickSystemAsset : ScriptableObject
	{
		[LabelText("Update Tick")]
		[ToggleLeft]
		public bool useUnityUpdate = true;
	
		[LabelText("Fixed Update Tick")]
		[ToggleLeft]
		public bool useUnityFixedUpdate = false;
	
		[LabelText("Late Update Tick")]
		[ToggleLeft]
		public bool useUnityLateUpdate = false;
	
		[InlineProperty]
		[HideLabel]
		public TickLayer mainTickGroup = new("Main", false);
	
		[InfoBox("Please ensure that all tick groups have unique group names. If not, listeners will only be able to subscribe to the first group with that given name.",
			InfoMessageType.Warning, VisibleIf = nameof(CheckForDuplicateTickNames))]
		[InfoBox("Please ensure that all tick group names are not empty and contain no spaces.",
			InfoMessageType.Warning, VisibleIf = nameof(CheckForInvalidTickNames))]
		[OnCollectionChanged(nameof(HandleNameUpdate))] // editor-only attribute
		public List<TickLayer> tickGroups = new();

		private readonly List<TickLayer> _unityTickGroups = new()
		{
			new TickLayer("Update", false),
			new TickLayer("FixedUpdate", false),
			new TickLayer("LateUpdate", false),
		};

		#region Getters

		public TickLayer GetMainTickGroup() => mainTickGroup;

		public TickLayer GetUpdateTickGroup() => _unityTickGroups[0];

		public TickLayer GetFixedUpdateTickGroup() => _unityTickGroups[1];

		public TickLayer GetLateUpdateTickGroup() => _unityTickGroups[2];

		public List<TickLayer> GetCustomTickGroups() => tickGroups;
		
		/// <summary>
		/// Returns the main tick group and any custom defined tick groups.
		/// </summary>
		/// <returns></returns>
		[Obsolete("Use either 'GetAllTickGroups()' or 'GetCustomTickGroups()' instead.")]
		public IEnumerable<TickLayer> GetTickGroups()
		{
			for (int i = 0; i < tickGroups.Count + 1; i++)
			{
				yield return i < tickGroups.Count ? tickGroups[i] : mainTickGroup;
			}
		}

		/// <summary>
		/// Returns all tick groups shown in the asset.
		/// (Main, Unity, and any custom tick groups
		/// </summary>
		public IEnumerable<TickLayer> GetAllTickGroups()
		{
			for (int i = 0; i < tickGroups.Count + _unityTickGroups.Count + 1; i++)
			{
				if (i < tickGroups.Count)
				{
					yield return tickGroups[i];
				}
				else if (i < tickGroups.Count + _unityTickGroups.Count)
				{
					yield return _unityTickGroups[i - tickGroups.Count];
				}
				else
				{
					yield return mainTickGroup;
				}
			}
		}

		#endregion

		#region Validation
	
		// loop through all tick groups and check for duplicate tick names
		private bool CheckForDuplicateTickNames()
		{
			#if UNITY_EDITOR
		
			for (int i = 0; i < tickGroups.Count; i++)
			{
				for (int j = 0; j < tickGroups.Count; j++)
				{
					if (i == j) continue;
					if (tickGroups[i].parameters.name == tickGroups[j].parameters.name)
					{
						return true;
					}
				}
			
				#endif
			}

			return false;
		}
	
		// loop through all tick groups and check for invalid tick names
		private bool CheckForInvalidTickNames()
		{
			#if UNITY_EDITOR
		
			for (int i = 0; i < tickGroups.Count; i++)
			{
				if (string.IsNullOrEmpty(tickGroups[i].parameters.name) ||
				    string.IsNullOrWhiteSpace(tickGroups[i].parameters.name) ||
				    tickGroups[i].parameters.name.Contains(' ')) 
				{
					return true;
				}
			}

			return false;
		
			#endif
		}

		// loop through all tick groups and update the tick group names if they are the default name
		private void HandleNameUpdate(CollectionChangeInfo info)
		{
			#if UNITY_EDITOR
		
			foreach (var group in tickGroups)
			{
				if (group.parameters.name == TickParams.defaultName)
				{
					group.parameters.name = $"{TickParams.defaultName}_{tickGroups.IndexOf(group)}";
				}
			}
		
			#endif
		}

		#endregion
	}
}