using System;
using Sirenix.OdinInspector;

namespace Core.CustomTickSystem.Components
{
	[Serializable]
	public class TickLayer
	{
		public TickLayer()
		{
			parameters = new TickParams();
			eventContainer = new TickEventContainer();
			timer = new TickTimer();
		}

		public TickLayer(string name, bool canEditName)
		{
			parameters = new TickParams(canRename: canEditName, canDisable: true)
			{
				name = name
			};
		
			eventContainer = new TickEventContainer();
			timer = new TickTimer(tickFirstFrame: false);
		}
	
		[InlineProperty]
		[HideLabel]
		public TickParams parameters;
	
		public TickEventContainer eventContainer;
	
		public TickTimer timer;
	}
}