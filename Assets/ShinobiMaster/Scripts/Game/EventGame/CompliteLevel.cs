using System.Diagnostics;
using Game.UI;
using Skins;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.EventGame
{
    public class CompleteLevel : global::EventGame
    {
        Stopwatch timer;

        bool startBlackout;
        public override void Update()
        {
            if (timer.ElapsedMilliseconds > 2000)
            {
                CallEndEvent();
            }
            else if (!startBlackout && timer.ElapsedMilliseconds > 1500)
            {
                //CameraControll.Singleton.Blackout.Blackout();
                startBlackout = true;
            }
        }

        public override void End()
        {
            LevelInfoUI.LevelInfo.HideFinalLevel();
            LevelInfoUI.LevelInfo.SetActiveStagesCountPanel(true);
            LevelInfoUI.LevelInfo.SetActiveLevelText(true);
            LevelInfoUI.LevelInfo.SetActiveKeysPanel(false);
            if (!NewSkinUI.Instance.IsNewSkin() || GameHandler.Singleton.Level.CurrStageNumber > SkinRepository.Instance.SkinStage2)
            {
                GameHandler.Singleton.LevelPassedWithDelay(0f);
            }

        }

        public override void StartEvent()
        {
            LevelInfoUI.LevelInfo.FinalLevel();
            timer = new Stopwatch();
            timer.Start();
            
            NewSkinUI.Instance.SkinForOpenShown = false;
            
            AudioManager.Instance.SetMusicVolumeSmooth(0, 0.7f);
            AudioManager.Instance.PlayWinSound();
            TimeControll.Singleton.PauseTimeControl();
            Time.timeScale = 1f;
            
            if (GameHandler.Singleton.PlayerProfile.KeysCount == 3)
            {
                ChestOpenningUI.Instance.ChestNumber = 0;
                ChestOpenningUI.Instance.OpenNumber++;
            
                ChestOpenningUI.Instance.SetActivePanelWithDelay(true, 2.0f);
            }
        
            LevelInfoUI.LevelInfo.SetParamsLevelIcon(GameHandler.Singleton.Level.GetStageIconParams());
            
            BossUI.Instance.SetActive(false);
        }
    }
}
