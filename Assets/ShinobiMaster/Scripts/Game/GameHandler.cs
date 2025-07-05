using System.Collections;
using System.Collections.Generic;
using Game;
using Game.EventGame;
using Game.LevelControll;
using Game.PlayerScripts;
using Game.UI;
using SendAppMetrica;
using Skins;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Singleton { get; private set; }

    public GameEventHandler EventHandler { get; private set; }

    public BaseLevel Level => levelControll;

    [SerializeField] private BaseLevel levelControll;
    
    public ISkinService SkinService { get; private set; }
    [field:SerializeField]
    public Player Player { get; set; }
    public PlayerProfile PlayerProfile { get; private set; }
    
    
    private void Awake()
    {
        if (Singleton)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;

        PlayerProfile = new PlayerProfile();
        
        EventHandler = new GameEventHandler();
        levelControll.StageComplete += StageComplete;
        levelControll.LevelComplete += LevelComplete;
        
        levelControll.StageComplete += OnStageCompleted;
        levelControll.LevelComplete += OnLevelCompleted;
    }

    private void Start()
    {
        SkinService = new PlayerSkinService();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SkipStage();
        }
    
        EventHandler.Update();
    }
    
    private void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
        
        levelControll.StageComplete -= OnStageCompleted;  
        levelControll.LevelComplete -= OnLevelCompleted;
    }

    public void PlayerDead()
    {
        var failedLevel = new FailedLevel();
        
        failedLevel.OnBlackout?.AddListener(OnFailedLevelBlackout);
    
        EventHandler.AddEvent(failedLevel);

        this.failedLevel = failedLevel;
    }

    public void LevelPassedWithDelay(float delay)
    {
        StartCoroutine(LevelPassedWithDelayProcess(delay));
    }

    private IEnumerator LevelPassedWithDelayProcess(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        while (Pause.IsPause)
        {
            yield return null;
        }
        
        SceneLoad.LevelPassed();
    }

    private FailedLevel failedLevel;

    private void OnFailedLevelBlackout(FailedLevel failedLevel)
    {
        LevelInfoUI.LevelInfo.SetActiveStagesCountPanel(false);
        LevelInfoUI.LevelInfo.SetActiveLevelText(false);
    
        SkipLevelUI.Instance.ShowPanel();
    
        SkipLevelUI.Instance.ClosePanelButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickButtonSound();
            
            var stageName = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber].name;

            var l = string.Empty;

            if (stageName.Contains("Location "))
            {
                l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber].name
                    .Replace("Location ", "");
            }
            else
            {
                l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber].name
                    .Replace("Location_p_", "");
            }
        
            var locNum = int.Parse(l);

            AnalyticsManager.Instance.Event_LevelFinish("lose_lose", 
                TimerStage.GetRecrodS(), levelControll.GetNumLevel(), 
                levelControll.CurrStageNumber+1, locNum, DifficultyRepository.Instance.CurrentDifficultyConfig + 1, 
                EnemyControll.Singleton.EnemiesCount, Player.Health, Player.LoseCountHP, Player.PickHP);
        
            DifficultyRepository.Instance.CurrentDifficultyConfig--;

            Singleton.Level.CurrStageNumber = 0;

            StaticGameObserver.LoadStartProgress(out var lvl, out _);
            StaticGameObserver.SaveProgress(lvl,  0);
            
            failedLevel.End();
        });
        
        SkipLevelUI.Instance.SkipLevelButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayClickButtonSound();
            
            AdLoadingPanel.Instance.Placement = "RewardAds_SkipLevel";
            
            AdLoadingPanel.Instance.SetActive(true, () =>
            {
                EventHandler.RemoveEvent(failedLevel);
                SkipLevelUI.Instance.HidePanel();
                AdLoadingPanel.Instance.HidePanel();
                
                SkipStage();
            }, SkipStageDismissAction);
        });
    }

    private void SkipStageDismissAction()
    {
        var stageName = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber].name;

        var l = string.Empty;

        if (stageName.Contains("Location "))
        {
            l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber].name
                .Replace("Location ", "");
        }
        else
        {
            l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber].name
                .Replace("Location_p_", "");
        }
        
        var locNum = int.Parse(l);

        AnalyticsManager.Instance.Event_LevelFinish("lose_lose", 
            TimerStage.GetRecrodS(), levelControll.GetNumLevel(), levelControll.CurrStageNumber+1, locNum, 
            DifficultyRepository.Instance.CurrentDifficultyConfig + 1, EnemyControll.Singleton.EnemiesCount, Player.Health, Player.LoseCountHP, Player.PickHP);
            
        DifficultyRepository.Instance.CurrentDifficultyConfig--;
    
        EventHandler.RemoveEvent(this.failedLevel);
        LevelInfoUI.LevelInfo.SetActiveStagesCountPanel(true);
        LevelInfoUI.LevelInfo.SetActiveLevelText(true);
        SkipLevelUI.Instance.HidePanel();
        AdLoadingPanel.Instance.HidePanel();
        this.failedLevel.End();
    }

    private void SkipStage()
    {
        levelControll.StageFinish(true);

        Player.Health = 1;
        
        var stageName = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber-1].name;

        var l = string.Empty;

        if (stageName.Contains("Location "))
        {
            l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber-1].name
                .Replace("Location ", "");
        }
        else
        {
            l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber-1].name
                .Replace("Location_p_", "");
        }
        
        var locNum = int.Parse(l);

        AnalyticsManager.Instance.Event_LevelFinish("lose_skip", 
            TimerStage.GetRecrodS(), levelControll.GetNumLevel(), levelControll.CurrStageNumber, locNum,
            DifficultyRepository.Instance.CurrentDifficultyConfig + 1, EnemyControll.Singleton.EnemiesCount, Player.Health, Player.LoseCountHP, Player.PickHP);
    }
    
    public void ContinueStage()
    {
        AdLoadingPanel.Instance.Placement = "RewardAds_ContinueStage";
        
        AudioManager.Instance.PlayClickButtonSound();
        
        AdLoadingPanel.Instance.SetActive(true, () =>
        {
            EventHandler.RemoveEvent(failedLevel);
            
            AudioManager.Instance.SetMusicVolumeSmooth(AudioManager.Instance.MusicInGameVolume, 0.7f);
            
            var stageName = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber].name;

            var l = string.Empty;

            if (stageName.Contains("Location "))
            {
                l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber].name
                    .Replace("Location ", "");
            }
            else
            {
                l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber].name
                    .Replace("Location_p_", "");
            }
        
            var locNum = int.Parse(l);

            AnalyticsManager.Instance.Event_LevelFinish("lose_continue", 
                TimerStage.GetRecrodS(), levelControll.GetNumLevel(), levelControll.CurrStageNumber+1, locNum,
                DifficultyRepository.Instance.CurrentDifficultyConfig + 1, EnemyControll.Singleton.EnemiesCount, Player.Health, Player.LoseCountHP, Player.PickHP);
            
            LevelInfoUI.LevelInfo.SetActiveStagesCountPanel(true);
            LevelInfoUI.LevelInfo.SetActiveLevelText(true);
            SkipLevelUI.Instance.HidePanel();
            AdLoadingPanel.Instance.HidePanel();
    
            var player = Singleton.Player;

            player.transform.position = player.PlayerDeathPosition;

            player.Health = 1;
            
            player.Respawn();
            
            player.Invulnerability(player.InvulnerabilityDuration);
            player.BlinkEffect(0.3f, player.InvulnerabilityDuration);
        }, SkipStageDismissAction);
    }

    public void LevelComplete(bool skip)
    {
        var completeLevelEvent = new CompleteLevel();
    
        EventHandler.AddEvent(completeLevelEvent);
        completeLevelEvent.EndEvent += EventHandler.RemoveEvent;
        
        if (!skip)
        {
            var stageName = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber-1].name;

            string l;

            if (stageName.Contains("Location "))
            {
                l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber-1].name
                    .Replace("Location ", "");
            }
            else
            {
                l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber-1].name
                    .Replace("Location_p_", "");
            }
        
            var locNum = int.Parse(l);

            AnalyticsManager.Instance.Event_LevelFinish("win",
                TimerStage.GetRecrodS(), levelControll.GetNumLevel(), levelControll.CurrStageNumber, locNum,
                DifficultyRepository.Instance.CurrentDifficultyConfig + 1, EnemyControll.Singleton.EnemiesCount, Player.Health, Player.LoseCountHP, Player.PickHP);
                
            DifficultyRepository.Instance.CurrentDifficultyConfig++;
        }
    }

    public void StageComplete(bool skip)
    {
        var completeStageEvent = new CompleteStage();
        
        EventHandler.AddEvent(completeStageEvent);
        completeStageEvent.EndEvent += EventHandler.RemoveEvent;

        NewSkinUI.Instance.ProgressAnimationShown = false;
        NewSkinUI.Instance.NeedProgressAnimation = true;

        if (!skip)
        {
            var stageName = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber-1].name;

            string l;

            if (stageName.Contains("Location "))
            {
                l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber-1].name
                    .Replace("Location ", "");
            }
            else
            {
                l = Singleton.Level.GetStages()[Singleton.Level.CurrStageNumber-1].name
                    .Replace("Location_p_", "");
            }
        
            var locNum = int.Parse(l);

            AnalyticsManager.Instance.Event_LevelFinish("win",
                TimerStage.GetRecrodS(), levelControll.GetNumLevel(), levelControll.CurrStageNumber, locNum,
                DifficultyRepository.Instance.CurrentDifficultyConfig+1, EnemyControll.Singleton.EnemiesCount, Player.Health, Player.LoseCountHP, Player.PickHP);
                
            DifficultyRepository.Instance.CurrentDifficultyConfig++;
        }
    }
    
    private void OnStageCompleted(bool skip)
    {
        if (!skip)
        {
            AdLoadingPanel.Instance.ShowInterstitialAd("StageComplete");
        }
    }
    
    private void OnLevelCompleted(bool skip)
    {
        if (!skip)
        {
            AdLoadingPanel.Instance.ShowInterstitialAd("StageComplete");
        }
    }

    public class GameEventHandler
    {
        List<EventGame> eventGames = new List<EventGame>();
        public void AddEvent(EventGame eventGame)
        {
            eventGames.Add(eventGame);
            eventGame.StartEvent();
        }

        public void Update()
        {
            for (int i = 0; i < eventGames.Count; i++)
            {
                eventGames[i].Update();
            }
        }

        public void RemoveEvent(EventGame eventGame)
        {
            eventGames.Remove(eventGame);
        }
    }
}
