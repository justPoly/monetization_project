using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using Firebase.Analytics;

public class BannerAds : MonoBehaviour
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
        
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
    }

    public void LoadBannerAd()
    {
        if (!AdsManager.Instance.adsDisabled)
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
        if (!AdsManager.Instance.adsDisabled)
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

    #region Show Callbacks
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
    #endregion

    #region Load Callbacks
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
        FirebaseAnalytics.LogEvent("banner_ad_loaded", "ad_unit_id", adUnitId);
    }
    #endregion
}
