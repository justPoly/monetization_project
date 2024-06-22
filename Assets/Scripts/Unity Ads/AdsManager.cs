using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;


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
            // Initialize Unity Ads (if not already initialized)
            initializeAds.InitializeUnityAds();
            // Load ads after Unity Ads is initialized
            StartCoroutine(LoadAdsAfterInitialization());
        }
    }

    private IEnumerator LoadAdsAfterInitialization()
    {
         // Wait for Unity Ads initialization to complete
        while (!Advertisement.isInitialized || !Advertisement.isSupported)
        {
            yield return null;
        }

         // Load ads now that Unity Ads is initialized and supported
        bannerAds.LoadBannerAd();
        interstitialAds.LoadInterstitialAd();
        rewardedAds.LoadRewardedAd();
    }

    public void DisableAds()
    {
        adsDisabled = true;
        // Replace with cloud save or any other alternative;
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

        // Initialize Unity Ads if necessary and load ads
        if (!Advertisement.isInitialized || !Advertisement.isSupported)
        {
            initializeAds.InitializeUnityAds();
            StartCoroutine(LoadAdsAfterInitialization());
        }
        else
        {
            // Directly load ads if already initialized
            bannerAds.LoadBannerAd();
            interstitialAds.LoadInterstitialAd();
            rewardedAds.LoadRewardedAd();
        }
        bannerAds.ShowBannerAd();
        Debug.Log("Ads enabled.");
    }

}
