using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Analytics;

public class GameManager : MonoBehaviour
{
    public TMP_Text diamondText;
    public TMP_Text gemsText;

    private static GameManager instance;
    private bool adsEnabled = true;

    public GameObject adTypePopup;
    [SerializeField]
    private GameObject storeEnquiryPopUp;
    
    [SerializeField]
    private GameObject adTestCompletedPopUp;

    public Button[] buttonPrefabs;
    private int currentButtonIndex = -1;
    private string currentAdType;
    private bool allButtonsTested; 

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
        // GameStateManager.EconomyManager.InitializeValues();
        ShowNextButton();
        adTypePopup.SetActive(false); 
        storeEnquiryPopUp.SetActive(false);
        allButtonsTested = false; 
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
        if (allButtonsTested)
        {
            RepeatProcess();
        }

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

    public void StashBannerAd()
    {
        AdsManager.Instance.bannerAds.HideBannerAd(); 
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

    public void OnButtonClick(string adType)
    {
        currentAdType = adType; 
        StartCoroutine(ShowAdTypePopupCoroutine());
    }

    private IEnumerator ShowAdTypePopupCoroutine()
    {
        yield return new WaitForSeconds(0.5f); 
        adTypePopup.SetActive(true);
    }

    public void OnYesButtonClick()
    {
        Debug.Log("User saw " + currentAdType + " ad.");
        adTypePopup.SetActive(false); // Hide the popup after selection
        SendFirebaseEvent(currentAdType, true); // Send Firebase event for "Yes" response
        DisablePreviousButton(); // Disable the previous button
        ShowNextButton(); // Show the next button or repeat the process
    }

    public void OnNoButtonClick()
    {
        Debug.Log("User did not see " + currentAdType + " ad.");
        adTypePopup.SetActive(false); // Hide the popup after selection
        SendFirebaseEvent(currentAdType, false); // Send Firebase event for "No" response
        DisablePreviousButton(); // Disable the previous button
        ShowNextButton(); // Show the next button or repeat the process
    }

    public void ShowNextButton()
    {
        // Increment the button index
        currentButtonIndex++;
        if (currentButtonIndex < buttonPrefabs.Length)
        {
            // Enable the next button in the array
            buttonPrefabs[currentButtonIndex].gameObject.SetActive(true);
        }
        else
        {
            allButtonsTested = true;
            StartCoroutine(storeEnquiryCoroutine());
            Debug.Log("All buttons tested.");
        }
    }

    private IEnumerator storeEnquiryCoroutine()
    {
        yield return new WaitForSeconds(0.5f); 
        adTestCompletedPopUp.SetActive(true);
        yield return new WaitForSeconds(4f); 
        adTestCompletedPopUp.SetActive(false);
        storeEnquiryPopUp.SetActive(true);
    }

    private void DisablePreviousButton()
    {
        if (currentButtonIndex >= 0 && currentButtonIndex < buttonPrefabs.Length)
        {
            // Disable the previous button in the array
            buttonPrefabs[currentButtonIndex].gameObject.SetActive(false);
        }
    }

    private void SendFirebaseEvent(string adType, bool sawAd)
    {
        string eventName = sawAd ? "AdSeen" : "AdNotSeen";
        FirebaseAnalytics.LogEvent(eventName, "AdType", adType);
        Debug.Log(adType);
    }

    
    private void RepeatProcess()
    {
        currentButtonIndex = -1; // Reset button index to start over
        allButtonsTested = false; // Reset the flag
        ShowNextButton(); // Start testing the buttons again
    }

    //Yes button when asked to navigate to the store
    public void TrackYesButtonPress()
    {
        storeEnquiryPopUp.SetActive(false);
        FirebaseAnalytics.LogEvent("nav_to_store_response", new Parameter("response", "yes"));
        Debug.Log("Firebase Analytics: Yes button pressed.");
    }

    //No button when asked to navigate to the store
    public void TrackNoButtonPress()
    {
        storeEnquiryPopUp.SetActive(false);
        FirebaseAnalytics.LogEvent("nav_to_store_response", new Parameter("response", "no"));
        Debug.Log("Firebase Analytics: No button pressed.");
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
