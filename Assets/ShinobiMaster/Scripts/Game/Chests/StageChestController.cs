using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Chests.Items;
using Game.EventGame;
using UnityEngine;

namespace Game.Chests
{
	public class StageChestController: MonoBehaviour
	{
		private const string DroppedHeartsAmountKey = "DroppedHeartsAmount";
	
		public int DroppedHeartsAmount
		{
			get => PlayerPrefs.GetInt(DroppedHeartsAmountKey, 0);
			set
			{
				PlayerPrefs.SetInt(DroppedHeartsAmountKey, value);
				PlayerPrefs.Save();
			}
		}
	
		public static StageChestController Instance { get; private set; }
	
		public KeyChestItem KeyChestItemPrefab;
		public HeartChestItem HeartChestItemPrefab;
		public HeartsSettings HeartsSettings;
		public int droppedHeartsAmountOnStage;



		private void Awake()
		{
			Instance = this;
		
			EventManager.OnStageInited.AddListener(OnStageInited);
			GameHandler.Singleton.Level.LevelComplete += OnLevelComplete;
		}

		private void OnDestroy()
		{
			EventManager.OnStageInited.RemoveListener(OnStageInited);
			GameHandler.Singleton.Level.LevelComplete -= OnLevelComplete;
		}




		private void OnStageInited(Stage stage)
		{
			this.droppedHeartsAmountOnStage = 0;
			
			stage.Chests.OfType<IBreakable>().ToList().ForEach(b => b.OnBreak += OnChestBreak);
		
			StartCoroutine(SetChestItems(stage));
		}

		private void OnChestBreak(IBreakable breakable)
		{
			var woodChest = breakable as WoodChest;

			if (woodChest == null)
			{
				return;
			}
			
			if (woodChest.Item == null)
			{
				if (DroppedHeartsAmount < HeartsSettings.HeartsAmountOnLevel &&
					 GameHandler.Singleton.Player.Health != GameHandler.Singleton.Player.MaxHealth)
				{
					var heartsSettings =
						HeartsSettings.HeartsOnStageList.SingleOrDefault(s =>
							s.Stage == GameHandler.Singleton.Level.CurrStageNumber);

					if (heartsSettings != null && this.droppedHeartsAmountOnStage < heartsSettings.HeartsAmount)
					{
						var chance = Random.value;

						if (chance <= heartsSettings.Chance)
						{
							woodChest.Put(HeartChestItemPrefab);
						}
					}
				}
			}
			
			var chestItem = woodChest.Open();
			
			if (chestItem is KeyChestItem keyChestItem)
			{
				GameHandler.Singleton.PlayerProfile.KeysCount++;
                
				keyChestItem.Key.Disappearance();
			}
			else if (chestItem is HeartChestItem)
			{
				DroppedHeartsAmount++;
				this.droppedHeartsAmountOnStage++;
			}
		}

		private IEnumerator SetChestItems(Stage stage)
		{
			yield return new WaitForSeconds(0.5f);
		
			yield return StartCoroutine(SetKeyForRandomChest(stage));
		}

		private IEnumerator SetKeyForRandomChest(Stage stage)
		{
			var chests = stage.Chests.Where(ch => ch != null && !ch.HasItem()).ToList();

			if (chests.Count <= 0)
			{
				yield break;
			}
			
			var randomChest = chests[Random.Range(0, chests.Count)];

			randomChest.Put(KeyChestItemPrefab);
		}
		
		private IEnumerator SetHeartsForRandomChest(Stage stage)
		{
			var heartsSettings =
				HeartsSettings.HeartsOnStageList.SingleOrDefault(s => s.Stage == GameHandler.Singleton.Level.CurrStageNumber);

			if (heartsSettings == null)
			{
				yield break;
			}

			var maxHeartsAmount = HeartsSettings.HeartsAmountOnLevel - DroppedHeartsAmount;
		
			var heartsAmount = Mathf.Clamp(heartsSettings.HeartsAmount, 0, maxHeartsAmount);

			var chance = heartsSettings.Chance;

			var randomValue = Random.value;

			if (randomValue > chance)
			{
				yield break;
			}

			var chests = stage.Chests.Where(ch => ch != null && !ch.HasItem()).ToList();

			if (chests.Count <= 0)
			{
				yield break;
			}
			
			var chestIdx = new List<int>();
			
			for (var i = 0; i < chests.Count; i++)
			{
				chestIdx.Add(i);
			}

			for (var i = 0; i < heartsAmount; i++)
			{
				if (chestIdx.Count == 0)
				{
					yield break;
				}
			
				var randomIdx = chestIdx[Random.Range(0, chestIdx.Count)];

				var randomChest = chests[randomIdx];
				
				randomChest.Put(HeartChestItemPrefab);

				chestIdx.Remove(randomIdx);

				yield return null;
			}
		}
		
		private void OnLevelComplete(bool skip)
		{
			DroppedHeartsAmount = 0;
		}

		private IEnumerator LoadChestDataProcess(Stage stage)
		{
			if (stage.Chests.Count > 0)
			{
				WoodChestData chestWithKeyData = null;
			
				foreach (var chest in stage.Chests)
				{
					var chestData = ChestsRepository.Instance.GetChestData(GameHandler.Singleton.Level.GetNumLevel(), 
						GameHandler.Singleton.Level.CurrStageNumber, chest.name);

					if (chestData == null)
					{
						chestData = new WoodChestData
						{
							Name = chest.name,
							IsOpened = false,
							WithKey = false
						};

						ChestsRepository.Instance.AddOrUpdateChestData(GameHandler.Singleton.Level.GetNumLevel(), 
							GameHandler.Singleton.Level.CurrStageNumber,chestData);
					}
					else
					{
						if (chestData.WithKey)
						{
							chestWithKeyData = chestData;
						}
					
						if (chestData.IsOpened)
						{
							Destroy(chest.gameObject);
						}
					}
				}

				yield return null;

				if (chestWithKeyData == null)
				{
					var chests = stage.Chests.Where(ch => ch != null).ToList();

					var randomChest = chests[Random.Range(0, chests.Count)];

					randomChest.Put(KeyChestItemPrefab);

					var chestData = ChestsRepository.Instance.GetChestData(GameHandler.Singleton.Level.GetNumLevel(),
						GameHandler.Singleton.Level.CurrStageNumber, randomChest.name);

					chestData.WithKey = true;

					ChestsRepository.Instance.AddOrUpdateChestData(GameHandler.Singleton.Level.GetNumLevel(), 
						GameHandler.Singleton.Level.CurrStageNumber, chestData);
				}
				else
				{
					if (!chestWithKeyData.IsOpened)
					{
						var chestWithKey = stage.Chests.SingleOrDefault(ch => ch != null && ch.name.Equals(chestWithKeyData.Name));

						if (chestWithKey != null)
						{
							chestWithKey.Put(KeyChestItemPrefab);
						}
					}
				}
			}
		}
	}
}