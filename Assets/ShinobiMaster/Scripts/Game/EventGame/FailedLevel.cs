
using System.Diagnostics;
using Game;
using UnityEngine.Events;

public class FailedLevel : EventGame
{
    Stopwatch timer;

    public UnityEvent<FailedLevel> OnBlackout { get; set; }

    bool startBlackout;
    private bool isBlakoutEnded;


    public FailedLevel()
    {
        OnBlackout = new UnityEvent<FailedLevel>();
    }
    
    public override void Update()
    {
        if (!this.isBlakoutEnded && timer.ElapsedMilliseconds > 2000)
        {
            this.isBlakoutEnded = true;
            OnBlackout.Invoke(this);
        }
    }



    public override void End()
    {
        LevelInfoUI.LevelInfo.SetActiveStagesCountPanel(true);
        LevelInfoUI.LevelInfo.SetActiveLevelText(true);
        
        GameHandler.Singleton.Level.RestartLevel();
        
        GameHandler.Singleton.Player.Health = GameHandler.Singleton.Player.MaxHealth;
    }

    public override void StartEvent()
    {
        TimeControll.Singleton.ChannelTimeTo(TimeControll.TimeNormal);
        timer = new Stopwatch();
        timer.Start();
        
        AudioManager.Instance.SetMusicVolumeSmooth(0, 0.5f);
        AudioManager.Instance.PlayLoseSound();
    }
}
