using System;
using Game.Enemy.Weapon;

namespace Skins
{
	public interface ISkinService
	{
		Action<WeaponSkin> OnCurrentWeaponSkinChanged { get; set; }
		Action<CharacterSkin> OnCurrentCharacterSkinChanged { get; set; }
		Action<Skin> OnSkinAdded { get; set; }
	
		CharacterSkin CurrentCharacterSkin { get; set; }
		WeaponSkin CurrentWeaponSkin { get; set; }
		int GetSkinsInProfileAmount();
		bool IsSkinInProfile(string skin);
		bool IsSkinInProfile(Skin skin);
		void BuySkin(string skin);
		void RemoveSkin(string skin);
		string[] GetBoughtSkins();
		void AddSkinInProfile(string skin);
		void AddSkinInProfile(Skin skin);
		string GetSkinsInProfileString();
	}
}