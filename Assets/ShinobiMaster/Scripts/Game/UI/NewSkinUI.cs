using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SendAppMetrica;
using Skins;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class NewSkinUI: MonoBehaviour
	{
		public static NewSkinUI Instance;

		private const string SkinForOpenShownKey = "sfos";

		[SerializeField] private GameObject newSkinPanel;
		[SerializeField] private GameObject inProgressPanel;
		[SerializeField] private GameObject skinAvailablePanel;
		[SerializeField] private Text[] headersText;
		[SerializeField] private Image[] skinImages;
		[SerializeField] private Image[] progressImages;
		[SerializeField] private Image lockSkinImage;
		[SerializeField] private Text[] progressTexts;
		[SerializeField] private Button loseItButton;
		private Skin currentSkin;
		public bool ProgressAnimationShown { get; set; }
		public bool NeedProgressAnimation { get; set; }

		public bool SkinForOpenShown
		{
			get => bool.Parse(PlayerPrefs.GetString(SkinForOpenShownKey, bool.FalseString));
			set
			{
				PlayerPrefs.SetString(SkinForOpenShownKey, value.ToString());
				PlayerPrefs.Save();
			}
		}
		
		
		
	
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


		public bool IsNewSkin()
		{
			this.currentSkin = SkinRepository.Instance.GetNextSkinForOpen(GameHandler.Singleton.Level.GetNumLevel(),
				GameHandler.Singleton.Level.CurrStageNumber, out var skinStage);

			return this.currentSkin != null;
		}

		public void ShowWithDelayIfSkin(float delay)
		{
			if (!IsNewSkin())
			{
				return;
			}

			StartCoroutine(ShowWithDelayIfSkinProcess(delay));
		}

		private IEnumerator ShowWithDelayIfSkinProcess(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			
			Pause.SetPause();
			
			SetActive(true);
		}

		public void SetActive(bool active)
		{
			var stageNum = GameHandler.Singleton.Level.CurrStageNumber;
		
			if (stageNum > SkinRepository.Instance.SkinStage2)
			{
				Pause.OffPause();
				TimeControll.Singleton.UnpauseTimeControl();
			
				return;
			}

			if (!active)
			{
				Pause.OffPause();
				TimeControll.Singleton.UnpauseTimeControl();
				GameHandler.Singleton.Level.LoadNextStage(0f);
			}
			
			this.currentSkin = SkinRepository.Instance.GetNextSkinForOpen(GameHandler.Singleton.Level.GetNumLevel(),
				stageNum, out var skinStage);
				
			if (this.currentSkin == null || this.currentSkin.Skipped)
			{
				this.newSkinPanel.SetActive(false);
			
				return;
			}
			
			if (active)
			{
				var prevStage = SkinRepository.Instance.GetPrevSkinStage(GameHandler.Singleton.Level.GetNumLevel(),
					stageNum);
	
				var progress = (stageNum - prevStage) /(float) (skinStage - prevStage);

				UpdatePanel();
				
				if (progress >= 1f)
				{
					ShowSkinAvailablePanel();
				}
				else
				{
					ShowSkinInProgressPanel();
				}
			}

			this.newSkinPanel.SetActive(active);
		}
		
		public void UpdatePanel()
		{
			var stageNum = GameHandler.Singleton.Level.CurrStageNumber;
		
			if (stageNum > SkinRepository.Instance.SkinStage2)
			{
				return;
			}
			
			this.currentSkin = SkinRepository.Instance.GetNextSkinForOpen(GameHandler.Singleton.Level.GetNumLevel(),
				stageNum, out var skinStage);

			var prevStage = SkinRepository.Instance.GetPrevSkinStage(GameHandler.Singleton.Level.GetNumLevel(),
				stageNum);

			var progress = (stageNum - prevStage) /(float) (skinStage - prevStage);

			var prevProgress = progress - (1 /(float) (skinStage - prevStage));

			StartCoroutine(AnimatedProgress(prevProgress, progress, 0.5f));
		
			UpdateHeadersText(this.currentSkin is WeaponSkin ? "NEW WEAPON!" : "NEW HERO!");

			SetSkinImagesSprite(this.currentSkin.Sprite);

			this.lockSkinImage.sprite = this.currentSkin.NotAvailableSprite;
		}

		private void SetProgressImagesFillAmount(float amount)
		{
			foreach (var progressImage in this.progressImages)
			{
				progressImage.fillAmount = amount;
			}
		}

		private void SetProgressTextsProgress(float progress)
		{
			foreach (var progressText in this.progressTexts)
			{
				progressText.text = Mathf.RoundToInt(progress * 100f) + "%";
			}
		}

		private IEnumerator AnimatedProgress(float start, float target, float time)
		{
			var currTime = time;

			float currProgress;

			if (NeedProgressAnimation && !ProgressAnimationShown && start < target)
			{
				TimeControll.Singleton.PauseTimeControl();
				Time.timeScale = 1f;
			
				while (currTime > 0)
				{
					var lerp = 1 - currTime / time;

					currProgress = Mathf.Lerp(start, target, lerp);

					SetProgressTextsProgress(currProgress);
					SetProgressImagesFillAmount(currProgress);
					this.lockSkinImage.fillAmount = 1 - currProgress;

					currTime -= Time.deltaTime;

					yield return null;
				}

				ProgressAnimationShown = true;
				NeedProgressAnimation = false;
			}
			
			TimeControll.Singleton.UnpauseTimeControl();

			currProgress = target;
			
			SetProgressTextsProgress(currProgress);
			SetProgressImagesFillAmount(currProgress);
			this.lockSkinImage.fillAmount = 1 - currProgress;
		}

		private void SetSkinImagesSprite(Sprite sprite)
		{
			foreach (var skinImage in this.skinImages)
			{
				skinImage.sprite = sprite;
			}
		}


		public void ShowSkinAvailablePanel()
		{
			StartCoroutine(ShowLoseItButtonWithDelay(2.0f));
			this.inProgressPanel.SetActive(false);
			this.skinAvailablePanel.SetActive(true);
		}

		public void ShowSkinInProgressPanel()
		{
			this.inProgressPanel.SetActive(true);
			this.skinAvailablePanel.SetActive(false);
		}

		private IEnumerator ShowLoseItButtonWithDelay(float delay)
		{
			this.loseItButton.gameObject.SetActive(false);
		
			yield return new WaitForSecondsRealtime(delay);
			
			this.loseItButton.gameObject.SetActive(true);
		}

		public void OnClickLoseItButton()
		{
			this.currentSkin.Skipped = true;
			
			SkinRepository.Instance.AddOrUpdateSkinData(this.currentSkin);
			
			ResetSkippedSkinsIfNewCycle(false);
			
			SetActive(false);
		}

		public void OnClickKeepItButton()
		{
			AdLoadingPanel.Instance.Placement = "RewardAds_BattlePassSkin";
			AdLoadingPanel.Instance.SetActive(true, KeepItRewardAction, KeepItDismissAction);
		}

		private void KeepItDismissAction()
		{
			AdLoadingPanel.Instance.HidePanel();
			SetActive(false);
		}

		private void KeepItRewardAction()
		{
			ResetSkippedSkinsIfNewCycle(true);
		
			this.currentSkin.Available = true;
			
			SkinRepository.Instance.AddOrUpdateSkinData(this.currentSkin);
			
			GameHandler.Singleton.SkinService.AddSkinInProfile(this.currentSkin);

			var idSkin = 0;
			var idWeapon = 0;
			
			if (this.currentSkin is CharacterSkin)
			{
				idSkin = this.currentSkin.Id;
			}
			else
			{
				idWeapon = this.currentSkin.Id;
			}
			
			AnalyticsManager.Instance.Event_BPSkin(GameHandler.Singleton.Level.GetNumLevel(), 
				GameHandler.Singleton.Level.CurrStageNumber, idSkin, idWeapon);

			var player = GameHandler.Singleton.Player;

			if (this.currentSkin is CharacterSkin characterSkin)
			{
				player.ApplySkin(characterSkin);
				SkinShopUI.Instance.ApplyCharacterSkin(characterSkin);
				SkinShopUI.Instance.SelectCharacterSkin(characterSkin);
				GameHandler.Singleton.SkinService.CurrentCharacterSkin = characterSkin;
			}
			
			if (this.currentSkin is WeaponSkin weaponSkin)
			{
				player.ApplyWeaponSkin(weaponSkin);
				SkinShopUI.Instance.ApplyWeaponSkin(weaponSkin);
				SkinShopUI.Instance.SelectWeaponSkin(weaponSkin);
				GameHandler.Singleton.SkinService.CurrentWeaponSkin = weaponSkin;
			}

			AdLoadingPanel.Instance.HidePanel();
			
			var skinType = this.currentSkin is WeaponSkin ? "WEAPON" : "HERO";
			
			SetActive(false);
			
			AfterSkinRewardUI.Instance.ApplyHeroSkin(GameHandler.Singleton.SkinService.CurrentCharacterSkin);
			AfterSkinRewardUI.Instance.ApplyWeaponSkin(GameHandler.Singleton.SkinService.CurrentWeaponSkin);
			
			AfterSkinRewardUI.Instance.SetHeaderText($"NEW {skinType} UNLOCK!");
			AfterSkinRewardUI.Instance.ShowPanel();
		}

		private void ResetSkippedSkinsIfNewCycle(bool available)
		{
			var notAvailableSkins = new List<Skin>();

			foreach (var skinOpenInfo in SkinRepository.Instance.SkinsOpenInfo)
			{
				if (!skinOpenInfo.Skin.Available)
				{
					notAvailableSkins.Add(skinOpenInfo.Skin);
				}
			}

			if (notAvailableSkins.Exists(s => s == this.currentSkin) &&
			    notAvailableSkins.IndexOf(this.currentSkin) == notAvailableSkins.Count - 1)
			{
				var skippedSkins =
					SkinRepository.Instance.SkinsOpenInfo.Where(s => !s.Skin.Available && s.Skin.Skipped).Select(s => s.Skin);

				foreach (var skippedSkin in skippedSkins)
				{
					if (available && this.currentSkin == skippedSkin)
					{
						continue;
					}
					
					skippedSkin.Skipped = false;
					SkinRepository.Instance.AddOrUpdateSkinData(skippedSkin);
				}
			}
		}

		private void UpdateHeadersText(string text)
		{
			foreach (var header in this.headersText)
			{
				header.text = text;
			}
		}
	}
}