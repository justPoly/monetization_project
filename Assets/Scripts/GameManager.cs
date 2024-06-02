using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine.Networking;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Analytics;

public class GameManager : MonoBehaviour
{
    public TMP_Text diamondText;
    public TMP_Text gemsText;

    private static GameManager instance;

    public GameObject adTypePopup;
    [SerializeField]
    private GameObject storeEnquiryPopUp;
    [SerializeField]
    private GameObject adTestCompletedPopUp;

    public GameObject noInternetPanel;
    private DateTime lastCheckedTime;
    private bool lastConnectionStatus;
    private const float cacheDuration = 30f; 

    public TextMeshProUGUI b1text;

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
        ShowNextButton();
        adTypePopup.SetActive(false); 
        storeEnquiryPopUp.SetActive(false);
        allButtonsTested = false; 
        noInternetPanel.SetActive(false);
    }

    private bool IsCacheValid()
    {
        return (DateTime.Now - lastCheckedTime).TotalSeconds < cacheDuration;
    }

    // Method to check internet connection
    private IEnumerator CheckInternetConnection(System.Action<bool> action)
    {
        if (IsCacheValid())
        {
            action(lastConnectionStatus);
            yield break;
        }

        UnityWebRequest request = new UnityWebRequest("http://google.com");
        request.timeout = 5; // Set a timeout for the request

        yield return request.SendWebRequest();

        lastCheckedTime = DateTime.Now;
        lastConnectionStatus = request.result == UnityWebRequest.Result.Success;

        action(lastConnectionStatus);
    }


    public void UpdateUI()
    {
        // Update moneyText and gemsText directly based on the current counts and economy manager values
        diamondText.text = $": {((int)GameStateManager.EconomyManager.Money).ToString()}";
        gemsText.text = $": {((int)GameStateManager.EconomyManager.Gems).ToString()}";
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
        StartCoroutine(CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                noInternetPanel.SetActive(false);
                Debug.Log("Showing interstitial ad...");
                AdsManager.Instance.interstitialAds.ShowInterstitialAd();
            }
            else
            {
                noInternetPanel.SetActive(true);
            }
        }));
        
    }

    public void DisplayRewardedAds()
    {
         StartCoroutine(CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                noInternetPanel.SetActive(false);
                Debug.Log("Showing rewarded ad...");
                AdsManager.Instance.rewardedAds.ShowRewardedAd();
            }
            else
            {
                noInternetPanel.SetActive(true);
            }
        }));
    }

    public void DisplayBannerAds()
    {
        StartCoroutine(CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                noInternetPanel.SetActive(false);
                Debug.Log("Showing banner ad...");
                AdsManager.Instance.bannerAds.ShowBannerAd();
            }
            else
            {
                noInternetPanel.SetActive(true);
            }
        }));
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
        StartCoroutine(CheckInternetConnection(isConnected =>
        {
            if (isConnected)
            {
                noInternetPanel.SetActive(false);
                adTypePopup.SetActive(true);
            }
            else
            {
                noInternetPanel.SetActive(true);
            }
        }));
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
            UpdateButtonText();
            Debug.Log("All buttons tested.");
        }
    }

    public void UpdateButtonText()
    {
        b1text = buttonPrefabs[0].GetComponentInChildren<TextMeshProUGUI>();
        b1text.text = "Test again";
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
  
}
