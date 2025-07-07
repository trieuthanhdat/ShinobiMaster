using Advertising;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using UnityEngine;

public class AdmobComponent : MonoSingleton<AdmobComponent>, IAdvertising
{
    public bool NeedReward { get; private set; }

    public string InterstitialAdUnitId = "ca-app-pub-6855474545715317/8420744983";
    public string RewardedAdUnitId = "ca-app-pub-6855474545715317/3286153593";
    public string BannerAdUnitId = "ca-app-pub-6855474545715317/1202398810";

    public Action RewardAction { get; set; }
    public Action DismissAction { get; set; }

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private BannerView bannerView;

    private string placement;

    public bool InterstitialAvailable => GameHandler.Singleton.Level.CurrStageNumber > 1 &&
                                         (GameHandler.Singleton.Level.GetNumLevel() > 1 ||
                                          GameHandler.Singleton.Level.CurrStageNumber > 3);

    private void Awake()
    {

        MobileAds.Initialize(initStatus => { });
        InitializeInterstitialAds();
        InitializeRewardedAds();
        InitializeBannerAds();
    }

    public bool HaveConnection()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    #region Banners  

    public void InitializeBannerAds()
    {
        bannerView = new BannerView(BannerAdUnitId, AdSize.Banner, AdPosition.Bottom);
        bannerView.OnBannerAdLoaded += ShowBanner;
        bannerView.OnBannerAdLoadFailed += OnBannerLoadFailed;
        bannerView.LoadAd(new AdRequest());
    }

    private void OnBannerLoadFailed(LoadAdError error)
    {
        Debug.LogError($"Banner Ad Failed to Load: {error.GetMessage()}");
    }

    public void ShowBanner()
    {
        bannerView?.Show();
    }

    public void HideBanner()
    {
        bannerView?.Hide();
    }

    #endregion

    #region Interstitial Ads  

    public void InitializeInterstitialAds()
    {
        LoadInterstitial();
    }

    public void ShowInterstitialAd()
    {
        if (!this.InterstitialAvailable || interstitialAd == null || !interstitialAd.CanShowAd())
        {
            Debug.LogWarning("Interstitial Ad is not ready.");
            return;
        }

        interstitialAd.Show();
    }

    public void ShowInterstitialAd(string placement)
    {
        this.placement = placement;
        ShowInterstitialAd();
    }

    public bool IsInterstitialAdReady()
    {
        return interstitialAd != null && interstitialAd.CanShowAd();
    }

    public bool IsInterstitialAdReady(string placement)
    {
        return IsInterstitialAdReady();
    }

    private void LoadInterstitial()
    {
        var adRequest = new AdRequest();
        InterstitialAd.Load(InterstitialAdUnitId, adRequest, (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Interstitial Ad Failed to Load: {error.GetMessage()}");
                return;
            }

            interstitialAd = ad;
            interstitialAd.OnAdFullScreenContentClosed += LoadInterstitial;
        });
    }

    #endregion

    #region Rewarded Ads  

    public void InitializeRewardedAds()
    {
        LoadRewardedAd();
    }

    public void ShowRewardedAd(Action rewardAction, Action dismissAction)
    {
        if (rewardedAd == null || !rewardedAd.CanShowAd())
        {
            Debug.LogWarning("Rewarded Ad is not ready.");
            return;
        }

        this.RewardAction = rewardAction;
        this.DismissAction = dismissAction;
        rewardedAd.Show((reward) =>
        {
            NeedReward = true;
            StartCoroutine(RewardOrDismissAction());
        });
    }

    public void ShowRewardedAd(string placement, Action rewardAction, Action dismissAction)
    {
        this.placement = placement;
        ShowRewardedAd(rewardAction, dismissAction);
    }

    public bool IsRewardedAdReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    public bool IsRewardedAdReady(string placement)
    {
        return IsRewardedAdReady();
    }

    private void LoadRewardedAd()
    {
        var adRequest = new AdRequest();
        RewardedAd.Load(RewardedAdUnitId, adRequest, (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Rewarded Ad Failed to Load: {error.GetMessage()}");
                return;
            }

            rewardedAd = ad;
        });
    }

    private IEnumerator RewardOrDismissAction()
    {
        yield return new WaitForSeconds(0.3f);

        if (NeedReward)
        {
            RewardAction?.Invoke();
        }
        else
        {
            DismissAction?.Invoke();
        }

        RewardAction = null;
        DismissAction = null;
        NeedReward = false;
    }

    #endregion
}
