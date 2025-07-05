using Game;
using Game.UI;
using SendAppMetrica;
using UnityEngine;

public class TapToPlay : EventGame
{
    public override void Update()
    {
    }

    public override void End()
    {
        Pause.OffPause();
        
        var locNum = int.Parse(GameHandler.Singleton.Level.GetStages()[GameHandler.Singleton.Level.CurrStageNumber].name.Replace("Location ", ""));

        AnalyticsManager.Instance.Event_LevelStart(GameHandler.Singleton.Level.GetNumLevel(), 
        GameHandler.Singleton.Level.CurrStageNumber+1, locNum, 
            GameHandler.Singleton.SkinService.CurrentCharacterSkin.Id, 
            GameHandler.Singleton.SkinService.CurrentWeaponSkin.Id, DifficultyRepository.Instance.CurrentDifficultyConfig + 1);
        
        TapToPlayPanel.Instance.OnClick -= TapToPlayPanelOnClick;
        SkinShopUI.Instance.SetActiveSkinShopPanel(false);
        TapToPlayTextUI.TapToPlayText.HideText();
        TapToPlayTextUI.TapToPlayText.AnimHidePanel();
        SettingsUI.Instance.HideButtonSmoothly(0.75f);
        LevelInfoUI.LevelInfo.AnimHideStageProgressPanel(1.0f);
        AudioManager.Instance.SetMusicVolumeSmooth(AudioManager.Instance.MusicInGameVolume, 0.7f);
    }

    public override void StartEvent()
    {
        Pause.SetPause();
        
        AudioManager.Instance.SetMusicVolumeSmooth(AudioManager.Instance.MusicInLobbyVolume, 0.7f);
        TapToPlayPanel.Instance.OnClick += TapToPlayPanelOnClick;
        SkinShopUI.Instance.SetActiveSkinShopPanel(true);
        TapToPlayTextUI.TapToPlayText.ShowText();
        TapToPlayTextUI.TapToPlayText.AnimShowPanel();
        LevelInfoUI.LevelInfo.AnimShowStageProgressPanel(0f);
        SettingsUI.Instance.ShowButtonSmoothly(0.1f, 0.3f);
    }

    private void TapToPlayPanelOnClick()
    {
        CallEndEvent();
    }
}
