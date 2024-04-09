using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using RhoTools;
using SimpleJSON;
using System.IO;

public class CCustomDirectives : EditorWindow
{
    /*
    {
        "directives": [
            "dir1",
            "dir2"
        ],
        "platforms": {
            "WebPlayer": [
                "dir1"
            ],
            "StandAlone": [
                "dir2"
            ],
        }

    }
     * */
    #region Window definition
    [MenuItem(CConstants.ROOT_MENU + "Custom directives...")]
    public static void ShowWindow()
    {
        CCustomDirectives tWindow = GetWindow<CCustomDirectives>();
        GUIContent titleContent = new GUIContent("CDirectives");
        tWindow.titleContent = titleContent;
        tWindow.SetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        tWindow.LoadData();
    }
    #endregion

    const string NAME = "directives";
    const string DIRECTIVES = "directives";
    const string PLATFORMS = "platforms";

    //Directives _directives;
    BuildTargetGroup _group = BuildTargetGroup.Standalone;
    string _name = "";
    JSONNode _data;

    void OnEnable()
    {
        SetGroup(_group);
    }

    void OnGUI()
    {
        GUILayout.Label("Directives (" + _group.ToString() + "):", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        BuildTargetGroup tGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup("Group:", _group);
        if (EditorGUI.EndChangeCheck())
        {
            SetGroup(tGroup);
        }

        if (GUILayout.Button("Player Settings..."))
            EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");

        GUILayout.BeginHorizontal();
        _name = GUILayout.TextField(_name);
        if (GUILayout.Button("Create directive") && _name != "")
        {
            AddDirective(_name);
            AddDirectiveToGroup(_group, _name);
            SaveVars();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Synchronize"))
        {
            SaveVars();
            Synchronize(_group);
        }
        GUILayout.Label(PlayerSettings.GetScriptingDefineSymbolsForGroup(_group));
        
        if (_data == null)
            LoadData();

        bool[] tValues = new bool[_data[DIRECTIVES].Count];
        JSONNode tDirs = _data[DIRECTIVES];
        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < tValues.Length; i++)
        {
            GUILayout.BeginHorizontal();
            tValues[i] = EditorGUILayout.Toggle(HasDirective(_group, tDirs[i]), GUILayout.MaxWidth(25));
            GUILayout.Label(tDirs[i]);
            if (GUILayout.Button("Copy", GUILayout.MaxWidth(50)))
            {
                EditorGUIUtility.systemCopyBuffer = tDirs[i];
            }
            if (GUILayout.Button("X", GUILayout.MaxWidth(25)))
            {
                RemoveDirective(tDirs[i]);
                ArrayUtility.RemoveAt(ref tValues, i);
                SaveVars();
                break;
            }
            GUILayout.EndHorizontal();
        }
        if (EditorGUI.EndChangeCheck())
        {
            for (int i = 0; i <  tValues.Length; i++)
            {
                if (tValues[i])
                    AddDirectiveToGroup(_group, tDirs[i]);
                else
                    RemoveDirectiveFromGroup(_group, tDirs[i]);
                SaveVars();
            }
        }
    }

    void SetGroup(BuildTargetGroup aGroup)
    {
        if (_data != null)
            SaveVars();
        else
            LoadData();
        _group = aGroup;
        CheckGrup(_group, PlayerSettings.GetScriptingDefineSymbolsForGroup(_group));
    }

    void SaveVars()
    {
        if (_data != null)
            CTxtManager.Write(GetFileName(), CPrettyJSON.FormatJson(_data.ToString()));
        //CEditorSaveLoad.Save(_directives, NAME + _group.ToString());
    }

    string GetFileName()
    {
        Directory.CreateDirectory(CEditorSaveLoad.SETTINGS_PATH);
        return CEditorSaveLoad.SETTINGS_PATH + NAME + ".json";
    }

    void LoadData()
    {
        string tPath = GetFileName();
        if (File.Exists(tPath))
            _data = CTxtManager.LoadJson(tPath);
        else
            _data = null;

        if (_data == null)
        {
            _data = JSON.Parse("{}");
        }

        JSONNode tNode;
        if (_data[DIRECTIVES] == null)
        {
            tNode = JSON.Parse("[]");
            _data.Add(DIRECTIVES, tNode);
        }
        if (_data[PLATFORMS] == null)
        {
            tNode = JSON.Parse("{}");
            _data.Add(PLATFORMS, tNode);
        }

        // Check for previous version
        BuildTargetGroup[] tTargets = CEnumUtils.GetValues<BuildTargetGroup>();
        for (int i = 0; i < tTargets.Length; i++)
        {
            BuildTargetGroup tTarget = tTargets[i];
            tPath = NAME + "_" + tTarget.ToString();
            string tFullPath = CEditorSaveLoad.SETTINGS_PATH + tPath + "." + CEditorSaveLoad.EXTENSION;
            // Check if fllie exists
            if (File.Exists(tFullPath))
            {
                CSavedDirectives tData = new CSavedDirectives();
                CEditorSaveLoad.Load(ref tData, tPath);
                for (int j = 0; j < tData.directives.Length; j++)
                {
                    string tDir = tData.directives[j];
                    if (!IsDirectiveSaved(tDir))
                        AddDirective(tDir);
                    if (tData.values.Length > j && tData.values[j])
                    {
                        if (!HasDirective(tTarget, tDir))
                            AddDirectiveToGroup(tTarget, tDir);
                    }
                }

                File.Delete(tFullPath);
            }
        }

        SaveVars();
    }

    void CheckGrup(BuildTargetGroup aGroup, string aString)
    {
        if (aString == "")
            return;
        string[] tVals = aString.Split(';');
        for (int i = 0; i < tVals.Length; i++)
        {
            string tValue = tVals[i];
            if (!HasDirective(aGroup, tValue))
            {
                if (!IsDirectiveSaved(tValue))
                    AddDirective(tValue);

                    AddDirectiveToGroup(aGroup, tValue);
            }
        }
    }

    bool HasDirective(BuildTargetGroup aGroup, string aDirective)
    {
        JSONNode tNode = _data[PLATFORMS][aGroup.ToString()];
        if (tNode != null)
        {
            foreach (JSONNode tChild in tNode.AsArray)
            {
                if (aDirective.Equals(tChild))
                    return true;
            }
        }
        return false;
    }

    void AddDirective(string aDirective)
    {
        _data[DIRECTIVES].Add(aDirective);
    }

    void RemoveDirective(string aDirective)
    {
        for (int i = 0; i < _data[DIRECTIVES].Count; i++)
        {
            JSONNode tNode = _data[DIRECTIVES][i];
            if (aDirective.Equals(tNode))
            {
                _data[DIRECTIVES].Remove(tNode);
                return;
            }
        }
    }

    void RemoveDirectiveFromGroup(BuildTargetGroup aGroup, string aDirective)
    {
        JSONNode tDirs = _data[PLATFORMS][aGroup.ToString()];
        for (int i = 0; i < tDirs.Count; i++)
        {
            JSONNode tNode = tDirs[i];
            if (aDirective.Equals(tNode))
            {
                _data[PLATFORMS][aGroup.ToString()].Remove(tNode);
                return;
            }
        }
    }

    void AddDirectiveToGroup(BuildTargetGroup aGroup, string aDirective)
    {
        if (!HasDirective(aGroup, aDirective))
            _data[PLATFORMS][aGroup.ToString()].Add(aDirective);
    }

    bool IsDirectiveSaved(string aDirective)
    {
        JSONNode tNode = _data[DIRECTIVES];
        if (tNode != null)
        {
            foreach (JSONNode tChild in tNode.Children)
            {
                if (aDirective.Equals(tChild))
                    return true;
            }
        }
        return false;
    }

    void Synchronize(BuildTargetGroup aGroup)
    {
        string tDefines = "";
        JSONNode tPlatform = _data[PLATFORMS][aGroup.ToString()];
        foreach (JSONNode tNode in tPlatform.Children)
            tDefines += tNode + ";";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(aGroup, tDefines);
    }

    [System.Serializable]
    class CSavedDirectives
    {
        public string[] directives;
        public bool[] values;

        public CSavedDirectives()
        {
            directives = new string[0];
            values = new bool[0];
        }

        public void SetVars(string[] aVars)
        {
            if (directives == null)
            {
                directives = aVars;
                values = new bool[directives.Length];
            }
            else
            {
                List<int> tRemain = new List<int>();
                List<string> tExtra = new List<string>();
                for (int i = 0; i < aVars.Length; i++)
                {
                    string tVar = aVars[i];
                    int tIndex = GetIndex(tVar);
                    tRemain.Add(tIndex);
                    if (tIndex < 0)
                        tExtra.Add(tVar);
                }
                string[] tVars = new string[tRemain.Count + tExtra.Count];
                bool[] tValues = new bool[tRemain.Count + tExtra.Count];
                int tExInd = 0;
                for (int i = 0; i < tVars.Length; i++)
                {
                    int tInd = tRemain[i];
                    if (tInd > 0)
                    {
                        tVars[i] = directives[tInd];
                        tValues[i] = values[tInd];
                    }
                    else
                    {
                        tVars[i] = tExtra[tExInd];
                        tValues[i] = true;
                    }
                }
                directives = tVars;
                values = tValues;
            }
        }

        public int GetIndex(string aVar)
        {
            for (int i = 0; i < directives.Length; i++)
            {
                if (directives[i] == aVar)
                    return i;
            }
            return -1;
        }

        public void AddVar(string aVar)
        {
            ArrayUtility.Add<string>(ref directives, aVar);
            ArrayUtility.Add<bool>(ref values, true);
        }

        public void RemoveVar(int aVar)
        {
            ArrayUtility.RemoveAt<string>(ref directives, aVar);
            ArrayUtility.RemoveAt<bool>(ref values, aVar);
        }

        public void InterpretList(string aString)
        {
            if (aString != "")
            {
                string[] tVars = aString.Split(';');
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = false;
                }
                for (int i = 0; i < tVars.Length; i++)
                {
                    int tInd = GetIndex(tVars[i]);
                    if (tInd >= 0)
                        values[tInd] = true;
                    else
                    {
                        AddVar(tVars[i]);
                    }
                }
            }
        }

        public void SetList(BuildTargetGroup aGroup)
        {
            if (directives.Length > 0)
            {
                string tRes = "";
                for (int i = 0; i < directives.Length; i++)
                {
                    if (values[i])
                        tRes += directives[i] + ";";
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(aGroup, tRes);
            }
        }
    }
}
