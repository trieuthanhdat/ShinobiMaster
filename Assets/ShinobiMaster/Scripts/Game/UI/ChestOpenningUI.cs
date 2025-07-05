using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Chests;
using Game.Chests.Items;
using Game.PlayerScripts;
using Skins;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.UI
{
	[Serializable]
	public class ChestOpenNumberRewards
	{
		public int ChestOpenNumber;
		public List<ChestRewardType> RewardTypes;
	}

	public class ChestOpenningUI: MonoBehaviour
	{
		public static ChestOpenningUI Instance;
	
		private const string OpenNumberKey = "chestOpenNumber";
		private const string ChestNumberKey = "chestNumber";
	
		public int OpenNumber
		{
			get => PlayerPrefs.GetInt(OpenNumberKey, -1);
			set
			{
				PlayerPrefs.SetInt(OpenNumberKey, value);
				PlayerPrefs.Save();
			}
		}
		public int ChestNumber
		{
			get => PlayerPrefs.GetInt(ChestNumberKey, 0);
			set
			{
				PlayerPrefs.SetInt(ChestNumberKey, value);
				PlayerPrefs.Save();
			}
		}

		public List<ChestOpenNumberRewards> ChestOpenNumberRewards;
		[SerializeField] private Text chooseBoxText;
		[SerializeField] private KeysUI keysUI;
		[SerializeField] private Button adsGetKeysButton;
		[SerializeField] private GameObject parent;
		[SerializeField] private List<IronChestUI> ironChestsUI;
		[SerializeField] private Button closePanelButton;
		[SerializeField] private Image bestPrizeImage;
		[SerializeField] private Sprite coinSprite;
		public Skin SkinReward { get; set; }
		public int[] Coins;
		public int ChestWithCoinsOpenNumber { get; set; }
		
		

		private void Awake()
		{
			if (Instance)
			{
				Destroy(gameObject);
				return;
			}
			else
			{
				Instance = this;
			}
		}

		private void Start()
		{
			GameHandler.Singleton.PlayerProfile.OnKeysCountChanged += OnKeysCountChanged;
		}

		private void OnKeysCountChanged(int keysCount, int prevKeyCount)
		{
			UpdateAdsGetKeysPanelState();
		}


		public void OnClickAdsGetKeys()
		{
			AdLoadingPanel.Instance.Placement = "RewardAds_Keys";
			AdLoadingPanel.Instance.SetActive(true, AdsGetKeysRewardAction, AdLoadingPanel.Instance.HidePanel);
		}

		private void AdsGetKeysRewardAction()
		{
			GameHandler.Singleton.PlayerProfile.KeysCount = 3;

			AdLoadingPanel.Instance.HidePanel();
		}

		private Skin GetSkinReward()
		{
			SkinReward = null;
			
			var availableSkins =
				SkinRepository.Instance.Skins.Where(s =>
					!GameHandler.Singleton.SkinService.IsSkinInProfile(s)).ToList();

			if (availableSkins.Count > 0)
			{
				SkinReward = availableSkins[Random.Range(0, availableSkins.Count)];
			}

			return SkinReward;
		}

		public void SetActivePanel(bool active)
		{
			if (active)
			{
				var rnd = new System.Random();
				Coins = Coins.OrderBy(x => rnd.Next()).ToArray();
			
				var skinReward = GetSkinReward();

				ChestWithCoinsOpenNumber = 0;

				if (skinReward != null)
				{
					this.bestPrizeImage.sprite = SkinReward.Sprite;
					this.bestPrizeImage.preserveAspect = true;
				}
				else
				{
					this.bestPrizeImage.sprite = this.coinSprite;
					this.bestPrizeImage.preserveAspect = true;
				}
				
				Pause.SetPause();
			
				UpdateAdsGetKeysPanelState();
				foreach (var ironChestUi in this.ironChestsUI)
				{
					ironChestUi.SetClosedState();
				}
			}
			else
			{
				if (!NewSkinUI.Instance.IsNewSkin())
				{
					Pause.OffPause();
					TimeControll.Singleton.UnpauseTimeControl();
					GameHandler.Singleton.Level.LoadNextStage(0f);
				}
				else
				{
					NewSkinUI.Instance.ShowWithDelayIfSkin(0f);
				}
			}
		
			this.parent.SetActive(active);
		}

		public void SetActivePanelWithDelay(bool active, float delay)
		{
			StartCoroutine(SetActivePanelWithDelayProcess(active, delay));
		}

		private IEnumerator SetActivePanelWithDelayProcess(bool active, float delay)
		{
			yield return new WaitForSeconds(delay);
			
			SetActivePanel(active);
		}

		private void UpdateAdsGetKeysPanelState()
		{
			if (GameHandler.Singleton.PlayerProfile.KeysCount == 0)
			{
				this.chooseBoxText.gameObject.SetActive(false);
				this.keysUI.gameObject.SetActive(false);
				this.adsGetKeysButton.gameObject.SetActive(ChestNumber != 8);
				this.closePanelButton.gameObject.SetActive(true);
			}
			else
			{
				this.chooseBoxText.gameObject.SetActive(true);
				this.keysUI.gameObject.SetActive(true);
				this.adsGetKeysButton.gameObject.SetActive(false);
				this.closePanelButton.gameObject.SetActive(false);
			}
		}
	}
}