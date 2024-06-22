using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Advertisements;
using Firebase.Analytics;
using Firebase;

public class BannerAds : MonoBehaviour
{
    [SerializeField] private string androidAdUnitId;
    [SerializeField] private string iosAdUnitId;

    private string adUnitId;
    private bool isFirebaseInitialized = false;
    private bool isBannerLoaded = false;

    private void Awake()
    {
        #if UNITY_IOS
            adUnitId = iosAdUnitId;
        #elif UNITY_ANDROID
            adUnitId = androidAdUnitId;
        #endif
        
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
    }

    private async void Start()
    {
        await InitializeFirebaseAndLoadBannerAd();
    }

    public async Task InitializeFirebaseAndLoadBannerAd()  // Change this method to public
    {
        if (!AdsManager.Instance.adsDisabled)
        {
            await CheckFirebaseDependenciesAsync();
            isFirebaseInitialized = true;
            LoadBanner();
        }
    }

    private async Task CheckFirebaseDependenciesAsync()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            // Handle dependency resolution failure (e.g., show an error message)
        }
    }

    private void LoadBanner()
    {
        if (isFirebaseInitialized && !isBannerLoaded)
        {
            BannerLoadOptions options = new BannerLoadOptions
            {
                loadCallback = BannerLoaded,
                errorCallback = BannerLoadedError
            };

            Advertisement.Banner.Load(adUnitId, options);
            FirebaseAnalytics.LogEvent("banner_ad_load_attempt", "ad_unit_id", adUnitId);
        }
    }

    public void ShowBannerAd()
    {
        if (isFirebaseInitialized && isBannerLoaded && !AdsManager.Instance.adsDisabled)
        {
            BannerOptions options = new BannerOptions
            {
                showCallback = BannerShown,
                clickCallback = BannerClicked,
                hideCallback = BannerHidden
            };

            Advertisement.Banner.Show(adUnitId, options);
            FirebaseAnalytics.LogEvent("banner_ad_show_attempt", "ad_unit_id", adUnitId);
        }
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
        FirebaseAnalytics.LogEvent("banner_ad_hidden", "ad_unit_id", adUnitId);
    }

    private void BannerHidden()
    {
        Debug.Log("Banner Ad Hidden");
        FirebaseAnalytics.LogEvent("banner_ad_hidden", "ad_unit_id", adUnitId);
    }

    private void BannerClicked()
    {
        Debug.Log("Banner Ad Clicked");
        FirebaseAnalytics.LogEvent("banner_ad_clicked", "ad_unit_id", adUnitId);
    }

    private void BannerShown()
    {
        Debug.Log("Banner Ad Shown");
        FirebaseAnalytics.LogEvent("banner_ad_shown", "ad_unit_id", adUnitId);
    }

    private void BannerLoadedError(string message)
    {
        Debug.LogError("Banner Ad Load Failed: " + message);
        FirebaseAnalytics.LogEvent("banner_ad_load_failed", new Parameter[] {
            new Parameter("ad_unit_id", adUnitId),
            new Parameter("error_message", message)
        });
    }

    private void BannerLoaded()
    {
        Debug.Log("Banner Ad Loaded");
        isBannerLoaded = true;
        FirebaseAnalytics.LogEvent("banner_ad_loaded", "ad_unit_id", adUnitId);
    }
}
