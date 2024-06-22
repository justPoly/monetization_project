using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using Firebase.Analytics;
using Firebase;
using Firebase.Extensions;
using UnityEngine.Events;
using System.Threading.Tasks;

public class InterstitialAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iosAdUnitId;

    private string adUnitId;
    public UnityEvent OnAdCompleted;
    public bool AdCompleted { get; private set; }
    public bool isAdLoaded { get; private set; }
    private bool isTesting = false;

    private void Awake()
    {
        #if UNITY_IOS
            adUnitId = iosAdUnitId;
        #elif UNITY_ANDROID
            adUnitId = androidAdUnitId;
        #endif
    }

    private async void Start()
    {
        await WaitForFirebaseInitialization();
        LoadInterstitialAd();
    }

    private async Task WaitForFirebaseInitialization()
    {
        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        await dependencyTask;
        if (dependencyTask.Result != DependencyStatus.Available)
        {
            Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyTask.Result);
            // Handle dependency resolution failure (e.g., show an error message)
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
            isTesting = false; // Normal ad show, not for testing
            Advertisement.Show(adUnitId, this);
            FirebaseAnalytics.LogEvent("interstitial_ad_show_attempt", "ad_unit_id", adUnitId);
        }
        else
        {
            AdCompleted = true;
            OnAdCompleted?.Invoke();
        }
    }

    public void ShowInterstitialAdsForTesting()
    {
        if (!AdsManager.Instance.adsDisabled && isAdLoaded)
        {
            AdCompleted = false;
            isTesting = true; // Mark as testing
            Advertisement.Show(adUnitId, this);
            FirebaseAnalytics.LogEvent("interstitial_ad_test_show_attempt", "ad_unit_id", adUnitId);
        }
        else
        {
            Debug.LogWarning("Ad is not loaded or ads are disabled.");
        }
    }

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

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed to show interstitial ad: {message}");
        AdCompleted = true;
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
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.Log("Interstitial Ad Completed");
            FirebaseAnalytics.LogEvent("interstitial_ad_completed", new Parameter[] {
                new Parameter("placement_id", placementId),
                new Parameter("completion_state", showCompletionState.ToString())
            });
        }
        else if (showCompletionState == UnityAdsShowCompletionState.SKIPPED)
        {
            Debug.Log("Interstitial Ad Skipped");
            FirebaseAnalytics.LogEvent("interstitial_ad_skipped", "placement_id", placementId);
        }
        else if (showCompletionState == UnityAdsShowCompletionState.UNKNOWN)
        {
            Debug.Log("Interstitial Ad Completed with Unknown State");
            FirebaseAnalytics.LogEvent("interstitial_ad_unknown_completion", "placement_id", placementId);
        }

        if (!isTesting)
        {
           AdCompleted = true;
           OnAdCompleted?.Invoke();
           isAdLoaded = false;
           LoadInterstitialAd();
        }


    }
}
