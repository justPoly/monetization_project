using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Analytics;

public class GameManager : MonoBehaviour
{
    public TMP_Text diamondText, miniDiamondText;
    public TMP_Text gemsText, miniGemsText;

    private static GameManager instance;

    public GameObject adTypePopup;
    [SerializeField]
    private GameObject storeEnquiryPopUp;
    [SerializeField]
    private GameObject adTestCompletedPopUp;
    public GameObject noInternetPanel;

    [SerializeField] private Transform contentParent; // Parent transform to instantiate game entries
    [SerializeField] private GameObject gameEntryPrefab; // Prefab for displaying game entries
    [SerializeField] private GameInfo[] games;

    
    public TextMeshProUGUI b1text;

    public Button[] buttonPrefabs;
    private int currentButtonIndex = -1;
    private string currentAdType;
    private bool allButtonsTested; 

    private int currentMoneyIncrement = 0;

    public static GameManager Instance => instance ??= FindObjectOfType<GameManager>() ?? new GameObject("GameManagerSingleton").AddComponent<GameManager>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
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
        noInternetPanel.SetActive(false);

        foreach (var game in games)
        {
            GameObject gameEntryGO = Instantiate(gameEntryPrefab, contentParent);
            GameEntryUI gameEntryUI = gameEntryGO.GetComponent<GameEntryUI>();
            gameEntryUI.Setup(game);
        }
    }

    IEnumerator CheckInternetConnection(string adType)
    {
        UnityWebRequest request = new UnityWebRequest("https://www.maliyo.com");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            noInternetPanel.SetActive(true);
        }
        else
        {
            ShowAd(adType);
            noInternetPanel.SetActive(false);
            StartCoroutine(ShowAdTypePopupCoroutine());
        }
    }

    public void UpdateUI()
    {
        // Update moneyText and gemsText directly based on the current counts and economy manager values
        diamondText.text = $": {((int)GameStateManager.EconomyManager.Money).ToString()}";
        gemsText.text = $": {((int)GameStateManager.EconomyManager.Gems).ToString()}";
        miniDiamondText.text = $": {((int)GameStateManager.EconomyManager.Money).ToString()}";
        miniGemsText.text = $": {((int)GameStateManager.EconomyManager.Gems).ToString()}";
    }

    // Update is called once per frame
    void Update()
    {
        if (allButtonsTested)
        {
            RepeatProcess();
        }

    }

    private void ShowAd(string adType)
    {
        switch (adType)
        {
            case "BannerAd":
                DisplayBannerAds();
                break;
            case "InterstitialAd":
                DisplayInterstitialAds();
                break;
            case "RewardedAd":
                DisplayRewardedAds();
                break;
            default:
                Debug.LogError("Unknown ad type: " + adType);
                break;
        }
    }

    public void DisplayInterstitialAds()
    {
        AdsManager.Instance.interstitialAds.ShowInterstitialAd();
    }

    public void DisplayInterstitialAdsTest()
    {
        AdsManager.Instance.interstitialAds.ShowInterstitialAdTesting();
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
        GameStateManager.EconomyManager.AddMoney(currentMoneyIncrement);
        currentMoneyIncrement += 5;
        UpdateUI();
    }

    public void TestAddGems()
    {
        GameStateManager.EconomyManager.AddGems(10);
        UpdateUI();
    }

    public void TestReduceMoney()
    {
        GameStateManager.EconomyManager.SpendMoney(20);
        UpdateUI();
    }

    public void OnButtonClick(string adType)
    {
        currentAdType = adType; 
        StartCoroutine(CheckInternetConnection(adType));
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