using System.Collections.Generic;
using Skins;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class HealthUI: MonoBehaviour
	{
		private Player player;
		[SerializeField]
		private HeartUI heartPrefab;
		[SerializeField]
		private Transform panel;
		private List<HeartUI> hearts;
		[SerializeField]
		private Image currSkinImage;



		private void Awake()
		{
			this.player = GameHandler.Singleton.Player;
		
			FillHeartsPanel();
			UpdateHeartsVisibility();
			
			player.OnHealthChanged += OnHealthChanged;
		}

		private void OnEnable()
		{
			UpdateHeartsVisibility();
		}

		private void Start()
		{
			UpdateCurrSkinSprite();
			
			GameHandler.Singleton.SkinService.OnCurrentCharacterSkinChanged += OnCurrentCharacterSkinChanged;
		}

		private void OnDestroy()
		{
			player.OnHealthChanged -= OnHealthChanged;
			GameHandler.Singleton.SkinService.OnCurrentCharacterSkinChanged -= OnCurrentCharacterSkinChanged;
		}



		private void UpdateHeartsVisibility()
		{
			for (var i = 0; i < player.MaxHealth; i++)
			{
				this.hearts[i].SetHeartVisibility(i < player.Health);
			}
		}

		private void FillHeartsPanel()
		{
			this.hearts = new List<HeartUI>();
		
			for (var i = 0; i < player.MaxHealth; i++)
			{
				var heart = Instantiate(this.heartPrefab, this.panel);

				this.hearts.Add(heart);
			}
		}

		private void UpdateCurrSkinSprite()
		{
			this.currSkinImage.sprite = GameHandler.Singleton.SkinService.CurrentCharacterSkin.IconSprite;
			this.currSkinImage.preserveAspect = true;
		}
		
		
		private void OnHealthChanged(int health)
		{
			UpdateHeartsVisibility();
		}
		
		private void OnCurrentCharacterSkinChanged(CharacterSkin skin)
		{
			UpdateCurrSkinSprite();
		}
	}
}