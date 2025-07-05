using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangedLevel : MonoBehaviour
{
    [SerializeField] Button goLevel;
    [SerializeField] Button goStage;

    [SerializeField] InputField levelInput;
    [SerializeField] InputField stageInput;
    
    

    private void Start()
    {
        goLevel.onClick.AddListener(ChangeLevel);
        goStage.onClick.AddListener(ChangeStage);
    }

    private void ChangeLevel()
    {
        Level[] levels = SceneLoad.GetListLevels();
        int num = System.Convert.ToInt32(levelInput.text);
        FileSystem.SaveFile(new GameFile() { CurrentLevel = num, CurrentStage = 0 });
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].NumerLevel == num)
            {
                SceneLoad.LoadLevel(levels[i]);
                Pause.OffPause();
                
                return;
            }
        }
    }
    
    private void ChangeStage()
    {
        if (int.TryParse(stageInput.text, out var stage))
        {
            GameHandler.Singleton.Level.UnloadStage(GameHandler.Singleton.Level.CurrentStage);
            GameHandler.Singleton.Level.LoadStage(stage);
            GameHandler.Singleton.Level.CurrStageNumber = stage;
        }
    }

    public void SkipLevel()
    {
        GameHandler.Singleton.Level.CurrStageNumber = 8;
        GameHandler.Singleton.Level.StageFinish(true);
    }
}
