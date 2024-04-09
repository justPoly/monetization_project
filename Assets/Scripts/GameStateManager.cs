using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/GameState Manager")]
#if UNITY_EDITOR
[FilePath("Scriptable Objects/Managers/GameStateManager.asset", FilePathAttribute.Location.PreferencesFolder)]
#endif
public class GameStateManager : SingletonScriptableObject<GameStateManager>
{
    [FancyHeader("GAMESTATE MANAGER", 3f, "#D4AF37", 8.5f, order = 0)]
    [Space(order = 1)]
    [CustomProgressBar(hideWhenZero = true, label = "m_loadingTxt"), SerializeField] public float m_loadingBar;
    [HideInInspector] public string m_loadingTxt;
    [HideInInspector] public bool m_loadingDone = false;

    [SerializeField] private ApplicationManager m_applicationManager;
    private ApplicationManager m_saveApplicationManager;


    public static ApplicationManager ApplicationManager
    {
        get { return Instance.m_applicationManager; }

    }

    public void Init()
    {
        
    }

}