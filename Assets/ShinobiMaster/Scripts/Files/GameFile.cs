using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameFile
{
    public int CurrentLevel;
    public int CurrentStage;

    public static GameFile Default()
    {
        GameFile gameFile = new GameFile();
        gameFile.CurrentLevel = 1;
        gameFile.CurrentStage = 0;
        return gameFile;
    }


}
