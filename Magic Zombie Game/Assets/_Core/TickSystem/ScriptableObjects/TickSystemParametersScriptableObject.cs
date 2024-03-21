using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.TickSystem
{
	[CreateAssetMenu(fileName = "TickSystemParameters", menuName = "Game/Tick System/Tick Parameters")]
	public class TickSystemParametersScriptableObject : ScriptableObject
	{
		#region Properties

		public string MainGroupName => mainGroupName;
		
		public string DefaultGroupName => defaultGroupName;
		
		public string UpdateGroupName => unityUpdateGroupName;
		
		public string FixedUpdateGroupName => unityFixedUpdateGroupName;
		
		public string LateUpdateGroupName => unityLateUpdateGroupName;
		
		public int MinTickRate => minTickRate;
		
		public int MaxTickRate => maxTickRate;

		#endregion

		#region Serialized Fields

		[HorizontalGroup("Naming", Gap = 0.05f)]
		[TitleGroup("Naming/Built-in")]
		[ValidateInput("@!mainGroupName.Contains(\' \')", "@GROUP_NAME_ERROR")]
		[LabelWidth(120)]
		[SerializeField]
		private string mainGroupName = "Main";
		
		[HorizontalGroup("Naming")]
		[TitleGroup("Naming/Built-in")]
		[ValidateInput("@!defaultGroupName.Contains(\' \')", "@GROUP_NAME_ERROR")]
		[LabelWidth(120)]
		[SerializeField]
		private string defaultGroupName = "TickGroup";

		[HorizontalGroup("Naming")]
		[TitleGroup("Naming/Unity")]
		[ValidateInput("@!unityUpdateGroupName.Contains(\' \')", "@GROUP_NAME_ERROR")]
		[LabelWidth(120)]
		[LabelText("Update Group")]
		[SerializeField]
		private string unityUpdateGroupName = "Update";
		
		[HorizontalGroup("Naming")]
		[TitleGroup("Naming/Unity")]
		[ValidateInput("@!unityFixedUpdateGroupName.Contains(\' \')", "@GROUP_NAME_ERROR")]
		[LabelWidth(120)]
		[LabelText("Fixed Update Group")]
		[SerializeField]
		private string unityFixedUpdateGroupName = "FixedUpdate";
		
		[HorizontalGroup("Naming")]
		[TitleGroup("Naming/Unity")]
		[ValidateInput("@!unityLateUpdateGroupName.Contains(\' \')", "@GROUP_NAME_ERROR")]
		[LabelWidth(120)]
		[LabelText("Late Update Group")]
		[SerializeField]
		private string unityLateUpdateGroupName = "LateUpdate";
		
		[TitleGroup("Tick Rates")]
		[LabelText("Minimum Tick Rate")]
		[LabelWidth(120)]
		[SerializeField]
		private int minTickRate = 1;
		
		[TitleGroup("Tick Rates")]
		[LabelText("Maximum Tick Rate")]
		[LabelWidth(120)]
		[SerializeField]
		private int maxTickRate = 120;

		#endregion
		
		private const string GROUP_NAME_ERROR = "Group name cannot contain spaces.";
	}
}