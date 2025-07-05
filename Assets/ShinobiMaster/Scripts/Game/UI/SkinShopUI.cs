using System;
using System.Linq;
using Skins;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	[Serializable]
	public class PageHeroSkins
	{
		public GameObject Page;
		public HeroSkinUI[] Skins;
	}
	[Serializable]
	public class PageWeaponSkins
	{
		public GameObject Page;
		public WeaponSkinUI[] Skins;
	}

	public class SkinShopUI: MonoBehaviour
	{
		public static SkinShopUI Instance;

		[SerializeField] private GameObject hero;
		[SerializeField] private GameObject shadowUnderHero;
		[SerializeField] private GameObject heroesSkinShopPanel;
		[SerializeField] private GameObject weaponsSkinShopPanel;
		[SerializeField] private Camera mainCamera;
		[SerializeField] private Camera heroesCamera;
		[SerializeField] private Canvas canvas;
		[SerializeField] private SkinnedMeshRenderer heroRender;
		[SerializeField] private Material heroMaterial;
		[SerializeField] private float heroMatOutline;
		[SerializeField] private GameObject[] heroPages;
		[SerializeField] private GameObject[] weaponPages;
		 private Image[] heroPagesIcon;
		 private Image[] weaponPagesIcon;
		[SerializeField] private Transform heroPagesIconParent;
		[SerializeField] private Transform weaponPagesIconParent;
		[SerializeField] private Image pageIconPrefab;
		[SerializeField] private Color activePageIconColor;
		[SerializeField] private Color inactivePageIconColor;
		[SerializeField] private HeroPointerHandler HeroPointerHandler;
		[SerializeField] private WeaponPointerHandler WeaponPointerHandler;
		[SerializeField] private Transform weaponSlot;
		[SerializeField] private Transform skinShopPanel;
		[SerializeField] private HeroSkinUI[] heroSkinsUI;
		[SerializeField] private WeaponSkinUI[] weaponSkinsUI;
		[SerializeField] private PageWeaponSkins[] pagesWeaponSkins;
		[SerializeField] private PageHeroSkins[] pagesHeroSkins;
		[SerializeField] private GameObject[] leftArrowsHeroSkins;
		[SerializeField] private GameObject[] rightArrowsHeroSkins;
		[SerializeField] private GameObject[] leftArrowsWeaponSkins;
		[SerializeField] private GameObject[] rightArrowsWeaponSkins;
		public CharacterSkin CurrentCharacterSkin { get; private set; }
		public WeaponSkin CurrentWeaponSkin { get; private set; }
		private int heroCurrentPageIdx;
		private int weaponCurrentPageIdx;
		private HeroSkinUI currentCharacterSkinUI;
		private WeaponSkinUI currentWeaponSkinUI;
		private static readonly int heroOutline = Shader.PropertyToID("_Outline");
		private Material heroShopMaterial;
		


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
			
			this.heroPagesIcon = new Image[this.heroPages.Length];
			this.weaponPagesIcon = new Image[this.weaponPages.Length];
			
			HeroPointerHandler.OnSwipe += HeroPointerHandlerOnSwipe;
			WeaponPointerHandler.OnSwipe += WeaponPointerHandlerOnSwipe;
		}

		private void Start()
		{
			this.heroShopMaterial = Instantiate(this.heroMaterial);
			this.heroShopMaterial.SetFloat(heroOutline, this.heroMatOutline);
		
			CurrentCharacterSkin = GameHandler.Singleton.SkinService.CurrentCharacterSkin;
			CurrentWeaponSkin = GameHandler.Singleton.SkinService.CurrentWeaponSkin;

			foreach (var heroSkin in this.heroSkinsUI)
			{
				heroSkin.Button.onClick.AddListener(delegate
				{
					OnClickHeroSkinButton(heroSkin);
				});
			}

			foreach (var weaponSkin in this.weaponSkinsUI)
			{
				weaponSkin.Button.onClick.AddListener(delegate
				{
					OnClickWeaponSkinButton(weaponSkin);
				});
			}
		
			var heroSkinUi = this.heroSkinsUI.Single(s => s.CharacterSkin == CurrentCharacterSkin);
			
			SelectCharacterSkin(heroSkinUi);

			var weaponSkiUi = this.weaponSkinsUI.Single(s => s.WeaponSkin == CurrentWeaponSkin);
			
			SelectWeaponSkin(weaponSkiUi);
		
			for (var i = 0; i < this.heroPages.Length; i++)
			{
				var pageIcon = Instantiate(this.pageIconPrefab, this.heroPagesIconParent);

				pageIcon.color = this.inactivePageIconColor;

				this.heroPagesIcon[i] = pageIcon;
			}

			ApplyCharacterSkin(CurrentCharacterSkin);
			
			for (var i = 0; i < this.weaponPages.Length; i++)
			{
				var pageIcon = Instantiate(this.pageIconPrefab, this.weaponPagesIconParent);

				pageIcon.color = this.inactivePageIconColor;

				this.weaponPagesIcon[i] = pageIcon;
			}

			ApplyWeaponSkin(CurrentWeaponSkin);
			
			var currentHeroPage = this.pagesHeroSkins.Single(p => p.Skins.Contains(currentCharacterSkinUI)).Page;

			var currentHeroPageIdx = this.heroPages.ToList().IndexOf(currentHeroPage);
			
			SetHeroCurrentPage(currentHeroPageIdx);
			
			var currentWeaponPage = this.pagesWeaponSkins.Single(p => p.Skins.Contains(currentWeaponSkinUI)).Page;

			var currentWeaponPageIdx = this.weaponPages.ToList().IndexOf(currentWeaponPage);
			
			SetWeaponCurrentPage(currentWeaponPageIdx);
			
			GameHandler.Singleton.Player.ApplySkin(CurrentCharacterSkin);
			GameHandler.Singleton.Player.ApplyWeaponSkin(CurrentWeaponSkin);
		}




		public void OpenHeroesSkinShop()
		{
			SetActiveHeroCamera(true);
			SetActiveHero(true);
			SetActiveShadowUnderHero(true);
			this.heroesSkinShopPanel.SetActive(true);
		}
		
		public void CloseHeroesSkinShop()
		{
			SetActiveHeroCamera(false);
			SetActiveHero(false);
			SetActiveShadowUnderHero(false);
			this.heroesSkinShopPanel.SetActive(false);
		}

		public void SetActiveHeroCamera(bool active)
		{
			this.canvas.worldCamera = active ? this.heroesCamera : this.mainCamera;
			this.heroesCamera.enabled = active;
		}
		
		public void OpenWeaponsSkinShop()
		{
			SetActiveHeroCamera(true);
			SetActiveHero(true);
			SetActiveShadowUnderHero(true);
			this.weaponsSkinShopPanel.SetActive(true);
		}
		
		public void CloseWeaponsSkinShop()
		{
			SetActiveHeroCamera(false);
			SetActiveHero(false);
			SetActiveShadowUnderHero(false);
			this.weaponsSkinShopPanel.SetActive(false);
		}

		public void ApplyCharacterSkin(CharacterSkin characterSkin)
		{
			this.CurrentCharacterSkin = characterSkin;
		
			this.heroRender.sharedMesh =
				CurrentCharacterSkin.SkinPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;

			this.heroRender.material = characterSkin.SkinMat;
		}
		
		public void ApplyWeaponSkin(WeaponSkin weaponSkin)
		{
			this.CurrentWeaponSkin = weaponSkin;

			if (this.weaponSlot.transform.childCount > 0)
			{
				Destroy(this.weaponSlot.GetChild(0).gameObject);
			}

			var weapon = Instantiate(CurrentWeaponSkin.SkinPrefab, this.weaponSlot);
			
			var rend = weapon.GetComponent<MeshRenderer>();

			if (rend != null && (rend.material.name.StartsWith("mat_player") || rend.material.name.StartsWith("mat_weapon")))
			{
				rend.material.SetFloat(heroOutline, this.heroMatOutline);
			}

			var glow = weapon.transform.Find("Glow");

			if (glow != null)
			{
				var glowRenderers = glow.GetComponentsInChildren<Renderer>();

				foreach (var renderer in glowRenderers)
				{
					renderer.sortingOrder = 15;
				}
			}
		}
		
		private void OnClickHeroSkinButton(HeroSkinUI heroSkinUi)
		{
			if (heroSkinUi.CharacterSkin == null || !heroSkinUi.CharacterSkin.Available)
			{
				return;
			}
		
		    ApplyCharacterSkin(heroSkinUi.CharacterSkin);
			
		    GameHandler.Singleton.Player.ApplySkin(heroSkinUi.CharacterSkin);

			GameHandler.Singleton.SkinService.CurrentCharacterSkin = heroSkinUi.CharacterSkin;
			
			AudioManager.Instance.PlayClickButtonSound();
			
			SelectCharacterSkin(heroSkinUi.CharacterSkin);
		}

		public void NextHeroPage()
		{
			SetHeroCurrentPage(this.heroCurrentPageIdx + 1);
		}

		public void PrevHeroPage()
		{
			SetHeroCurrentPage(this.heroCurrentPageIdx - 1);
		}

		public void NextWeaponPage()
		{
			SetWeaponCurrentPage(this.weaponCurrentPageIdx + 1);
		}

		public void PrevWeaponPage()
		{
			SetWeaponCurrentPage(this.weaponCurrentPageIdx - 1);
		}

		public void SetHeroCurrentPage(int idx)
		{
			if (idx < 0 || idx > this.heroPages.Length - 1)
			{
				return;
			}
		
			this.heroCurrentPageIdx = idx;
			
			UpdateHeroSkinsArrows();

			for (var i = 0; i < this.heroPages.Length; i++)
			{
				this.heroPages[i].SetActive(i == idx);
			}
			
			for (var i = 0; i < this.heroPagesIcon.Length; i++)
			{
				this.heroPagesIcon[i].color = i == idx ? this.activePageIconColor : this.inactivePageIconColor;
			}
		}
		
		public void SetWeaponCurrentPage(int idx)
		{
			if (idx < 0 || idx > this.weaponPages.Length - 1)
			{
				return;
			}
		
			this.weaponCurrentPageIdx = idx;
			
			UpdateWeaponSkinsArrows();

			for (var i = 0; i < this.weaponPages.Length; i++)
			{
				this.weaponPages[i].SetActive(i == idx);
			}
			
			for (var i = 0; i < this.weaponPagesIcon.Length; i++)
			{
				this.weaponPagesIcon[i].color = i == idx ? this.activePageIconColor : this.inactivePageIconColor;
			}
		}

		public void SetActiveSkinShopPanel(bool active)
		{
			this.skinShopPanel.gameObject.SetActive(active);
		}

		public void SetActiveHero(bool active)
		{
			this.hero.SetActive(active);
		}

		public void SetActiveShadowUnderHero(bool active)
		{
			this.shadowUnderHero.SetActive(active);
		}

		public void SelectCharacterSkin(HeroSkinUI heroSkin)
		{
			if (heroSkin.CharacterSkin == null || !heroSkin.CharacterSkin.Available)
			{
				return;
			}
		
			if (this.currentCharacterSkinUI != null)
			{
				this.currentCharacterSkinUI.SetSelected(false);
			}

			this.currentCharacterSkinUI = heroSkin;
			
			this.currentCharacterSkinUI.SetSelected(true);
		}

		public void SelectCharacterSkin(CharacterSkin characterSkin)
		{
			SelectCharacterSkin(this.heroSkinsUI.Single(s => s.CharacterSkin == characterSkin));
		}
		
		public void SelectWeaponSkin(WeaponSkin weaponSkin)
		{
			SelectWeaponSkin(this.weaponSkinsUI.Single(s => s.WeaponSkin == weaponSkin));
		}
		
		private void OnClickWeaponSkinButton(WeaponSkinUI weaponSkinUi)
		{
			if (weaponSkinUi.WeaponSkin == null || !weaponSkinUi.WeaponSkin.Available)
			{
				return;
			}
		
			ApplyWeaponSkin(weaponSkinUi.WeaponSkin);

			GameHandler.Singleton.Player.ApplyWeaponSkin(weaponSkinUi.WeaponSkin);
			
			GameHandler.Singleton.SkinService.CurrentWeaponSkin = weaponSkinUi.WeaponSkin;
			
			AudioManager.Instance.PlayClickButtonSound();
			
			SelectWeaponSkin(weaponSkinUi.WeaponSkin);
		}

		private void SelectWeaponSkin(WeaponSkinUI weaponSkin)
		{
			if (weaponSkin.WeaponSkin == null || !weaponSkin.WeaponSkin.Available)
			{
				return;
			}
		
			if (this.currentWeaponSkinUI != null)
			{
				this.currentWeaponSkinUI.SetSelected(false);
			}

			this.currentWeaponSkinUI = weaponSkin;
			
			this.currentWeaponSkinUI.SetSelected(true);
		}
		
		private void HeroPointerHandlerOnSwipe(Dir dir)
		{
			var delta = dir == Dir.Right ? 1 : -1;

			var idx = this.heroCurrentPageIdx + delta;
			
			if (idx < 0 || idx > this.heroPages.Length - 1)
			{
				return;
			}
			
			AudioManager.Instance.PlayClickButtonSound();
		
			SetHeroCurrentPage(idx);
		}

		private void UpdateHeroSkinsArrows()
		{
			if (this.heroCurrentPageIdx == 0)
			{
				foreach (var arrow in this.leftArrowsHeroSkins)
				{
					arrow.SetActive(false);
				}
			
				foreach (var arrow in this.rightArrowsHeroSkins)
				{
					arrow.SetActive(true);
				}
			} 
			else if (this.heroCurrentPageIdx == this.heroPages.Length - 1)
			{
				foreach (var arrow in this.leftArrowsHeroSkins)
				{
					arrow.SetActive(true);
				}
			
				foreach (var arrow in this.rightArrowsHeroSkins)
				{
					arrow.SetActive(false);
				}
			}
			else
			{
				foreach (var arrow in this.leftArrowsHeroSkins)
				{
					arrow.SetActive(true);
				}
			
				foreach (var arrow in this.rightArrowsHeroSkins)
				{
					arrow.SetActive(true);
				}
			}
		}
		
		private void UpdateWeaponSkinsArrows()
		{
			if (this.weaponCurrentPageIdx == 0)
			{
				foreach (var arrow in this.leftArrowsWeaponSkins)
				{
					arrow.SetActive(false);
				}
			
				foreach (var arrow in this.rightArrowsWeaponSkins)
				{
					arrow.SetActive(true);
				}
			} 
			else if (this.weaponCurrentPageIdx == this.weaponPages.Length - 1)
			{
				foreach (var arrow in this.leftArrowsWeaponSkins)
				{
					arrow.SetActive(true);
				}
			
				foreach (var arrow in this.rightArrowsWeaponSkins)
				{
					arrow.SetActive(false);
				}
			}
			else
			{
				foreach (var arrow in this.leftArrowsWeaponSkins)
				{
					arrow.SetActive(true);
				}
			
				foreach (var arrow in this.rightArrowsWeaponSkins)
				{
					arrow.SetActive(true);
				}
			}
		}
		
		private void WeaponPointerHandlerOnSwipe(Dir dir)
		{
			var delta = dir == Dir.Right ? 1 : -1;
			
			var idx = this.weaponCurrentPageIdx + delta;
			
			if (idx < 0 || idx > this.weaponPages.Length - 1)
			{
				return;
			}
			
			AudioManager.Instance.PlayClickButtonSound();
		
			SetWeaponCurrentPage(idx);
		}
	}
}