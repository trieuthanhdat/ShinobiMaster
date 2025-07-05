using System.Diagnostics;
using Game;
using Game.UI;
using SendAppMetrica;
using Skins;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CompleteStage : EventGame
{
    Stopwatch timer;

    bool startBlackout;
    public override void Update()
    {
        if (timer.ElapsedMilliseconds > 2000)
        {
            CallEndEvent();
        }
        else if (!startBlackout && timer.ElapsedMilliseconds > 1500 && !Pause.IsPause)
        {
            //CameraControll.Singleton.Blackout.Blackout();
            startBlackout = true;
        }
    }

    public override void End()
    {
       LevelInfoUI.LevelInfo.HideFinalStage();
       LevelInfoUI.LevelInfo.SetActiveStagesCountPanel(true);
       LevelInfoUI.LevelInfo.SetActiveLevelText(true);
       LevelInfoUI.LevelInfo.SetActiveKeysPanel(false);
       if (!NewSkinUI.Instance.IsNewSkin() || GameHandler.Singleton.Level.CurrStageNumber > SkinRepository.Instance.SkinStage2)
       {
           GameHandler.Singleton.Level.LoadNextStage(0f);
       }
    }

    public override void StartEvent()
    {
        timer = new Stopwatch();
        timer.Start();

        if (GameHandler.Singleton.Level.CurrStageNumber != 1)
        {
            LevelInfoUI.LevelInfo.FinalStage();

            NewSkinUI.Instance.SkinForOpenShown = false;

            AudioManager.Instance.SetMusicVolumeSmooth(0, 0.7f);
            AudioManager.Instance.PlayWinSound();
        }

        TimeControll.Singleton.PauseTimeControl();
        Time.timeScale = 1f;
        
        if (GameHandler.Singleton.PlayerProfile.KeysCount == 3)
        {
            ChestOpenningUI.Instance.ChestNumber = 0;
            ChestOpenningUI.Instance.OpenNumber++;
        
            ChestOpenningUI.Instance.SetActivePanelWithDelay(true, 2.0f);
        }
        else
        {
            if (GameHandler.Singleton.Level.CurrStageNumber != 1)
            {
                NewSkinUI.Instance.ShowWithDelayIfSkin(2f);
            }
            else
            {
                Pause.OffPause();
                TimeControll.Singleton.UnpauseTimeControl();
                GameHandler.Singleton.Level.LoadNextStage(1.5f);
            }
        }
        
        LevelInfoUI.LevelInfo.SetParamsLevelIcon(GameHandler.Singleton.Level.GetStageIconParams());
        
        BossUI.Instance.SetActive(false);
    }
}

