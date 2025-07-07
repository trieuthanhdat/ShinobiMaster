using SendAppMetrica;
using System;
using System.Collections;
using UnityEngine;

namespace Advertising
{
    public class ApplovinMaxComponent : MonoBehaviour, IAdvertising
    {
        //        public static ApplovinMaxComponent Instance { get; private set; }

        //        public bool NeedReward { get; private set; }

        //        private const string InterstitialAdUnitId = "da646e3188ae4c6d";
        //        private const string RewardedAdUnitId = "e6f37452cec01962";
        //        private const string BannerAdUnitId = "169f819e6e353f77";

        //        public Action RewardAction { get; set; }
        //        public Action DismissAction { get; set; }

        //        private int interstitialRetryAttempt;
        //        private int rewardedRetryAttempt;

        //        private string placement;
        //        private string rewardedResult;
        //        private string interstitialResult;

        //        public bool InterstitialAvailable => GameHandler.Singleton.Level.CurrStageNumber > 1 && (GameHandler.Singleton.Level.GetNumLevel() > 1 ||
        //                                             GameHandler.Singleton.Level.CurrStageNumber > 3);




        //        private void Awake()
        //        {
        //            if (Instance == null)
        //            {
        //                Instance = this;
        //                this.transform.SetParent(null);
        //                DontDestroyOnLoad(this.gameObject);
        //            }
        //            else 
        //            {
        //                if (Instance != this)
        //                {
        //                    Destroy(this.gameObject);
        //                }

        //                return;
        //            }

        //#if UNITY_EDITOR || DEVELOPMENT_BUILD
        //            return;
        //#endif

        //            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        //            {
        //                if (sdkConfiguration.ConsentDialogState == MaxSdkBase.ConsentDialogState.Applies)
        //                {
        //                    var gdpr = FindObjectOfType<GDPR>();

        //                    if (gdpr.IsGDPRAccepted)
        //                    {
        //                        gdpr.SetActiveWindow(true);
        //                    }
        //                }

        //                InitializeInterstitialAds();
        //                InitializeRewardedAds();
        //            };

        //            MaxSdk.SetSdkKey("6AQkyPv9b4u7yTtMH9PT40gXg00uJOTsmBOf7hDxa_-FnNZvt_qTLnJAiKeb5-2_T8GsI_dGQKKKrtwZTlCzAR");
        //            MaxSdk.InitializeSdk();
        //        }

        //        public bool HaveConnection()
        //        {
        //            return Application.internetReachability != NetworkReachability.NotReachable;
        //        }

        //        #region Banners

        //        public void InitializeBannerAds()
        //        {
        //            MaxSdkCallbacks.OnBannerAdLoadedEvent += OnBannerAdLoadedEvent;

        //            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets
        //            // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments
        //            MaxSdk.CreateBanner(BannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        //            // Set background or background color for banners to be fully functional
        //            MaxSdk.SetBannerBackgroundColor(BannerAdUnitId, Color.clear);
        //        }

        //        public void ShowBanner()
        //        {
        //#if UNITY_EDITOR || DEVELOPMENT_BUILD
        //            return;
        //#endif

        //            MaxSdk.ShowBanner(BannerAdUnitId);
        //        }

        //        public void HideBanner()
        //        {
        //            MaxSdk.HideBanner(BannerAdUnitId);
        //        }

        //        private void OnBannerAdLoadedEvent(string obj)
        //        {
        //            ShowBanner();

        //            AnalyticsManager.Instance.Event_VideoAdsWatch("banner", null,"watched", null);
        //        }


        //        #endregion

        //        #region Interstitial Ads

        //        public void InitializeInterstitialAds()
        //        {
        //            // Attach callback
        //            MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
        //            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
        //            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
        //            MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;
        //            MaxSdkCallbacks.OnInterstitialClickedEvent += OnInterstitialClickedEvent;
        //            MaxSdkCallbacks.OnInterstitialDisplayedEvent += OnInterstitialDisplayedEvent;

        //            // Load the first interstitial
        //            LoadInterstitial();
        //        }




        //        public void ShowInterstitialAd()
        //        {
        //#if UNITY_EDITOR || DEVELOPMENT_BUILD
        //            return;
        //#endif
        //            if (!this.InterstitialAvailable)
        //            {
        //                return;
        //            }


        //            if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId) )
        //            {   
        //                MaxSdk.ShowInterstitial(InterstitialAdUnitId);
        //            }
        //        }

        //        public void ShowInterstitialAd(string placement)
        //        {
        //#if UNITY_EDITOR || DEVELOPMENT_BUILD
        //            return;
        //#endif

        //            if (!this.InterstitialAvailable)
        //            {
        //                return;
        //            }

        //            if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        //            {
        //                this.placement = placement;

        //                var connection = HaveConnection();

        //                AnalyticsManager.Instance.Event_VideoAdsStarted("interstitial", placement, "start", connection);

        //                ShowInterstitialAd();
        //            }
        //        }


        //        public bool IsInterstitialAdReady()
        //        {
        //            return MaxSdk.IsInterstitialReady(InterstitialAdUnitId);
        //        }

        //        public bool IsInterstitialAdReady(string placement)
        //        {
        //            var result = IsInterstitialAdReady();

        //            var strResult = result ? "success" : "not_available";
        //            var connection = HaveConnection();

        //            AnalyticsManager.Instance.Event_VideoAdsAvailable("interstitial", placement, strResult, connection);

        //            return result;
        //        }



        //        private void LoadInterstitial()
        //        {
        //            MaxSdk.LoadInterstitial(InterstitialAdUnitId);
        //        }

        //        private void OnInterstitialLoadedEvent(string adUnitId)
        //        {
        //            this.interstitialRetryAttempt = 0;
        //        }

        //        private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
        //        {
        //            Time.timeScale = 1f;

        //            this.interstitialRetryAttempt++;

        //            var retryDelay = Math.Pow(2, Math.Min(6, this.interstitialRetryAttempt));

        //            // Interstitial ad failed to load. We recommend re-trying in 3 seconds.
        //            Invoke(nameof(LoadInterstitial), (float)retryDelay);
        //        }

        //        private void OnInterstitialDisplayedEvent(string obj)
        //        {
        //            this.interstitialResult = "watched";
        //        }

        //        private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
        //        {
        //            Time.timeScale = 1f;

        //            // Interstitial ad failed to display. We recommend loading the next ad
        //            LoadInterstitial();
        //        }

        //        private void OnInterstitialClickedEvent(string obj)
        //        {
        //            this.interstitialResult = "clicked";
        //        }

        //        private void OnInterstitialDismissedEvent(string adUnitId)
        //        {
        //            Time.timeScale = 1f;

        //            // Interstitial ad is hidden. Pre-load the next ad
        //            LoadInterstitial();

        //            var connection = HaveConnection();
        //            AnalyticsManager.Instance.Event_VideoAdsWatch("interstitial", this.placement, this.interstitialResult, connection);
        //        }

        //        #endregion


        //        #region Rewarded Ads

        //        public void InitializeRewardedAds()
        //        {
        //            // Attach callback
        //            MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
        //            MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
        //            MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        //            MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;

        //            MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;

        //            MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
        //            MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        //            // Load the first RewardedAd
        //            LoadRewardedAd();
        //        }

        //        public void ShowRewardedAd(Action rewardAction, Action dismissAction)
        //        {
        //#if UNITY_EDITOR || DEVELOPMENT_BUILD
        //            return;
        //#endif

        //            if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        //            {
        //                this.RewardAction = rewardAction;
        //                this.DismissAction = dismissAction;
        //                MaxSdk.ShowRewardedAd(RewardedAdUnitId);
        //            }
        //        }

        //        public void ShowRewardedAd(string placement, Action rewardAction, Action dismissAction)
        //        {
        //#if UNITY_EDITOR || DEVELOPMENT_BUILD
        //            return;
        //#endif

        //            if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        //            {
        //                this.placement = placement;

        //                var connection = HaveConnection();

        //                AnalyticsManager.Instance.Event_VideoAdsStarted("rewarded", placement, "start", connection);

        //                ShowRewardedAd(rewardAction, dismissAction);
        //            }
        //        }


        //        public bool IsRewardedAdReady()
        //        {
        //            return MaxSdk.IsRewardedAdReady(RewardedAdUnitId);
        //        }

        //        public bool IsRewardedAdReady(string placement)
        //        {
        //            var result = IsRewardedAdReady();

        //            var strResult = result ? "success" : "not_available";
        //            var connection = HaveConnection();

        //            AnalyticsManager.Instance.Event_VideoAdsAvailable("rewarded", placement, strResult, connection);

        //            return result;
        //        }



        //        private void LoadRewardedAd()
        //        {
        //            MaxSdk.LoadRewardedAd(RewardedAdUnitId);
        //        }

        //        private void OnRewardedAdLoadedEvent(string adUnitId)
        //        {
        //            this.rewardedRetryAttempt = 0;
        //        }

        //        private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
        //        {
        //            Time.timeScale = 1f;

        //            this.NeedReward = true;

        //            StartCoroutine(RewardOrDismissAction());

        //            this.rewardedRetryAttempt++;

        //            var retryDelay = Math.Pow(2, Math.Min(6, this.rewardedRetryAttempt));

        //            // Rewarded ad failed to load. We recommend re-trying in 3 seconds.
        //            Invoke(nameof(LoadRewardedAd), (float)retryDelay);
        //        }

        //        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
        //        {
        //            Time.timeScale = 1f;

        //            this.NeedReward = true;

        //            StartCoroutine(RewardOrDismissAction());
        //            // Rewarded ad failed to display. We recommend loading the next ad
        //            LoadRewardedAd();
        //        }

        //        private void OnRewardedAdDisplayedEvent(string adUnitId)
        //        {
        //            this.rewardedResult = "watched";
        //        }

        //        private void OnRewardedAdClickedEvent(string adUnitId)
        //        {
        //            this.rewardedResult = "clicked";
        //        }

        //        private void OnRewardedAdDismissedEvent(string adUnitId)
        //        {
        //            Time.timeScale = 1f;

        //            StartCoroutine(RewardOrDismissAction());
        //            // Rewarded ad is hidden. Pre-load the next ad
        //            LoadRewardedAd();

        //            var connection = HaveConnection();
        //            AnalyticsManager.Instance.Event_VideoAdsWatch("rewarded", this.placement, this.rewardedResult, connection);
        //        }

        //        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
        //        {
        //            Time.timeScale = 1f;

        //            // Rewarded ad was displayed and user should receive the reward
        //            this.rewardedResult = "watched";
        //            this.NeedReward = true;
        //        }

        //        private IEnumerator RewardOrDismissAction()
        //        {
        //            yield return new WaitForSeconds(0.3f);

        //            if (this.NeedReward)
        //            {
        //                this.RewardAction?.Invoke();
        //            }
        //            else
        //            {
        //                this.DismissAction?.Invoke();
        //            }

        //            this.RewardAction = null;
        //            this.DismissAction = null;

        //            this.NeedReward = false;
        //        }

        //        #endregion
        public Action RewardAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action DismissAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void HideBanner()
        {
        }

        public bool IsInterstitialAdReady()
        {
            return false;
        }

        public bool IsInterstitialAdReady(string placement)
        {
            return false;
        }

        public bool IsRewardedAdReady()
        {
            return false;
        }

        public bool IsRewardedAdReady(string placement)
        {
            return false;
        }

        public void ShowBanner()
        {
        }

        public void ShowInterstitialAd()
        {
        }

        public void ShowInterstitialAd(string placement)
        {
        }

        public void ShowRewardedAd(Action rewardAction, Action dismissAction)
        {
        }

        public void ShowRewardedAd(string placement, Action rewardAction, Action dismissAction)
        {
        }
    }
}