using UnityEditor;
using UnityEngine;
using UnityEngine.Advertisements;

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

    private float timer;
    private bool isActionDelayed;
    private float delayDuration = 1f; // Adjust the delay duration as needed

    [SerializeField] private ApplicationManager m_applicationManager;
    private ApplicationManager m_saveApplicationManager;

    [SerializeField] private EconomyManager m_economyManager;
    private EconomyManager m_saveEconomyManager;

    public static ApplicationManager ApplicationManager
    {
        get { return Instance.m_applicationManager; }

    }

    public static EconomyManager EconomyManager
    {
        get { return Instance.m_economyManager; }

    }

    public void Init()
    {
       isActionDelayed = true;
       timer = 0f;
    }

    public void Update()
    {
        if (isActionDelayed)
        {
            timer += Time.deltaTime;
            if (timer >= delayDuration)
            {
                // Perform delay action here
                AdsManager.Instance.bannerAds.ShowBannerAd();
                // Reset the timer and flag
                timer = 0f;
                isActionDelayed = false;
            }
        }
    }

    public void InitializeValues()
    {
        EconomyManager.InitializeValues();
    }

    public void UpdateCurrency()
    {
        GameManager.Instance.UpdateUI();
    }

}