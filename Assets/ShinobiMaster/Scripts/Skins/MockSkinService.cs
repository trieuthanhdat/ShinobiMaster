using System;

namespace Skins
{
	public class MockSkinService: ISkinService
	{
		public Action<WeaponSkin> OnCurrentWeaponSkinChanged { get; set; }
		public Action<CharacterSkin> OnCurrentCharacterSkinChanged { get; set; }
		public Action<Skin> OnSkinAdded { get; set; }
		public CharacterSkin CurrentCharacterSkin { get; set; }
		public WeaponSkin CurrentWeaponSkin { get; set; }


		public int GetSkinsInProfileAmount()
		{
			throw new System.NotImplementedException();
		}

		public bool IsSkinInProfile(string skin)
		{
			throw new System.NotImplementedException();
		}

		public bool IsSkinInProfile(Skin skin)
		{
			throw new System.NotImplementedException();
		}

		public void BuySkin(string skin)
		{
			throw new System.NotImplementedException();
		}

		public void RemoveSkin(string skin)
		{
			throw new System.NotImplementedException();
		}

		public string[] GetBoughtSkins()
		{
			throw new System.NotImplementedException();
		}

		public void AddSkinInProfile(string skin)
		{
			throw new System.NotImplementedException();
		}

		public void AddSkinInProfile(Skin skin)
		{
			throw new System.NotImplementedException();
		}

		public string GetSkinsInProfileString()
		{
			throw new System.NotImplementedException();
		}
	}
}