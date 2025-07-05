using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class FileSystem
{
    public static readonly string NameFile = "ghostWalker";
    public static readonly string TypeFile = "gw";

#if UNITY_EDITOR
    private static string path { get { return Application.dataPath; } }
#else
    private static string path { get { return Application.persistentDataPath; } }
#endif

    public static GameFile LoadFile()
    {
        GameFile opt = GameFile.Default();
        if (File.Exists(path + "/" + NameFile + "." + TypeFile))
        {
            using (FileStream file = File.Open(path + "/" + NameFile + "." + TypeFile, FileMode.OpenOrCreate))
            {
                BinaryFormatter bf = new BinaryFormatter();
                opt = (GameFile)bf.Deserialize(file);
            }
        }
        return opt;
    }

    public static void SaveFile(GameFile opt)
    {
        using (FileStream file = File.Open(path + "/" + NameFile + "." + TypeFile, FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(file, opt);
        }
    }
}
