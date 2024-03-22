using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CustomTickSystem.Tests
{
	public class TickSystemTests
	{
		[UnityTest]
		public IEnumerator MainTickGroup()
		{
			// Arrange
			bool prevState = TickSystem.GetLayer("Main").parameters.enabled;
			float interval = TickSystem.GetLayer("Main").parameters.TickInterval();
			var cachedWait = new WaitForSeconds(interval);
			var tickListener = new TickListener("Main");
			
			// Act
			TickSystem.GetLayer("Main").parameters.enabled = true;
			tickListener.BeginListen();
			for (int i = 0; i < 5; i++) yield return cachedWait; // 5 ticks
			tickListener.EndListen();
			TickSystem.GetLayer("Main").parameters.enabled = prevState;

			// Assert
			Assert.AreEqual(TickSystem.GetLayer("Main").eventContainer.TickCount, tickListener.tickCount);
		}
	
		[UnityTest]
		public IEnumerator UnityUpdateTickLoop()
		{
			// Arrange
			bool prevState = TickSystem.GetLayer("Update").parameters.enabled;
			var tickListener = new TickListener("Update");
			
			// Act
			TickSystem.GetLayer("Update").parameters.enabled = true;
			tickListener.BeginListen();
			for (int i = 0; i < 5; i++) yield return null; // 5 ticks
			tickListener.EndListen();
			TickSystem.GetLayer("Update").parameters.enabled = prevState;
			
			// Assert
			Assert.AreEqual(TickSystem.GetLayer("Update").eventContainer.TickCount, tickListener.tickCount);
		}
		
		[UnityTest]
		public IEnumerator UnityFixedUpdateTickGroup()
		{
			// Arrange
			bool prevState = TickSystem.GetLayer("FixedUpdate").parameters.enabled;
			var interval = new WaitForFixedUpdate();
			var tickListener = new TickListener("FixedUpdate");
			
			// Act
			TickSystem.GetLayer("FixedUpdate").parameters.enabled = true;
			tickListener.BeginListen();
			for (int i = 0; i < 5; i++) yield return interval; // 5 ticks
			tickListener.EndListen();
			TickSystem.GetLayer("FixedUpdate").parameters.enabled = prevState;
			
			// Assert
			Assert.AreEqual(TickSystem.GetLayer("FixedUpdate").eventContainer.TickCount, tickListener.tickCount);
		}
		
		[UnityTest]
		public IEnumerator UnityLateUpdateTickGroup()
		{
			// Arrange
			bool prevState = TickSystem.GetLayer("LateUpdate").parameters.enabled;
			var interval = new WaitForEndOfFrame();
			var tickListener = new TickListener("LateUpdate");
			
			// Act
			TickSystem.GetLayer("LateUpdate").parameters.enabled = true;
			tickListener.BeginListen();
			for (int i = 0; i < 5; i++) yield return interval; // 5 ticks
			tickListener.EndListen();
			TickSystem.GetLayer("LateUpdate").parameters.enabled = prevState;
			
			// Assert
			Assert.AreEqual(TickSystem.GetLayer("LateUpdate").eventContainer.TickCount, tickListener.tickCount);
		}

		/// <summary>
		/// Dedicated class that handles listening to tick events and counting the number of ticks.
		/// </summary>
		private class TickListener
		{
			public TickListener(string layerName)
			{
				this.layerName = layerName;
				tickCount = 0;
			}

			public int tickCount;
			public string layerName;
			
			public void BeginListen() => TickSystem.AddListener(layerName, Listener);

			public void EndListen() => TickSystem.RemoveListener(layerName, Listener);

			private void Listener()
			{
				tickCount++;
				Debug.Log($"'{layerName}' has ticked '{tickCount}' times");
			}
		}
	}
}