using Skins;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class HeroSkinUI: MonoBehaviour
	{
		[field:SerializeField] public CharacterSkin CharacterSkin { get; set; }
		[SerializeField] private Image image;
		[SerializeField] private Image borderImage;
		[SerializeField] private GameObject selectedFX;
		[SerializeField] private Color selectedBorderColor;
		[SerializeField] private Color borderColor;
		public Button Button;
		[SerializeField] private Button skinForAdsButton;
		[SerializeField] private Text adsViewCountText;
		[SerializeField] private Button buyButton;
		[SerializeField] private Graphic[] buyButtonGraphics;
		[SerializeField] private Color buyButtonActiveColor;
		[SerializeField] private Color buyButtonInactiveColor;


		private void Awake()
		{
			GameHandler.Singleton.SkinService.OnSkinAdded += OnSkinAdded;
			GameHandler.Singleton.PlayerProfile.OnSoftCoinsChanged += OnSoftCoinsChanged;
			
			this.skinForAdsButton.onClick.AddListener(OnClickSkinForAds);
			this.buyButton.onClick.AddListener(OnClickBuyButton);
		}

		private void Start()
		{
			UpdateState();
		}




		public void SetSelected(bool selected)
		{
			this.selectedFX.SetActive(selected);
			this.borderImage.color = selected ? this.selectedBorderColor : this.borderColor;
		}

		public void UpdateState()
		{
			if (CharacterSkin == null)
			{
				return;
			}
		
			if (CharacterSkin.Available)
			{
				this.image.sprite = CharacterSkin.Sprite;
				this.skinForAdsButton.gameObject.SetActive(false);
				this.buyButton.gameObject.SetActive(false);
			}
			else
			{
				this.image.sprite = CharacterSkin.NotAvailableSprite;
				
				if (CharacterSkin.ForAds)
				{
					this.skinForAdsButton.gameObject.SetActive(true);

					this.adsViewCountText.text = $"{CharacterSkin.CurrentViewCount}/{CharacterSkin.NeedViewCount}";
				}
				
				if (CharacterSkin.ForCoins)
				{
					this.buyButton.gameObject.SetActive(true);
					
					foreach (var graphic in this.buyButtonGraphics)
					{
						graphic.color = GameHandler.Singleton.PlayerProfile.SoftCoins < CharacterSkin.Price ? 
							this.buyButtonInactiveColor : this.buyButtonActiveColor;
					}
				}
			}
		}
		
		private void OnSkinAdded(Skin skin)
		{
			if (skin == CharacterSkin)
			{
				UpdateState();
			}
		}
		
		private void OnSoftCoinsChanged(int currCoins, int prevCoins)
		{
			UpdateState();
		}
		
		private void OnClickSkinForAds()
		{
			AdLoadingPanel.Instance.Placement = "RewardAds_AdsSkin";
			AdLoadingPanel.Instance.SetActive(true, SkinForAdsRewardAction, AdLoadingPanel.Instance.HidePanel);
		}
		
		private void OnClickBuyButton()
		{
			if (GameHandler.Singleton.PlayerProfile.SoftCoins < CharacterSkin.Price)
			{
				return;
			}
			
			if (GameHandler.Singleton.SkinService.IsSkinInProfile(CharacterSkin))
			{
				return;
			}

			GameHandler.Singleton.PlayerProfile.SoftCoins -= CharacterSkin.Price;

			GameHandler.Singleton.SkinService.AddSkinInProfile(CharacterSkin);
			
			OnUnlockForAds();
			
			UpdateState();
		}

		private void SkinForAdsRewardAction()
		{
			CharacterSkin.CurrentViewCount++;

			if (CharacterSkin.CurrentViewCount == CharacterSkin.NeedViewCount)
			{
				OnUnlockForAds();
			}
			
			UpdateState();
			
			AdLoadingPanel.Instance.HidePanel();
		}
		
		private void OnUnlockForAds()
		{
			CharacterSkin.Available = true;
			CharacterSkin.Skipped = true;
			
			SkinRepository.Instance.AddOrUpdateSkinData(CharacterSkin);
				
			GameHandler.Singleton.SkinService.CurrentCharacterSkin = CharacterSkin;
			
			AfterSkinRewardUI.Instance.ApplyHeroSkin(GameHandler.Singleton.SkinService.CurrentCharacterSkin);
			AfterSkinRewardUI.Instance.ApplyWeaponSkin(GameHandler.Singleton.SkinService.CurrentWeaponSkin);
			
			Button.onClick?.Invoke();
			
			SkinShopUI.Instance.CloseHeroesSkinShop();
			
			AfterSkinRewardUI.Instance.SetHeaderText($"NEW HERO UNLOCK!");
			AfterSkinRewardUI.Instance.ShowPanel();
			
			AfterSkinRewardUI.Instance.OnPanelHidden += OnPanelHidden;
		}
		
		private void OnPanelHidden()
		{
			AfterSkinRewardUI.Instance.SetActiveHeroCamera(false);
			SkinShopUI.Instance.OpenHeroesSkinShop();
			
			AfterSkinRewardUI.Instance.OnPanelHidden -= OnPanelHidden;
		}
	}
}