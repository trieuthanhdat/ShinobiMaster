using System;
using System.Collections;
using Advertising;
using UnityEngine;
using UnityEngine.UI;

public class AdLoadingPanel : MonoBehaviour
{
	public static AdLoadingPanel Instance { get; private set; }

	[field:SerializeField]
	public Button CloseAdButton { get; set; }
	[field:SerializeField]
	public string Placement { get; set; }
		
	private Action rewardAction;
	private Action dismissAction;

	private IAdvertising ads;

	[SerializeField] private GameObject panel;




	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			this.transform.SetParent(null);
			DontDestroyOnLoad(this.gameObject);
		}
		else 
		{
			if (Instance != this)
			{
				Destroy(this.gameObject);
			}
                
			return;
		}
	
		if (this.ads == null)
		{
			this.ads = ApplovinMaxComponent.Instance.GetComponent<IAdvertising>();
		}
	}

	private void OnPanelEnable()
	{
		this.ads.RewardAction = this.rewardAction;
		this.ads.DismissAction = this.dismissAction;
		
		if (this.ads.IsRewardedAdReady(this.Placement))
		{
			this.ads.ShowRewardedAd(this.Placement, this.rewardAction, this.dismissAction);
		}
		else
		{
			StartWaitingForAd();
		}

		if (this.CloseAdButton != null)
		{
			this.CloseAdButton.onClick.AddListener(HidePanel);
		}

#if  !UNITY_EDITOR && !DEVELOPMENT_BUILD
			//Time.timeScale = 0;
#endif
	}

	private void OnPanelDisable()
	{
		StopWaitingForAd();
			
		if (this.CloseAdButton != null)
		{
			this.CloseAdButton.onClick.RemoveListener(HidePanel);
		}
			
		this.ads.RewardAction = null;
		this.ads.DismissAction = null;

		this.Placement = string.Empty;
	}


	public void SetActive(bool active, Action rewardAction = null, Action dismissAction = null)
	{
		this.rewardAction = rewardAction;
		this.dismissAction = dismissAction;

		SetActivePanel(active);
			
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		rewardAction?.Invoke();
#endif
	}

	public void HidePanel()
	{
		SetActive(false);
	}

	private void SetActivePanel(bool active)
	{
		this.panel.SetActive(active);
	
		if (active)
		{
			OnPanelEnable();
		}
		else
		{
			OnPanelDisable();
		}
	}

	public void ShowInterstitialAd(string placement)
	{
		if (this.ads == null)
		{
			this.ads = FindObjectOfType<ApplovinMaxComponent>().GetComponent<IAdvertising>();
		}
		
		if (this.ads.IsInterstitialAdReady(placement))
		{
			this.ads.ShowInterstitialAd(placement);
		}
	}

	public void ShowInterstitialAdWithDelay(string placement, float delay)
	{
		StartCoroutine(ShowInterstitialAdWithDelayProcess(placement, delay));
	}
	
	private IEnumerator ShowInterstitialAdWithDelayProcess(string placement, float delay)
	{
		yield return new WaitForSeconds(delay);
		
		while (Pause.IsPause)
		{
			yield return null;
		}
		
		ShowInterstitialAd(placement);
	}



	private void StartWaitingForAd()
	{
		MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoaded;
	}

	private void StopWaitingForAd()
	{
		MaxSdkCallbacks.OnRewardedAdLoadedEvent -= OnRewardedAdLoaded;
	}

	private void OnRewardedAdLoaded(string rObj)
	{
		this.ads.ShowRewardedAd(this.Placement, this.rewardAction, this.dismissAction);
			
		StopWaitingForAd();
	}
}