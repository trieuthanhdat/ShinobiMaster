using System;

namespace Advertising
{
	public interface IAdvertising
	{
		Action RewardAction { get; set; }
		Action DismissAction { get; set; }
	
		void ShowInterstitialAd();
		void ShowInterstitialAd(string placement);
		bool IsInterstitialAdReady();
		bool IsInterstitialAdReady(string placement);
		void ShowRewardedAd(Action rewardAction, Action dismissAction);
		void ShowRewardedAd(string placement, Action rewardAction, Action dismissAction);
		bool IsRewardedAdReady();
		bool IsRewardedAdReady(string placement);
		void ShowBanner();
		void HideBanner();
	}
}