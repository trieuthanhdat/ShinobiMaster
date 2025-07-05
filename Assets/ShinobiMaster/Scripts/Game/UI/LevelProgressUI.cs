using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.LevelControll;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	[Serializable]
	public class BossIcon
	{
		public Enemy.Enemy BossPrefab;
		public BossProgressIcon BossProgressIconPrefab;
	}

	[Serializable]
	public class LevelBackground
	{
		public Image TopPattern;
		public Image BottomPattern;
		public Image LevelImage;
	}

	public class LevelProgressUI: MonoBehaviour
	{
		public static LevelProgressUI Singleton { get; private set; }
	
		public static bool ShowOnLaunch = true;
		private List<BossProgressIcon> bossProgressIcons;
		public RectTransform LocationProgressPanel;
		public RectTransform BossProgressPanel;
		public List<RectTransform> Locations;
		public List<LevelBackground> LevelBackgrounds;
		public List<BossIcon> BossIcons;
		public Image LinePrefab;
		public List<LevelParams> LevelParams;
		
		


		private void Awake()
		{
			if (Singleton)
			{
				Destroy(gameObject);
				return;
			}
			else
			{
				Singleton = this;
			}
			
			this.bossProgressIcons = new List<BossProgressIcon>();
		}
		
		private void Start()
		{
			StaticGameObserver.LoadStartProgress(out var currLvl, out var s);

			currLvl--;
		
			if (ShowOnLaunch)
			{
				UpdateLevelProgress(currLvl, s);
				
				ShowOnLaunch = false;
			}
			else
			{
				StartCoroutine(UpdateLevelProgressProcess(currLvl, s,2.0f));
			}
		}

		public void UpdateLevelProgress(int level, int stage)
		{
			this.bossProgressIcons.Clear();

			foreach (Transform tr in BossProgressPanel.transform)
			{
				Destroy(tr.gameObject);
			}
		
			var lineWidth = LinePrefab.rectTransform.rect.width;

			var lineOffset = lineWidth/2f;

			var prevLevelBosses = GameHandler.Singleton.Level.GetPrevLevelBosses();
			
			var currLvlOffset = prevLevelBosses.Count;

			for (var i = 0; i < prevLevelBosses.Count; i++)
			{
				var bossIcon = Instantiate(
					BossIcons.Single(b => b.BossPrefab.name.Equals(prevLevelBosses[i])).BossProgressIconPrefab,
					BossProgressPanel);

				var bossIconRectTr = bossIcon.GetComponent<RectTransform>();

				bossIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(i * lineWidth, 0);

				var line = Instantiate(LinePrefab, BossProgressPanel);

				line.transform.SetAsFirstSibling();

				line.rectTransform.anchoredPosition = bossIconRectTr.anchoredPosition + new Vector2(lineOffset, 0);

				this.bossProgressIcons.Add(bossIcon);
			}

			var bossIcon2 = Instantiate(BossIcons.Single(b => b.BossPrefab.name.Equals(GameHandler.Singleton.Level.bossPrefab.name)).BossProgressIconPrefab,
				BossProgressPanel);

			var bossIconRectTr2 = bossIcon2.GetComponent<RectTransform>();

			bossIcon2.GetComponent<RectTransform>().anchoredPosition = new Vector3(prevLevelBosses.Count * lineWidth, 0);

			var line2 = Instantiate(LinePrefab, BossProgressPanel);
				
			line2.transform.SetAsFirstSibling();
				
			line2.rectTransform.anchoredPosition = bossIconRectTr2.anchoredPosition + new Vector2(lineOffset, 0);
					
			this.bossProgressIcons.Add(bossIcon2);

			for (var i = 0; i < 2; i++)
			{
				var bossIcon = Instantiate(BossIcons[i].BossProgressIconPrefab,
					BossProgressPanel);

				var bossIconRectTr = bossIcon.GetComponent<RectTransform>();

				bossIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3((prevLevelBosses.Count + 1) * lineWidth + i * lineWidth, 0);

				var line = Instantiate(LinePrefab, BossProgressPanel);
				
				line.transform.SetAsFirstSibling();
				
				line.rectTransform.anchoredPosition = bossIconRectTr.anchoredPosition + new Vector2(lineOffset, 0);
					
				this.bossProgressIcons.Add(bossIcon);
			}
			
			LevelBackgrounds[0].LevelImage.sprite = GameHandler.Singleton.Level.LevelMenuBackground;
			LevelBackgrounds[0].TopPattern.color = GameHandler.Singleton.Level.LevelMenuColor;
			LevelBackgrounds[0].BottomPattern.color = GameHandler.Singleton.Level.LevelMenuColor;
			
			var width = ((Screen.width / (float) Screen.height) / (1920f / 1080f)) * 1920f;
			
			for (var i = 0; i < Locations.Count; i++)
			{
				Locations[i].offsetMin = new Vector2(i * width, 0f);
				Locations[i].offsetMax = new Vector2(i * width, 0f);
			}

			var bossLevel = level - currLvlOffset;

			for (var i = 0; i < bossProgressIcons.Count; i++)
			{
				bossProgressIcons[i].StageProgressText.text = stage + "/7";
				bossProgressIcons[i].StageProgressPanel.SetActive(level == bossLevel);
				bossProgressIcons[i].KilledImage.enabled = level > bossLevel;
				bossProgressIcons[i].DeadBossImage.enabled = level > bossLevel;
				bossProgressIcons[i].BossImage.enabled = level == bossLevel;
				bossProgressIcons[i].CirclePrevBossImage.enabled = level > bossLevel;
				bossProgressIcons[i].CircleCurrBossImage.enabled = level == bossLevel;
				bossProgressIcons[i].CircleNextBossImage.enabled = level < bossLevel;
				bossLevel++;
			}
			
			BossProgressPanel.anchoredPosition = new Vector2(currLvlOffset * -lineWidth, 0);
			LocationProgressPanel.anchoredPosition = new Vector2(0f, 0);
		}

		private IEnumerator UpdateLevelProgressProcess(int level, int stage, float time)
		{
			var prevLvl = level - 1;
		
			this.bossProgressIcons.Clear();

			foreach (Transform tr in BossProgressPanel.transform)
			{
				Destroy(tr.gameObject);
			}
		
			var lineWidth = LinePrefab.rectTransform.rect.width;

			var lineOffset = lineWidth/2f;

			var prevLevelBosses = GameHandler.Singleton.Level.GetPrevLevelBosses();

			for (var i = 0; i < prevLevelBosses.Count; i++)
			{
				var bossIcon = Instantiate(
					BossIcons.Single(b => b.BossPrefab.name.Equals(prevLevelBosses[i])).BossProgressIconPrefab,
					BossProgressPanel);

				var bossIconRectTr = bossIcon.GetComponent<RectTransform>();

				bossIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(i * lineWidth, 0);

				var line = Instantiate(LinePrefab, BossProgressPanel);

				line.transform.SetAsFirstSibling();

				line.rectTransform.anchoredPosition = bossIconRectTr.anchoredPosition + new Vector2(lineOffset, 0);

				this.bossProgressIcons.Add(bossIcon);
			}

			var bossIcon2 = Instantiate(BossIcons.Single(b => b.BossPrefab.name.Equals(GameHandler.Singleton.Level.bossPrefab.name)).BossProgressIconPrefab,
				BossProgressPanel);

			var bossIconRectTr2 = bossIcon2.GetComponent<RectTransform>();

			bossIcon2.GetComponent<RectTransform>().anchoredPosition = new Vector3(prevLevelBosses.Count * lineWidth, 0);

			var line2 = Instantiate(LinePrefab, BossProgressPanel);
				
			line2.transform.SetAsFirstSibling();
				
			line2.rectTransform.anchoredPosition = bossIconRectTr2.anchoredPosition + new Vector2(lineOffset, 0);
					
			this.bossProgressIcons.Add(bossIcon2);

			for (var i = 0; i < 2; i++)
			{
				var bossIcon = Instantiate(BossIcons[i].BossProgressIconPrefab,
					BossProgressPanel);

				var bossIconRectTr = bossIcon.GetComponent<RectTransform>();

				bossIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3((prevLevelBosses.Count + 1) * lineWidth + i * lineWidth, 0);

				var line = Instantiate(LinePrefab, BossProgressPanel);
				
				line.transform.SetAsFirstSibling();
				
				line.rectTransform.anchoredPosition = bossIconRectTr.anchoredPosition + new Vector2(lineOffset, 0);
					
				this.bossProgressIcons.Add(bossIcon);
			}
		
			var width = ((Screen.width / (float) Screen.height) / (1920f / 1080f)) * 1920f;
			
			for (var i = 0; i < Locations.Count; i++)
			{
				Locations[i].offsetMin = new Vector2(i * width, 0f);
				Locations[i].offsetMax = new Vector2(i * width, 0f);
			};
			
			var levelParam = LevelParams[GameHandler.Singleton.Level.PrevStageLevelIdx];
			
			LevelBackgrounds[0].LevelImage.sprite = levelParam.LevelMenuBackground;
			LevelBackgrounds[0].TopPattern.color = levelParam.LevelMenuColor;
			LevelBackgrounds[0].BottomPattern.color = levelParam.LevelMenuColor;

			LevelBackgrounds[1].LevelImage.sprite = GameHandler.Singleton.Level.LevelMenuBackground;
			LevelBackgrounds[1].TopPattern.color = GameHandler.Singleton.Level.LevelMenuColor;
			LevelBackgrounds[1].BottomPattern.color = GameHandler.Singleton.Level.LevelMenuColor;
			
			var bossLevel = level - prevLevelBosses.Count;

			for (var i = 0; i < bossProgressIcons.Count; i++)
			{
				bossProgressIcons[i].StageProgressText.text = stage + "/7";
				bossProgressIcons[i].StageProgressPanel.SetActive(prevLvl == bossLevel);
				bossProgressIcons[i].KilledImage.enabled = prevLvl > bossLevel;
				bossProgressIcons[i].DeadBossImage.enabled = prevLvl > bossLevel;
				bossProgressIcons[i].BossImage.enabled = prevLvl == bossLevel;
				bossProgressIcons[i].CirclePrevBossImage.enabled = prevLvl > bossLevel;
				bossProgressIcons[i].CircleCurrBossImage.enabled = prevLvl == bossLevel;
				bossProgressIcons[i].CircleNextBossImage.enabled = prevLvl < bossLevel;

				bossLevel++;
			}
			
			BossProgressPanel.anchoredPosition = new Vector2((prevLevelBosses.Count - 1) * -lineWidth, 0);
			LocationProgressPanel.anchoredPosition = new Vector2(0, 0);
			
			yield return new WaitForSeconds(0.5f);

			var prevBossIcon = prevLevelBosses.Count - 1;
			
			bossProgressIcons[prevBossIcon].StageProgressPanel.SetActive(false);
			bossProgressIcons[prevBossIcon].KilledImage.enabled = true;
			bossProgressIcons[prevBossIcon].DeadBossImage.enabled = true;
			bossProgressIcons[prevBossIcon].BossImage.enabled = false;
			bossProgressIcons[prevBossIcon].CirclePrevBossImage.enabled = true;
			bossProgressIcons[prevBossIcon].CircleCurrBossImage.enabled = false;
			bossProgressIcons[prevBossIcon].CircleNextBossImage.enabled = false;
			
			yield return new WaitForSeconds(0.5f);
		
			var currTime = time;

			var bossPanelStartX = BossProgressPanel.anchoredPosition.x;
			var bossPanelTargetX = BossProgressPanel.anchoredPosition.x - lineWidth;
			
			var locationPanelStartX = LocationProgressPanel.anchoredPosition.x;
			var locationPanelTargetX = LocationProgressPanel.anchoredPosition.x - width;

			while (currTime > 0)
			{
				var lerp = 1 - currTime / time;
				
				BossProgressPanel.anchoredPosition = new Vector2(Mathf.Lerp(bossPanelStartX, bossPanelTargetX, lerp), 0f);
				LocationProgressPanel.anchoredPosition = new Vector2(Mathf.Lerp(locationPanelStartX, locationPanelTargetX, lerp), 0f);
			
				currTime -= Time.deltaTime;

				yield return null;
			}
			
			BossProgressPanel.anchoredPosition = new Vector2(bossPanelTargetX, 0f);
			LocationProgressPanel.anchoredPosition = new Vector2(locationPanelTargetX, 0f);
			
			var currBossIcon = prevLevelBosses.Count;
			
			bossProgressIcons[currBossIcon].StageProgressText.text = stage +"/7";
			bossProgressIcons[currBossIcon].StageProgressPanel.SetActive(true);
			bossProgressIcons[currBossIcon].KilledImage.enabled = false;
			bossProgressIcons[currBossIcon].DeadBossImage.enabled = false;
			bossProgressIcons[currBossIcon].BossImage.enabled = true;
			bossProgressIcons[currBossIcon].CirclePrevBossImage.enabled = false;
			bossProgressIcons[currBossIcon].CircleCurrBossImage.enabled = true;
			bossProgressIcons[currBossIcon].CircleNextBossImage.enabled = false;
		}
	}
}