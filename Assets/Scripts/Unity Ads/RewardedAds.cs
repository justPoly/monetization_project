using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using Firebase.Analytics;
using Firebase;
using Firebase.Extensions;
using UnityEngine.Events;
using System.Threading.Tasks;

public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iosAdUnitId;

    private string adUnitId;

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
        LoadRewardedAd();
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

    public void LoadRewardedAd()
    {
        if (!AdsManager.Instance.adsDisabled)
        {
            Advertisement.Load(adUnitId, this);
            FirebaseAnalytics.LogEvent("rewarded_ad_load_attempt", "ad_unit_id", adUnitId);
        }
    }

    public void ShowRewardedAd()
    {
        if (!AdsManager.Instance.adsDisabled)
        {
            Advertisement.Show(adUnitId, this);
            FirebaseAnalytics.LogEvent("rewarded_ad_show_attempt", "ad_unit_id", adUnitId);
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Rewarded Ad Loaded");
        FirebaseAnalytics.LogEvent("rewarded_ad_loaded", "placement_id", placementId);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load rewarded ad: {message}");
        FirebaseAnalytics.LogEvent("rewarded_ad_load_failed", new Parameter[] {
            new Parameter("placement_id", placementId),
            new Parameter("error", error.ToString()),
            new Parameter("message", message)
        });
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed to show rewarded ad: {message}");
        FirebaseAnalytics.LogEvent("rewarded_ad_show_failed", new Parameter[] {
            new Parameter("placement_id", placementId),
            new Parameter("error", error.ToString()),
            new Parameter("message", message)
        });
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Rewarded Ad Started");
        FirebaseAnalytics.LogEvent("rewarded_ad_started", "placement_id", placementId);
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Rewarded Ad Clicked");
        FirebaseAnalytics.LogEvent("rewarded_ad_clicked", "placement_id", placementId);
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == adUnitId)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                Debug.Log("Ads Fully Watched .....");
                GameManager.Instance.TestAddMoney();
                LoadRewardedAd();  // Preload the next ad
                FirebaseAnalytics.LogEvent("rewarded_ad_completed", "placement_id", placementId);
            }
            else if (showCompletionState == UnityAdsShowCompletionState.SKIPPED)
            {
                Debug.Log("Ads Skipped .....");
                FirebaseAnalytics.LogEvent("rewarded_ad_skipped", "placement_id", placementId);
            }
            else if (showCompletionState == UnityAdsShowCompletionState.UNKNOWN)
            {
                Debug.Log("Ads completion state unknown .....");
                FirebaseAnalytics.LogEvent("rewarded_ad_unknown_completion", "placement_id", placementId);
            }
        }
    }
}
