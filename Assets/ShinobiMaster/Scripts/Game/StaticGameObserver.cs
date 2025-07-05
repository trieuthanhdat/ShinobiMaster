using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticGameObserver
{

    static LevelControll levelControll;

    public static void LoadLevelController(LevelControll Controll)
    {
        levelControll = Controll;
    }


    public static void SaveProgress(int level, int stage)
    {
        GameFile opt = new GameFile();
        opt.CurrentLevel = level;
        opt.CurrentStage = stage;
        FileSystem.SaveFile(opt);
    }

    public static void LoadStartProgress(out int level, out int stage)
    {
        GameFile opt = FileSystem.LoadFile();
        level = opt.CurrentLevel;
        stage = opt.CurrentStage;
    }

    public static void UnLoadLevelController()
    {
        levelControll = null;
    }
}
