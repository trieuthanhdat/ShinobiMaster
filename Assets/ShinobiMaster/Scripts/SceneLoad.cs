using System.Collections;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoad
{

    private static Level[] levelsCash = null;

    public static void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
    public static void Level1Load()
    {
        SceneManager.LoadScene(0);
    }

    public static void LevelPassed()
    {
        Level[] levels = GetListLevels();
        Level currentLevel = GetCurrentLevel();
        if (currentLevel != null && currentLevel.NumerLevel != levels.Length)
        {
            for (int i = 0; i < levels.Length; i++)
            {
                if (levels[i].NumerLevel > currentLevel.NumerLevel)
                {
                    LoadLevel(levels[i]);
                    return;
                }
            }
            LoadLevel(levels.First(x => x.NumerLevel == 1));
        }
        else
        {
            LoadRepeatLevel();
        }
    }

    public static void LoadLevel(Level level)
    {
        if (SceneManager.GetActiveScene().name.Equals(level.FullName))
        {
            return;
        }
    
        SceneManager.LoadScene(level.FullName);
    }

    public static void LoadRepeatLevel()
    {
        int countScene = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < countScene; i++)
        {
            string name = GetNameSceneFromPath(SceneUtility.GetScenePathByBuildIndex(i));
            if (name.IndexOf("REP") != -1)
            {
                SceneManager.LoadScene(i);
            }
        }
    }

    public static Level GetCurrentLevel()
    {
        Level[] levels = GetListLevels();
        string name = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].FullName == name)
                return levels[i];
        }
        return null;
    }

    public static Level[] GetListLevels()
    {
        if (levelsCash != null)
        {
            return levelsCash;
        }
        int countScene = SceneManager.sceneCountInBuildSettings;

        Scene[] scenes = new Scene[countScene];
        for (int i = 0; i < countScene; i++)
        {
            scenes[i] = SceneManager.GetSceneByBuildIndex(i);
        }


        List<Level> levels = new List<Level>();
        for (int i = 0; i < scenes.Length; i++)
        {
            string name = GetNameSceneFromPath(SceneUtility.GetScenePathByBuildIndex(i));
            bool startCheck = false;
            bool startComments = false;
            bool endComments = false;

            int startIndexComent = 0;
            int endIndexComent = 0;

            int numLevel = 0;

            bool final = false;
            for (int i2 = 0; i2 < name.Length; i2++)
            {
                if (endComments && char.IsDigit(name[i2]))
                {
                    string num = name.Substring(i2);
                    numLevel = Convert.ToInt32(num);
                    final = true;
                    break;
                }

                if (startCheck && !startComments && name[i2] == '$')
                {
                    startIndexComent = i2;
                    startComments = true;
                }
                else if (startCheck && startComments && name[i2] == '$')
                {
                    endIndexComent = i2;
                    endComments = true;
                }

                if (!startCheck && name[i2 + 0] == 'L' && name[i2 + 1] == 'V' && name[i2 + 2] == 'L')
                {
                    startCheck = true;
                }
                else if (i2 == 0)
                {
                    break;
                }
            }

            if (final)
            {
                Level level = new Level();
                level.EndIndexComent = endIndexComent;
                level.FullName = name;
                level.NumerLevel = numLevel;
                level.StartIndexComent = startIndexComent;
                levels.Add(level);
            }
        }
        SaveCash(levels.ToArray());
        return levelsCash;
    }

    private static void SaveCash(Level[] levels)
    {
        levelsCash = levels;
    }

    private static string GetNameSceneFromPath(string path)
    {
        path = path.Remove(path.Length - 6);
        for (int i = 0; i < path.Length; i++)
        {
            if (path[i] == '\\' || path[i] == '/')
            {
                path = path.Remove(0, i + 1);
                i = 0;
            }
        }
        return path;
    }
}

public class Level
{
    public int StartIndexComent;
    public int EndIndexComent;
    public string FullName;
    public int NumerLevel;
}
