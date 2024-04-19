using UnityEngine;
using UnityEngine.Events;

namespace Points
{
	/// <summary>
	/// A dedicated component that uses unity events to handle the buyable component events.
	/// Simply wraps the existing component events into unity events.
	/// </summary>
	[RequireComponent(typeof(BuyableComponent))]
	public class BuyableComponentEvents : MonoBehaviour
	{
		public BuyableComponent BuyableComponent => _buyableComponent;
		
		[UnityEventsCategory]
		public UnityEvent onBuySuccess;
		
		[UnityEventsCategory]
		public UnityEvent onBuyFailed;
		
		private BuyableComponent _buyableComponent;
		
		#region Unity Methods

		private void Awake()
		{
			_buyableComponent = GetComponent<BuyableComponent>();
		}

		private void OnEnable()
		{
			_buyableComponent.OnBuySuccess += onBuySuccess.Invoke;
			_buyableComponent.OnBuyFail += onBuyFailed.Invoke;
		}

		private void OnDisable()
		{
			_buyableComponent.OnBuySuccess -= onBuySuccess.Invoke;
			_buyableComponent.OnBuyFail -= onBuyFailed.Invoke;
		}

		#endregion
	}
}