using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Extensions;
using UnityEngine.Advertisements;

public class GameStateManager : SingletonScriptableObject<GameStateManager>
{
    [SerializeField] private ApplicationManager m_applicationManager;
    [SerializeField] private EconomyManager m_economyManager;

    private bool isActionDelayed;
    private float delayDuration = 1f; // Adjust the delay duration as needed

    public static ApplicationManager ApplicationManager => Instance.m_applicationManager;
    public static EconomyManager EconomyManager => Instance.m_economyManager;

    private DelayHandler delayHandler; // Reference to the MonoBehaviour that handles the coroutine

    private void Start()
    {
        delayHandler = new GameObject("DelayHandler").AddComponent<DelayHandler>(); // Create the MonoBehaviour
        delayHandler.Initialize(this);
    }

    public void DelayBannerAd()
    {
        delayHandler.StartCoroutine(delayHandler.DelayCoroutine());
    }

    public void Update()
    {
        // Other update logic here
    }

    public void InitializeValues()
    {
        EconomyManager.InitializeValues();
    }

    public void UpdateCurrency()
    {
        GameManager.Instance.UpdateUI();
    }

    private class DelayHandler : MonoBehaviour
    {
        private GameStateManager gameStateManager;

        public void Initialize(GameStateManager manager)
        {
            gameStateManager = manager;
        }

        public IEnumerator DelayCoroutine()
        {
            yield return new WaitUntil(() => FirebaseApp.CheckAndFixDependenciesAsync().IsCompleted);

            // Now that Firebase initialization is done, show the banner ad
            AdsManager.Instance.bannerAds.ShowBannerAd();

            gameStateManager.isActionDelayed = false; // Reset the delay flag
        }
    }
}
