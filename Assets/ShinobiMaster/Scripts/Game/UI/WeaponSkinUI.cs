using Skins;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class WeaponSkinUI: MonoBehaviour
	{
		[field:SerializeField] public WeaponSkin WeaponSkin { get; set; }
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
			if (WeaponSkin == null || !WeaponSkin.Available)
			{
				return;
			}
		
			this.selectedFX.SetActive(selected);
			this.borderImage.color = selected ? this.selectedBorderColor : this.borderColor;
		}
		
		public void UpdateState()
		{
			if (WeaponSkin == null)
			{
				return;
			}
		
			if (WeaponSkin.Available)
			{
				this.image.sprite = WeaponSkin.Sprite;
				this.skinForAdsButton.gameObject.SetActive(false);
				this.buyButton.gameObject.SetActive(false);
			}
			else
			{
				this.image.sprite = WeaponSkin.NotAvailableSprite;
				
				if (WeaponSkin.ForAds)
				{
					this.skinForAdsButton.gameObject.SetActive(true);

					this.adsViewCountText.text = $"{WeaponSkin.CurrentViewCount}/{WeaponSkin.NeedViewCount}";
				}

				if (WeaponSkin.ForCoins)
				{
					this.buyButton.gameObject.SetActive(true);

					foreach (var graphic in this.buyButtonGraphics)
					{
						graphic.color = GameHandler.Singleton.PlayerProfile.SoftCoins < WeaponSkin.Price ? 
							this.buyButtonInactiveColor : this.buyButtonActiveColor;
					}
				}
			}
		}
		
		private void OnSkinAdded(Skin skin)
		{
			if (skin == WeaponSkin)
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
			if (GameHandler.Singleton.PlayerProfile.SoftCoins < WeaponSkin.Price)
			{
				return;
			}

			if (GameHandler.Singleton.SkinService.IsSkinInProfile(WeaponSkin))
			{
				return;
			}

			GameHandler.Singleton.PlayerProfile.SoftCoins -= WeaponSkin.Price;

			GameHandler.Singleton.SkinService.AddSkinInProfile(WeaponSkin);
			
			OnUnlockForAds();
			
			UpdateState();
		}

		private void SkinForAdsRewardAction()
		{
			WeaponSkin.CurrentViewCount++;

			if (WeaponSkin.CurrentViewCount == WeaponSkin.NeedViewCount)
			{
				OnUnlockForAds();
			}
			
			UpdateState();
			
			AdLoadingPanel.Instance.HidePanel();
		}

		private void OnUnlockForAds()
		{
			WeaponSkin.Available = true;
			WeaponSkin.Skipped = true;
			
			SkinRepository.Instance.AddOrUpdateSkinData(WeaponSkin);
				
			GameHandler.Singleton.SkinService.CurrentWeaponSkin = WeaponSkin;
			
			AfterSkinRewardUI.Instance.ApplyHeroSkin(GameHandler.Singleton.SkinService.CurrentCharacterSkin);
			AfterSkinRewardUI.Instance.ApplyWeaponSkin(GameHandler.Singleton.SkinService.CurrentWeaponSkin);
			
			Button.onClick?.Invoke();
			
			SkinShopUI.Instance.CloseWeaponsSkinShop();
			
			AfterSkinRewardUI.Instance.SetHeaderText($"NEW WEAPON UNLOCK!");
			AfterSkinRewardUI.Instance.ShowPanel();
			
			AfterSkinRewardUI.Instance.OnPanelHidden += OnPanelHidden;
		}

		private void OnPanelHidden()
		{
			AfterSkinRewardUI.Instance.SetActiveHeroCamera(false);
			SkinShopUI.Instance.OpenWeaponsSkinShop();
			
			AfterSkinRewardUI.Instance.OnPanelHidden -= OnPanelHidden;
		}
	}
}