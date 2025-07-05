using System;
using System.Linq;
using UnityEngine;

namespace Skins
{
	public class PlayerSkinService: ISkinService
	{
		private const string CurrentCharacterSkinKey = "CurrChSkin";
		private const string CurrentWeaponSkinKey = "CurrWSkin";
		private const string SkinsKey = "Skins";

		public Action<WeaponSkin> OnCurrentWeaponSkinChanged { get; set; }
		public Action<CharacterSkin> OnCurrentCharacterSkinChanged { get; set; }
		public Action<Skin> OnSkinAdded { get; set; }
		
		
		public CharacterSkin CurrentCharacterSkin
		{
			get => SkinRepository.Instance.CharacterSkins.Single(s => s.Name.Equals(PlayerPrefs.GetString(CurrentCharacterSkinKey, 
			SkinRepository.Instance.DefaultSkins.First(sk => sk is CharacterSkin).Name)));
			set
			{
				if (CurrentCharacterSkin == value)
				{
					return;
				}
			
				PlayerPrefs.SetString(CurrentCharacterSkinKey, value.Name);
				PlayerPrefs.Save();
				
				OnCurrentCharacterSkinChanged?.Invoke(value);
			}
		}

		public WeaponSkin CurrentWeaponSkin
		{
			get => SkinRepository.Instance.WeaponSkins.Single(s => s.Name.Equals(PlayerPrefs.GetString(CurrentWeaponSkinKey, 
			SkinRepository.Instance.DefaultSkins.First(sk => sk is WeaponSkin).Name)));
			set
			{
				if (CurrentWeaponSkin == value)
				{
					return;
				}
			
				PlayerPrefs.SetString(CurrentWeaponSkinKey, value.Name);
				PlayerPrefs.Save();
				
				OnCurrentWeaponSkinChanged?.Invoke(value);
			}
		}



		public PlayerSkinService()
		{
			foreach (var skin in SkinRepository.Instance.DefaultSkins)
			{
				if (!IsSkinInProfile(skin))
				{
					AddSkinInProfile(skin);
				}
			}
		}



		public int GetSkinsInProfileAmount()
		{
			var SkinsInProfileString = GetSkinsInProfileString();

			return SkinsInProfileString.Split(',').Length;
		}
		
		public bool IsSkinInProfile(string skin)
		{
			if (string.IsNullOrEmpty(skin))
			{
				return false;
			}

			var skins = GetSkinsInProfileString().Split(',');

			foreach (var s in skins)
			{
				if (string.IsNullOrWhiteSpace(s))
				{
					continue;
				}

				if (s.Equals(skin))
				{
					return true;
				}
			}
			
			return false;
		} 
		
		public bool IsSkinInProfile(Skin skin)
		{
			return IsSkinInProfile(skin.Name);
		} 
		
		public void BuySkin(string skin)
		{
			AddSkinInProfile(skin);
		}

		public void RemoveSkin(string skin)
		{
			var SkinsInProfileString = GetSkinsInProfileString();
			SkinsInProfileString = SkinsInProfileString
				.Replace(skin, string.Empty)
				.Replace(",,", string.Empty)
				.Trim(',');
			
			AddSkinInProfile(SkinsInProfileString);
		}
		
		public string[] GetBoughtSkins()
		{
			var SkinsInProfileString = GetSkinsInProfileString();
			return SkinsInProfileString.Split(',');
		}
		
		public void AddSkinInProfile(string skin)
		{
			if (IsSkinInProfile(skin))
			{
				return;
			}
			
			var SkinsInProfileString = GetSkinsInProfileString();
			SkinsInProfileString += $"{skin},";
			
			PlayerPrefs.SetString(SkinsKey, SkinsInProfileString);
			PlayerPrefs.Save();
			
			OnSkinAdded?.Invoke(SkinRepository.Instance.Skins.Single(s => s.Name.Equals(skin)));
		}
		
		public void AddSkinInProfile(Skin skin)
		{
			if (IsSkinInProfile(skin.Name))
			{
				return;
			}
			
			AddSkinInProfile(skin.Name);
		}
		
		public string GetSkinsInProfileString()
		{
			var SkinsInProfileString = PlayerPrefs.GetString(SkinsKey, SkinsToString(SkinRepository.Instance.DefaultSkins));
			
			return SkinsInProfileString;
		}

		private string SkinsToString(Skin[] skins)
		{
			return skins.Aggregate(string.Empty, (current, skin) => current + skin.Name + ",");
		}
	}
}