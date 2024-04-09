using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class CEditorSaveLoad
{
    public const string EXTENSION = "psf";
    public const string SETTINGS_PATH = "CustomSettings/";

    public static string GetFileName(string aName)
    {
        Directory.CreateDirectory(SETTINGS_PATH);
        return SETTINGS_PATH + aName + "." + EXTENSION;
    }

    public static void Save<T>(T aData, string aSaveFile)
    {
        BinaryFormatter tBf = new BinaryFormatter();
        string tFilePath = GetFileName(aSaveFile);
        FileStream tFile = File.Create(tFilePath);
        tBf.Serialize(tFile, aData);
        tFile.Close();
        Debug.Log("File '" + tFilePath + "' saved");
    }

    public static void Load<T>(ref T aData, string aSaveFile)
    {
        string tFileName = GetFileName(aSaveFile);
        if (File.Exists(tFileName))
        {
            BinaryFormatter tBf = new BinaryFormatter();
            FileStream tFile = File.Open(tFileName, FileMode.Open);
            aData = (T)tBf.Deserialize(tFile);
            tFile.Close();
            Debug.Log("File '" + tFileName + "' loaded");
        }
        else
            Debug.Log("The file '" + tFileName + "' does not exist");
    }
}
