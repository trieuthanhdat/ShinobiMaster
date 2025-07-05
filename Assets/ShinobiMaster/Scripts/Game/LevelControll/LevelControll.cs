using Game.LevelControll;

public class LevelControll : BaseLevel
{

    public override void StartGame()
    {
        StaticGameObserver.LoadStartProgress(out var l, out var s);
        
        var level = SceneLoad.GetCurrentLevel();
        
        var countLevel = SceneLoad.GetListLevels().Length;

        if (l > countLevel)
        {
            SceneLoad.LoadRepeatLevel();
        }
        else if (l != level.NumerLevel)
        {
            var levels = SceneLoad.GetListLevels();

            foreach (var lvl in levels)
            {
                if (lvl.NumerLevel == l)
                {
                    SceneLoad.LoadLevel(lvl);
                    return;
                }
            }
        }

        LevelInfoUI.LevelInfo.SetNameLevel(SceneLoad.GetCurrentLevel().NumerLevel);

        CurrStageNumber = s;

        LoadStage(s);
        
        LevelInfoUI.LevelInfo.SetParamsLevelIcon(GetStageIconParams());
    }
    

    protected override void CallLoadStage()
    {
        base.CallLoadStage();
    
        CurrentStage.StageComplete += StageFinish;
        GameHandler.Singleton.Player.Respawn();
    }

    public override int GetNumLevel()
    {
        return SceneLoad.GetCurrentLevel().NumerLevel;
    }
}
