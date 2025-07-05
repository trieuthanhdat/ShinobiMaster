using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class CoinsUI: MonoBehaviour
	{
		[SerializeField] private Text coinsText;


		

		private void OnEnable()
		{
			SetCoinsCountText(GameHandler.Singleton.PlayerProfile.SoftCoins);
		}
		
		private void Start()
		{
			GameHandler.Singleton.PlayerProfile.OnSoftCoinsChanged += OnSoftCoinsChanged;
		}

		private void OnDestroy()
		{
			GameHandler.Singleton.PlayerProfile.OnSoftCoinsChanged -= OnSoftCoinsChanged;
		}




		public void SetCoinsCountText(int count)
		{
			this.coinsText.text = count.ToString();
		}
		
		private void OnSoftCoinsChanged(int currCoins, int prevCoins)
		{
			SetCoinsCountText(currCoins);
		}

		public void OnClickMoney()
		{
			GameHandler.Singleton.PlayerProfile.SoftCoins += 500;
		}
	}
}