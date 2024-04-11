using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public InitializeAds initializeAds;
    public BannerAds bannerAds;
    public InterstitialAds interstitialAds;
    public RewardedAds rewardedAds;

    public static AdsManager Instance { get; private set; }


    // private void Awake()
    // {
    //     if (Instance != null && Instance != this)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    //     Instance = this;
    //     DontDestroyOnLoad(gameObject);


    //     bannerAds.LoadBannerAd();
    //     interstitialAds.LoadInterstitialAd();
    //     rewardedAds.LoadRewardedAd();
    // }

    private void Awake()
    {
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);

    // Initialize Unity Ads (if not already initialized)
    initializeAds.InitializeUnityAds();

    // Load ads after Unity Ads is initialized
    StartCoroutine(LoadAdsAfterInitialization());
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
}
