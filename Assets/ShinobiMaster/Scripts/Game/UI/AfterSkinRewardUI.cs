using System;
using Skins;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class AfterSkinRewardUI: MonoBehaviour
	{
		private static readonly int Outline = Shader.PropertyToID("_Outline");
		public static AfterSkinRewardUI Instance;

		public Action OnPanelHidden;
	
		[SerializeField] private GameObject hero;
		[SerializeField] private GameObject shadowUnderHero;
		[SerializeField] private Camera heroCamera;
		[SerializeField] private Camera mainCamera;
		[SerializeField] private Canvas canvas;
		[SerializeField] private SkinnedMeshRenderer heroRender;
		[SerializeField] private Material heroMat;
		[SerializeField] private float heroOutline;
		[SerializeField] private Transform weaponSlot;
		[SerializeField] private Text headerText;
		[SerializeField] private GameObject panel;

		private Material heroRewardMaterial;
		


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
			this.heroRewardMaterial = Instantiate(this.heroMat);
			this.heroRewardMaterial.SetFloat(Outline, this.heroOutline);

			this.heroRender.material = this.heroRewardMaterial;
		}


		public void ShowPanel()
		{
			SetActiveHeroCamera(true);
			SetActiveHero(true);
			SetActiveShadowUnderHero(true);
			this.panel.SetActive(true);
		}

		public void HidePanel()
		{
			SetActiveHeroCamera(false);
			SetActiveHero(false);
			SetActiveShadowUnderHero(false);
			this.panel.SetActive(false);
			
			OnPanelHidden?.Invoke();
		}
		
		
		
		public void SetActiveHero(bool active)
		{
			this.hero.SetActive(active);
		}

		public void SetActiveShadowUnderHero(bool active)
		{
			this.shadowUnderHero.SetActive(active);
		}

		public void SetActiveHeroCamera(bool active)
		{
			this.canvas.worldCamera = active ? this.heroCamera : this.mainCamera;
			this.heroCamera.enabled = active;
		}
		
		public void ApplyHeroSkin(CharacterSkin characterSkin)
		{
			this.heroRender.sharedMesh =
				characterSkin.SkinPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
			this.heroRender.material = characterSkin.SkinMat;

        }
		
		public void ApplyWeaponSkin(WeaponSkin weaponSkin)
		{
			if (this.weaponSlot.transform.childCount > 0)
			{
				Destroy(this.weaponSlot.GetChild(0).gameObject);
			}

			var weapon = Instantiate(weaponSkin.SkinPrefab, this.weaponSlot);

			var rend = weapon.GetComponent<MeshRenderer>();

			if (rend != null && (rend.material.name.StartsWith("mat_player") || rend.material.name.StartsWith("mat_weapon")))
			{
				rend.material.SetFloat(Outline, this.heroOutline);
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

		public void SetHeaderText(string text)
		{
			this.headerText.text = text;
		}
	}
}