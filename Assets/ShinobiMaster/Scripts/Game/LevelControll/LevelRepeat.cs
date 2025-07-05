using System;
using System.Collections.Generic;
using System.Xml.Schema;
using Game;
using Game.Enemy;
using Game.LevelControll;
using Game.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelRepeat : BaseLevel
{
    private int level;

    public const string StageLevelIdxKey = "StageLevelIdx";
    public const string BossIdxKey = "BossIdx";
    
    public int CurrLvl
    {
        get => PlayerPrefs.GetInt("CurrLvl");
        set
        {
            PlayerPrefs.SetInt("CurrLvl", value);
            PlayerPrefs.Save();
        }
    }

    public int StageLevelIdx
    {
        get => PlayerPrefs.GetInt(StageLevelIdxKey, 0);
        set
        {
            PlayerPrefs.SetInt(StageLevelIdxKey, value);
            PlayerPrefs.Save();
        }
    }
    
    public int BossIdx
    {
        get => PlayerPrefs.GetInt(BossIdxKey, 0);
        set
        {
            PlayerPrefs.SetInt(BossIdxKey, value);
            PlayerPrefs.Save();
        }
    }

    public override Stage[] GetStages()
    {
        return this.levelStages[StageLevelIdx].Stages;
    }
    
    [SerializeField] protected List<LevelParams> levelStages;
    [SerializeField] private Enemy[] bosses;
    private System.Random random;



    public override void Awake()
    {
        base.Awake();
        
        this.random = new System.Random();
        
        if (!PlayerPrefs.HasKey(StageLevelIdxKey))
        {
            StageLevelIdx = this.random.GetRandomWithExclusion(0, this.levelStages.Count - 1, 
            new []{this.levelStages.Count - 1});
        }

        if (!PlayerPrefs.HasKey(BossIdxKey))
        {
            BossIdx = this.random.GetRandomWithExclusion(0, this.bosses.Length - 1,
            new []{this.bosses.Length - 1});
        }
        
        LoadLevelParams();
    }

    

    public override void StageFinish(bool skip)
    {
        if (CurrStageNumber < GetStages().Length - 1)
        {
            CurrStageNumber++;
            StaticGameObserver.SaveProgress(GetNumLevel(), CurrStageNumber);
            StageCompleteEvent(skip);
        }
        else
        {
            CurrStageNumber++;

            UpdatePrevLevelBosses();
            
            PrevStageLevelIdx = StageLevelIdx;
            
            StageLevelIdx = this.random.GetRandomWithExclusion(0, this.levelStages.Count - 1, new []{StageLevelIdx});
            BossIdx = this.random.GetRandomWithExclusion(0, this.bosses.Length - 1, new []{BossIdx});
            
            LoadLevelParams();
            
            StaticGameObserver.SaveProgress(GetNumLevel() + 1, 0);

            LevelCompleteEvent(skip);
        }     
    }
    
    

    private StageColorScheme GetStageColorScheme()
    {
        return this.levelStages[StageLevelIdx].StageColorScheme;
    }

    private void LoadLevelParams()
    {
        StageColorScheme = GetStageColorScheme();
        LevelMenuBackground = this.levelStages[StageLevelIdx].LevelMenuBackground;
        LevelMenuColor = this.levelStages[StageLevelIdx].LevelMenuColor;

        this.bossPrefab = this.bosses[BossIdx];
    }

    public override void StartGame()
    {
        StaticGameObserver.LoadStartProgress(out var l, out var s);
        
        level = l;
        CurrStageNumber = s;
        
        LoadStage(CurrStageNumber);
        
        LevelInfoUI.LevelInfo.SetNameLevel(GetNumLevel());
        LevelInfoUI.LevelInfo.SetParamsLevelIcon(GetStageIconParams());
    }

    public override void RestartStage()
    {
        UnloadStage(CurrentStage);
            
        this.player.Respawn();
            
        LoadStage(CurrStageNumber);
    }

    protected override void CallLoadStage()
    {
        base.CallLoadStage();
    
        RenderSettings.fogColor = this.levelStages[StageLevelIdx].FogColor;
        CurrentStage.StageComplete += StageFinish;
        GameHandler.Singleton.Player.Respawn();
    }

    public override int GetNumLevel()
    {
        return level;
    }

    protected override void CallUnloadStage()
    {

    }
}
