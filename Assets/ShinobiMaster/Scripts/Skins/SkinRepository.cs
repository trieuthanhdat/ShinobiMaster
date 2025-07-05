using System;
using System.Linq;
using UnityEngine;

namespace Skins
{
	[Serializable]
	public class SkinOpenInfo
	{
		public int Level;
		public int Stage;
		public Skin Skin;
	}

	public class SkinRepository: MonoBehaviour
	{
		public static SkinRepository Instance { get; private set; }

		public const string SkinsDataKey = "skinsdata";

		public Skin[] DefaultSkins => this.defaultSkins;
		public CharacterSkin[] CharacterSkins => this.characterSkins;
		public WeaponSkin[] WeaponSkins => this.weaponSkins;
		public Skin[] Skins;
	
		[SerializeField] private CharacterSkin[] characterSkins;
		[SerializeField] Skin[] defaultSkins;
		[SerializeField] private WeaponSkin[] weaponSkins;
		public SkinOpenInfo[] SkinsOpenInfo;
		private SkinsData skinsData;
		public int SkinStage1;
		public int SkinStage2;
		
		
		
		
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				this.transform.SetParent(null);
				DontDestroyOnLoad(this);
			}
			else 
			{
				if (Instance != this)
				{
					Destroy(this.gameObject);
                    
					return;
				}
                
				return;
			}
			
			Skins = new Skin[CharacterSkins.Length + WeaponSkins.Length];

			Array.ConstrainedCopy(CharacterSkins, 0, Skins, 0, CharacterSkins.Length);
			Array.ConstrainedCopy(WeaponSkins, 0, Skins, CharacterSkins.Length, WeaponSkins.Length);
			
			this.skinsData = LoadSkinData();
			
			foreach (var skin in CharacterSkins)
			{
				var skinData = this.skinsData.Skins.SingleOrDefault(s => s.Name.Equals(skin.Name));

				if (skinData == null)
				{
					continue;
				}
				
				skin.UpdateData(skinData);
			}
			
			foreach (var skin in WeaponSkins)
			{
				var skinData = this.skinsData.Skins.SingleOrDefault(s => s.Name.Equals(skin.Name));

				if (skinData == null)
				{
					continue;
				}
				
				skin.UpdateData(skinData);
			}
		}
		
		
		private void OnApplicationQuit()
		{
			SaveSkinData();
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				SaveSkinData();
			}
		}
		
		public void SaveSkinData()
		{
			var json = JsonUtility.ToJson(this.skinsData);
		
			PlayerPrefs.SetString(SkinsDataKey, json);
			PlayerPrefs.Save();
		}
		
		private SkinsData LoadSkinData()
		{
			SkinsData data;
		
			if (PlayerPrefs.HasKey(SkinsDataKey))
			{
				data = JsonUtility.FromJson<SkinsData>(PlayerPrefs.GetString(SkinsDataKey));
			}
			else
			{
				data = new SkinsData();
			}
			
			return data;
		}


		public void AddOrUpdateSkinData(Skin skin)
		{
			var skinData = this.skinsData.Skins.SingleOrDefault(s => s.Name.Equals(skin.Name));
		
			if (skinData == null)
			{
				skinData = new SkinData
				{
					Name = skin.Name
				};

				this.skinsData.Skins.Add(skinData);
			}
			
			skinData.Available = skin.Available;
			skinData.Skipped = skin.Skipped;
			skinData.AdsViewCount = skin.CurrentViewCount;
		}


		public Skin GetSkinForOpen(int level, int stage)
		{
			var skin = SkinsOpenInfo.SingleOrDefault(s => s.Level == level && s.Stage == stage && !s.Skin.Skipped)?.Skin;

			var skinOpenInfo = SkinsOpenInfo.FirstOrDefault(s => s.Level == level);

			if (skinOpenInfo == null)
			{
				if (stage == SkinStage1 || stage == SkinStage2)
				{
					skin = SkinsOpenInfo.FirstOrDefault(s => !s.Skin.Available && !s.Skin.Skipped)?.Skin;
				}
			}

			return skin;
		}

		public int GetPrevSkinStage(int level, int stage)
		{
			var st = 0;
		
			SkinOpenInfo skinOpenInfo = null;

			for (var i = 0; i < SkinsOpenInfo.Length; i++)
			{
				var skinOpen = SkinsOpenInfo[i];
			
				if (skinOpen.Level == level && skinOpen.Stage < stage)
				{
					skinOpenInfo = skinOpen;
					
					break;
				}
			}

			if (skinOpenInfo != null)
			{
				st = skinOpenInfo.Stage;
			}
			
			skinOpenInfo = SkinsOpenInfo.FirstOrDefault(s => s.Level == level);

			if (skinOpenInfo == null)
			{
				if (stage <= SkinStage2)
				{
					st = SkinStage1;
				}

				if (stage <= SkinStage1)
				{
					st = 0;
				}
			}

			return st;
		}
		
		public Skin GetNextSkinForOpen(int level, int stage, out int skinStage)
		{
			skinStage = SkinStage1;

			Skin skin = null;

			var skinOpenInfo = SkinsOpenInfo.FirstOrDefault(s => s.Level == level && s.Stage >= stage);

			if (skinOpenInfo != null)
			{
				skinStage = skinOpenInfo.Stage;
			
				var skinOpenInfoIdx = SkinsOpenInfo.ToList().IndexOf(skinOpenInfo);
				
				skinOpenInfo = null;

				for (var i = skinOpenInfoIdx; i < SkinsOpenInfo.Length; i++)
				{
					var skinOpen = SkinsOpenInfo[i];

					if (!skinOpen.Skin.Available && !skinOpen.Skin.Skipped)
					{
						skinOpenInfo = skinOpen;

						break;
					}
				}
				
				if (skinOpenInfo != null)
				{
					skin = skinOpenInfo.Skin;
				}
			}
			else
			{
				if (stage <= SkinStage2)
				{
					skinStage = SkinStage2;
				}

				if (stage <= SkinStage1)
				{
					skinStage = SkinStage1;
				}

				skin = SkinsOpenInfo.FirstOrDefault(s => !s.Skin.Available && !s.Skin.Skipped)?.Skin;
			}

			return skin;
		}
	}
}