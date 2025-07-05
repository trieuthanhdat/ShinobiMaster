using System.Collections.Generic;
using System.Linq;
using Game.Chests;
using Game.Chests.Items;
using Skins;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class IronChestUI: MonoBehaviour
	{
		public IronChest IronChest
		{
			get => this.ironChest;
			set
			{
				this.ironChest = value;

				this.ironChest.OnOpened += OnOpened;
			}
		}

		[SerializeField] private IronChest ironChest;

		[SerializeField] private Sprite openedSprite;
		[SerializeField] private Sprite closedSprite;
		[SerializeField] private Image image;
		[SerializeField] private GameObject openedStatePanel;
		[SerializeField] private GameObject coinsItemOpenedState;
		[SerializeField] private Text coinsItemText;
		[SerializeField] private GameObject skinItemOpenedState;
		[SerializeField] private Image skinItemImage;

		private ChestOpenningUI chestOpenningUi;

		private bool isOpened;


		private void Awake()
		{
			if (IronChest != null)
			{
				IronChest.OnOpened += OnOpened;
			}

			this.chestOpenningUi = FindObjectOfType<ChestOpenningUI>();
		}


		public void OnClickChest()
		{
			if (IronChest.IsOpened || this.isOpened)
			{
				return;
			}

			if (GameHandler.Singleton.PlayerProfile.KeysCount <= 0)
			{
				return;
			}

			this.isOpened = true;

			var chestOpenNumberRewards = this.chestOpenningUi.ChestOpenNumberRewards.SingleOrDefault(r => 
					r.ChestOpenNumber == this.chestOpenningUi.OpenNumber);
			
			if (chestOpenNumberRewards == null)
			{
				chestOpenNumberRewards = new ChestOpenNumberRewards()
				{
					ChestOpenNumber = this.chestOpenningUi.OpenNumber,
					RewardTypes = new List<ChestRewardType>()
				};

				for (var i = 0; i < 9; i++)
				{
					chestOpenNumberRewards.RewardTypes.Add(ChestRewardType.Coins);
				}

				var skinAfterFirstAds = Random.value <= 0.2f;

				var skinIdx = skinAfterFirstAds ? Random.Range(3, 6) : Random.Range(6, 9);

				chestOpenNumberRewards.RewardTypes[skinIdx] = ChestRewardType.Skin;
				
				this.chestOpenningUi.ChestOpenNumberRewards.Add(chestOpenNumberRewards);
			}
			
			if (this.chestOpenningUi.ChestNumber < chestOpenNumberRewards.RewardTypes.Count)
			{
				var chestItem = this.ironChest.GetComponent<ChestItem>();

				if (chestItem != null)
				{
					Destroy(chestItem);
				}
			
				if (chestOpenNumberRewards.RewardTypes[this.chestOpenningUi.ChestNumber] == ChestRewardType.Coins)
				{
					var coinChestItem = this.ironChest.gameObject.AddComponent<CoinChestItem>();

					coinChestItem.CoinType = CoinType.Soft;
					coinChestItem.Coins = this.chestOpenningUi.Coins[this.chestOpenningUi.ChestWithCoinsOpenNumber];
		
					this.ironChest.Put(coinChestItem);
					this.chestOpenningUi.ChestWithCoinsOpenNumber++;
				}
				else if (chestOpenNumberRewards.RewardTypes[this.chestOpenningUi.ChestNumber] ==
				         ChestRewardType.Skin)
				{
					if (this.chestOpenningUi.SkinReward != null)
					{
						var skinChestItem = this.ironChest.gameObject.AddComponent<SkinChestItem>();


						skinChestItem.Skin = this.chestOpenningUi.SkinReward;

						this.ironChest.Put(skinChestItem);
					}
					else
					{
						var coinChestItem = this.ironChest.gameObject.AddComponent<CoinChestItem>();

						coinChestItem.CoinType = CoinType.Soft;
						coinChestItem.Coins = 500;
		
						this.ironChest.Put(coinChestItem);
					}
				}
			}
		
			IronChest.Open();

			GameHandler.Singleton.PlayerProfile.KeysCount--;
			this.chestOpenningUi.ChestNumber++;
		}


		private void OnUnlockSkin(Skin skin)
		{
			if (skin is CharacterSkin characterSkin)
			{
				GameHandler.Singleton.SkinService.CurrentCharacterSkin = characterSkin;
				
				SkinShopUI.Instance.ApplyCharacterSkin(characterSkin);

				GameHandler.Singleton.Player.ApplySkin(characterSkin);
				
				SkinShopUI.Instance.SelectCharacterSkin(characterSkin);
			}
			else if(skin is WeaponSkin weaponSkin)
			{
				GameHandler.Singleton.SkinService.CurrentWeaponSkin = weaponSkin;
				
				SkinShopUI.Instance.ApplyWeaponSkin(weaponSkin);

				GameHandler.Singleton.Player.ApplyWeaponSkin(weaponSkin);
				
				SkinShopUI.Instance.SelectWeaponSkin(weaponSkin);
			}

			AfterSkinRewardUI.Instance.ApplyHeroSkin(GameHandler.Singleton.SkinService.CurrentCharacterSkin);
			AfterSkinRewardUI.Instance.ApplyWeaponSkin(GameHandler.Singleton.SkinService.CurrentWeaponSkin);

			var type = skin is CharacterSkin ? "HERO" : "WEAPON";
			
			AfterSkinRewardUI.Instance.SetHeaderText($"NEW {type} UNLOCK!");
			AfterSkinRewardUI.Instance.ShowPanel();
		}

		public void SetClosedState()
		{
			this.openedStatePanel.SetActive(false);
			this.skinItemOpenedState.SetActive(false);
			this.coinsItemOpenedState.SetActive(false);
			this.image.enabled = true;
			IronChest.IsOpened = false;
			this.isOpened = false;
		}

		private void OnOpened(Chest chest, ChestItem chestItem)
		{
			this.image.enabled = false;
			
			this.openedStatePanel.SetActive(true);

			if (chestItem is SkinChestItem skinChestItem)
			{
				this.skinItemImage.sprite = skinChestItem.Skin.Sprite;
				this.skinItemImage.preserveAspect = true;
				this.skinItemOpenedState.SetActive(true);
				
				skinChestItem.Skin.Available = true;
				skinChestItem.Skin.Skipped = true;
				SkinRepository.Instance.AddOrUpdateSkinData(skinChestItem.Skin);
				
				GameHandler.Singleton.SkinService.AddSkinInProfile(skinChestItem.Skin);
				
				OnUnlockSkin(skinChestItem.Skin);
			}
			else if (chestItem is CoinChestItem coinChestItem)
			{
				this.coinsItemText.text = coinChestItem.Coins.ToString();
				this.coinsItemOpenedState.SetActive(true);

				GameHandler.Singleton.PlayerProfile.SoftCoins += coinChestItem.Coins;
			}
		}
	}
}