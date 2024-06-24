using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using Firebase.Analytics;
using UnityEngine.SceneManagement;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iosAdUnitId;

    private string adUnitId;
    private bool isTestingAd = false;

    public static event Action OnInterstitialAdCompleted; // Define an event

    private void Awake()
    {
        #if UNITY_IOS
            adUnitId = iosAdUnitId;
        #elif UNITY_ANDROID
            adUnitId = androidAdUnitId;
        #endif
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
        if (!AdsManager.Instance.adsDisabled)
        {
            Advertisement.Show(adUnitId, this);
            FirebaseAnalytics.LogEvent("interstitial_ad_show_attempt", "ad_unit_id", adUnitId);
            LoadInterstitialAd();  // Preload the next ad
        }
    }

    public void ShowInterstitialAdTesting()
    {
        if (!AdsManager.Instance.adsDisabled)
        {
            isTestingAd = true;
            Advertisement.Show(adUnitId, this);
            FirebaseAnalytics.LogEvent("interstitial_ad_show_attempt", "ad_unit_id", adUnitId);
            LoadInterstitialAd();  // Preload the next ad
        }
    }

    #region LoadCallbacks
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial Ad Loaded");
        FirebaseAnalytics.LogEvent("interstitial_ad_loaded", "placement_id", placementId);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load interstitial ad: {message}");
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
        FirebaseAnalytics.LogEvent("interstitial_ad_completed", new Parameter[] {
            new Parameter("placement_id", placementId),
            new Parameter("completion_state", showCompletionState.ToString())
        });

        if (isTestingAd)
        {
            isTestingAd = false;
            OnInterstitialAdCompleted?.Invoke(); // Invoke the event when ad is completed
        }
    }
    #endregion
}
