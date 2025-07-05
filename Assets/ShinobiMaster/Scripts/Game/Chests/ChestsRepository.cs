using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Chests
{
	public class ChestsRepository: MonoBehaviour
	{
		public static ChestsRepository Instance { get; private set; }
		
		private const string WoodChestsDataKey = "chestsdata";
	
		private WoodChestsLevelsData woodChestLevelsData;




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
			
			this.woodChestLevelsData = LoadChestsData();
		}
		
		private void OnApplicationQuit()
		{
			SaveChestsData();
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				SaveChestsData();
			}
		}


		public void SaveChestsData()
		{
			var json = JsonUtility.ToJson(this.woodChestLevelsData);
		
			PlayerPrefs.SetString(WoodChestsDataKey, json);
			PlayerPrefs.Save();
		}
		
		private WoodChestsLevelsData LoadChestsData()
		{
			WoodChestsLevelsData data;
		
			if (PlayerPrefs.HasKey(WoodChestsDataKey))
			{
				data = JsonUtility.FromJson<WoodChestsLevelsData>(PlayerPrefs.GetString(WoodChestsDataKey));
			}
			else
			{
				data = new WoodChestsLevelsData();
			}
			
			return data;
		}


		public void AddOrUpdateChestData(int levelNum, int stageNum, WoodChestData woodChestData)
		{
			var data = this.woodChestLevelsData.ChestsLevelsData.SingleOrDefault(d => d.LevelNum == levelNum && d.StageNum == stageNum);

			if (data == null)
			{
				data = new WoodChestLevelData()
				{
					LevelNum = levelNum,
					StageNum = stageNum,
					ChestsData = new List<WoodChestData>
					{
						woodChestData
					},
				};
				
				this.woodChestLevelsData.ChestsLevelsData.Add(data);
			}
			else
			{
				var chestData = data.ChestsData.SingleOrDefault(ch => ch.Name.Equals(woodChestData.Name));

				if (chestData == null)
				{
					data.ChestsData.Add(woodChestData);
				}
				else
				{
					chestData.IsOpened = woodChestData.IsOpened;
					chestData.WithKey = woodChestData.WithKey;
				}
			}
		}

		public WoodChestData GetChestData(int levelNum, int stageNum, string woodChestName)
		{
			var data = this.woodChestLevelsData.ChestsLevelsData.SingleOrDefault(d => d.LevelNum == levelNum && d.StageNum == stageNum);

			var chestData = data?.ChestsData.SingleOrDefault(ch => ch.Name.Equals(woodChestName));

			return chestData;
		}
	}
}