using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text diamondText;
    public TMP_Text gemsText;

    private static GameManager instance;
    private bool adsEnabled = true;

    public static GameManager Instance => instance ??= FindObjectOfType<GameManager>() ?? new GameObject("GameManagerSingleton").AddComponent<GameManager>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        UpdateUI();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameStateManager.EconomyManager.InitializeValues();
    }

    public void UpdateUI()
    {
        // Update moneyText and gemsText directly based on the current counts and economy manager values
        diamondText.text = $"Diamonds: {((int)GameStateManager.EconomyManager.Money).ToString()}";
        gemsText.text = $"Gems: {((int)GameStateManager.EconomyManager.Gems).ToString()}";
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayInterstitialAds()
    {
        AdsManager.Instance.interstitialAds.ShowInterstitialAd();
    }

    public void DisplayRewardedAds()
    {
        AdsManager.Instance.rewardedAds.ShowRewardedAd();
    }

    public void DisplayBannerAds()
    {
        AdsManager.Instance.bannerAds.ShowBannerAd();
    }

    public void TestAddMoney()
    {
        GameStateManager.EconomyManager.AddMoney(30);
        UpdateUI();
    }

    public void TestAddGems()
    {
        GameStateManager.EconomyManager.AddGems(10);
        UpdateUI();
    }

    public void TestReduceMoney()
    {
        GameStateManager.EconomyManager.ReduceMoney(20);
        UpdateUI();
    }

    public void DisableAdsFor30Days()
    {
        adsEnabled = false;
        AdsManager.Instance.bannerAds.HideBannerAd(); 
        AdsManager.Instance.interstitialAds.HideInterstitialAd();
        AdsManager.Instance.rewardedAds.HideRewardedAd(); 
    }

    public void EnableAdsAgain()
    {
        adsEnabled = true;
        AdsManager.Instance.bannerAds.ShowBannerAd();
        AdsManager.Instance.interstitialAds.LoadInterstitialAd();
        AdsManager.Instance.rewardedAds.LoadRewardedAd(); 
    }

    public void DisableAllAdsForever()
    {
        adsEnabled = false;
        AdsManager.Instance.bannerAds.HideBannerAd(); 
        AdsManager.Instance.interstitialAds.HideInterstitialAd();
        AdsManager.Instance.rewardedAds.HideRewardedAd(); 
        PlayerPrefs.SetInt("NoAdsPurchased", 1); 
    }

    public void EnableAllAdsForever()
    {
        adsEnabled = PlayerPrefs.GetInt("NoAdsPurchased", 0) != 1;
        if (adsEnabled)
        {
            AdsManager.Instance.bannerAds.ShowBannerAd();
            AdsManager.Instance.interstitialAds.LoadInterstitialAd();
            AdsManager.Instance.rewardedAds.LoadRewardedAd(); 
        }
    }
}
