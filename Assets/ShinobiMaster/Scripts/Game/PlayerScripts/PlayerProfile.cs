using System;
using UnityEngine;

namespace Game.PlayerScripts
{
	public class PlayerProfile
	{
		private const string KeysCountKey = "KeysCount";
		private const string SoftCoinsKey = "SoftCoins";
		
		public Action<int, int> OnKeysCountChanged { get; set; }
		public Action<int, int> OnSoftCoinsChanged { get; set; }


		public int KeysCount
		{
			get => PlayerPrefs.GetInt(KeysCountKey, 0);
			set
			{
				if (value == KeysCount || value > 3)
				{
					return;
				}

				var prev = KeysCount;
			
				PlayerPrefs.SetInt(KeysCountKey, value);
				PlayerPrefs.Save();
				
				OnKeysCountChanged?.Invoke(value, prev);
			}
		}

		public int SoftCoins
		{
			get => PlayerPrefs.GetInt(SoftCoinsKey, 0);
			set
			{
				if (value == SoftCoins)
				{
					return;
				}
			
				var prev = SoftCoins;
			
				PlayerPrefs.SetInt(SoftCoinsKey, value);
				PlayerPrefs.Save();
				
				OnSoftCoinsChanged?.Invoke(value, prev);
			}
		}
	}
}