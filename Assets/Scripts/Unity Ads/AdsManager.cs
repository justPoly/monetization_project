using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using Firebase.Extensions;
using Firebase;

public class AdsManager : MonoBehaviour
{
    public InitializeAds initializeAds;
    public BannerAds bannerAds;
    public InterstitialAds interstitialAds;
    public RewardedAds rewardedAds;

    public static AdsManager Instance { get; private set; }

    public bool adsDisabled { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        adsDisabled = PlayerPrefs.GetInt("NoAds", 0) == 1;

        if (!adsDisabled)
        {
            initializeAds.InitializeUnityAds();
            StartCoroutine(LoadAdsAfterInitialization());
        }
    }

    private IEnumerator LoadAdsAfterInitialization()
    {
        // Wait for Firebase initialization to complete
        var checkDependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => checkDependencyTask.IsCompleted);

        if (checkDependencyTask.Result == DependencyStatus.Available)
        {
            // Initialize and load ads now that Firebase is initialized and supported
            StartCoroutine(InitializeAndLoadAds());
        }
        else
        {
            Debug.LogError("Firebase initialization failed.");
        }
    }

    private IEnumerator InitializeAndLoadAds()
    {
        // Ensure BannerAds has initialized Firebase and loaded the banner ad
        yield return bannerAds.InitializeFirebaseAndLoadBannerAd();

        // Load other ad types
        interstitialAds.LoadInterstitialAd();
        rewardedAds.LoadRewardedAd();
    }

    public void DisableAds()
    {
        adsDisabled = true;
        PlayerPrefs.SetInt("NoAds", 1);
        PlayerPrefs.Save();
        bannerAds.HideBannerAd();
        Debug.Log("Ads disabled.");
    }

    public void EnableAds()
    {
        adsDisabled = false;
        PlayerPrefs.SetInt("NoAds", 0);
        PlayerPrefs.Save();

        if (!Advertisement.isInitialized || !Advertisement.isSupported)
        {
            initializeAds.InitializeUnityAds();
            StartCoroutine(LoadAdsAfterInitialization());
        }
        else
        {
            StartCoroutine(InitializeAndLoadAds());
        }
        bannerAds.ShowBannerAd();
        Debug.Log("Ads enabled.");
    }
}
