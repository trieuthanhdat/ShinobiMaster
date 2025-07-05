using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuControll : MonoBehaviour//Требует рефакторинг
{
    [SerializeField] private ElementList elementList;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
#if UNITY_EDITOR
        /*Level[] levels = SceneLoad.GetListLevels();

        for (int i = 0; i < levels.Length; i++)
        {
            Button button = Instantiate(elementList.PrefabLoadButton, elementList.ListPanel);
            button.GetComponentInChildren<Text>().text = levels[i].NumerLevel.ToString();
            button.gameObject.SetActive(true);
            int num = i;
            object index = num;
            button.onClick.AddListener(delegate { SceneLoad.LoadLevel(levels[(int)index]); });
        }*/
        
        StaticGameObserver.LoadStartProgress(out var level, out var stage);
        
        var countLevel = SceneLoad.GetListLevels().Length;

        if (level > countLevel)
        {
            SceneLoad.LoadRepeatLevel();
        }
        else
        {
            SceneLoad.LoadLevel(SceneLoad.GetListLevels()[level - 1]);
        }
#else
        StaticGameObserver.LoadStartProgress(out var level, out var stage);
        
        var countLevel = SceneLoad.GetListLevels().Length;

        if (level > countLevel)
        {
            SceneLoad.LoadRepeatLevel();
        }
        else
        {
            SceneLoad.LoadLevel(SceneLoad.GetListLevels()[level - 1]);
        }
#endif
    }
}
