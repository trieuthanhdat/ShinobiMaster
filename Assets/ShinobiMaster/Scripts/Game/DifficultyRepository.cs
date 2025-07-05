using System;
using UnityEngine;

namespace Game
{
	[Serializable]
	public class DifficultyConfig
	{
		public float AttackSpeed;
		public float ProjectileVelocity;
		public Vector2 StartAttackDelay;
		public float SlowTimeScale;
		public float AttackCancellationRadius;
	}

	public class DifficultyRepository: MonoBehaviour
	{
		public static DifficultyRepository Instance { get; private set; }

		public const string CurrentDifficultyConfigKey = "diffconf";
	
	
		[field:SerializeField] public DifficultyConfig[] DifficultyConfigs { get; set; }

		public int CurrentDifficultyConfig
		{
			get => PlayerPrefs.GetInt(CurrentDifficultyConfigKey, 0);
			set
			{
				if (value < 0)
				{
					value = 0;
				}

				if (value > DifficultyConfigs.Length - 1)
				{
					value = DifficultyConfigs.Length - 1;
				}
			
				PlayerPrefs.SetInt(CurrentDifficultyConfigKey, value);
				PlayerPrefs.Save();
			}
		}




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
		}



		public DifficultyConfig GetCurrentDifficultyConfig()
		{
			return DifficultyConfigs[CurrentDifficultyConfig];
		}
	}
}