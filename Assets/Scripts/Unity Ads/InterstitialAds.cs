using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using Firebase.Analytics;
using UnityEngine.Events;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iosAdUnitId;

    private string adUnitId;

    // Event to handle scene transition
    public UnityEvent OnAdCompleted;

    public bool AdCompleted { get; private set; }

    // Property to track if ad is loaded
    public bool isAdLoaded;

    private void Awake()
    {
        #if UNITY_IOS
            adUnitId = iosAdUnitId;
        #elif UNITY_ANDROID
            adUnitId = androidAdUnitId;
        #endif
    }

    private void Start()
    {
        // Ensure AdsManager is initialized before loading the ad
        if (AdsManager.Instance != null)
        {
            LoadInterstitialAd();
        }
        else
        {
            Debug.LogError("AdsManager.Instance is null. Make sure AdsManager is initialized.");
        }
    }

    public void LoadInterstitialAd()
    {
        if (!AdsManager.Instance.adsDisabled)
        {
            Advertisement.Load(adUnitId, this);
            FirebaseAnalytics.LogEvent("interstitial_ad_load_attempt", "ad_unit_id", adUnitId);
        }
    }

    public void ShowInterstitialAd()
    {
        if (!AdsManager.Instance.adsDisabled && isAdLoaded)
        {
            AdCompleted = false;
            Advertisement.Show(adUnitId, this);
            FirebaseAnalytics.LogEvent("interstitial_ad_show_attempt", "ad_unit_id", adUnitId);
        }
        else
        {
            // Directly invoke ad completion if ads are disabled or not loaded
            AdCompleted = true;
            OnAdCompleted?.Invoke();
        }
    }

    #region LoadCallbacks
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial Ad Loaded");
        isAdLoaded = true;
        FirebaseAnalytics.LogEvent("interstitial_ad_loaded", "placement_id", placementId);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load interstitial ad: {message}");
        isAdLoaded = false;
        LoadInterstitialAd();
        FirebaseAnalytics.LogEvent("interstitial_ad_load_failed", new Parameter[] {
            new Parameter("placement_id", placementId),
            new Parameter("error", error.ToString()),
            new Parameter("message", message)
        });
    }
    #endregion

    #region ShowCallbacks
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed to show interstitial ad: {message}");
        AdCompleted = true; // Set ad completion state even on failure
        OnAdCompleted?.Invoke();
        FirebaseAnalytics.LogEvent("interstitial_ad_show_failed", new Parameter[] {
            new Parameter("placement_id", placementId),
            new Parameter("error", error.ToString()),
            new Parameter("message", message)
        });
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Interstitial Ad Started");
        FirebaseAnalytics.LogEvent("interstitial_ad_started", "placement_id", placementId);
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Interstitial Ad Clicked");
        FirebaseAnalytics.LogEvent("interstitial_ad_clicked", "placement_id", placementId);
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log("Interstitial Ad Completed");
        FirebaseAnalytics.LogEvent("interstitial_ad_completed", new Parameter[] 
        {
            new Parameter("placement_id", placementId),
            new Parameter("completion_state", showCompletionState.ToString())
        });
        // Set ad completion state
        AdCompleted = true;
        OnAdCompleted?.Invoke();
        // Preload the next ad
        isAdLoaded = false;
        LoadInterstitialAd();
    }
    #endregion
}
