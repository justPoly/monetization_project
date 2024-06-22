using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
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

    public TextMeshProUGUI b1text;

    public Button[] buttonPrefabs;
    private int currentButtonIndex = -1;
    private string currentAdType;
    private bool allButtonsTested;

    private int currentMoneyIncrement = 0;

    public static GameManager Instance => instance ??= FindObjectOfType<GameManager>() ?? new GameObject("GameManagerSingleton").AddComponent<GameManager>();

    private void Awake()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        ShowNextButton();
        SetActiveIfNotNull(adTypePopup, false);
        SetActiveIfNotNull(storeEnquiryPopUp, false);
        allButtonsTested = false;
        SetActiveIfNotNull(noInternetPanel, false);
    }

    IEnumerator CheckInternetConnection(string adType)
    {
        UnityWebRequest request = new UnityWebRequest("https://www.maliyo.com");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            SetActiveIfNotNull(noInternetPanel, true);
        }
        else
        {
            ShowAd(adType);
            SetActiveIfNotNull(noInternetPanel, false);
            StartCoroutine(ShowAdTypePopupCoroutine());
        }
    }

    public void UpdateUI()
    {
        if (GameStateManager.EconomyManager == null)
        {
            Debug.LogError("EconomyManager is null");
            return;
        }

        diamondText.text = $": {((int)GameStateManager.EconomyManager.Money).ToString()}";
        gemsText.text = $": {((int)GameStateManager.EconomyManager.Gems).ToString()}";
        miniDiamondText.text = $": {((int)GameStateManager.EconomyManager.Money).ToString()}";
        miniGemsText.text = $": {((int)GameStateManager.EconomyManager.Gems).ToString()}";
    }


    void Update()
    {
        if (allButtonsTested)
        {
            RepeatProcess();
        }
         UpdateUI();
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
        AdsManager.Instance.interstitialAds.ShowInterstitialAdsForTesting();
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
        AdsManager.Instance?.bannerAds?.HideBannerAd();
    }

    public void TestAddMoney()
    {
        GameStateManager.EconomyManager?.AddMoney(currentMoneyIncrement);
        currentMoneyIncrement += 5;
        UpdateUI();
    }

    public void TestAddGems()
    {
        GameStateManager.EconomyManager?.AddGems(10);
        UpdateUI();
    }

    public void TestReduceMoney()
    {
        GameStateManager.EconomyManager?.ReduceMoney(20);
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
        SetActiveIfNotNull(adTypePopup, true);
    }

    public void OnYesButtonClick()
    {
        Debug.Log("User saw " + currentAdType + " ad.");
        SetActiveIfNotNull(adTypePopup, false);
        SendFirebaseEvent(currentAdType, true);
        DisablePreviousButton();
        ShowNextButton();
    }

    public void OnNoButtonClick()
    {
        Debug.Log("User did not see " + currentAdType + " ad.");
        SetActiveIfNotNull(adTypePopup, false);
        SendFirebaseEvent(currentAdType, false);
        DisablePreviousButton();
        ShowNextButton();
    }

    public void ShowNextButton()
    {
        Debug.Log("ShowNextButton called.");
        currentButtonIndex++;
        Debug.Log("Current button index: " + currentButtonIndex);
        if (currentButtonIndex < buttonPrefabs.Length)
        {
            buttonPrefabs[currentButtonIndex]?.gameObject.SetActive(true);
            Debug.Log("Enabled button: " + buttonPrefabs[currentButtonIndex].name);
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
        b1text = buttonPrefabs[0]?.GetComponentInChildren<TextMeshProUGUI>();
        if (b1text != null)
        {
            b1text.text = "Test again";
        }
    }

    private IEnumerator storeEnquiryCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SetActiveIfNotNull(adTestCompletedPopUp, true);
        yield return new WaitForSeconds(4f);
        SetActiveIfNotNull(adTestCompletedPopUp, false);
        SetActiveIfNotNull(storeEnquiryPopUp, true);
    }

    private void DisablePreviousButton()
    {
        if (currentButtonIndex >= 0 && currentButtonIndex < buttonPrefabs.Length)
        {
            buttonPrefabs[currentButtonIndex]?.gameObject.SetActive(false);
        }
    }

    private void SendFirebaseEvent(string adType, bool sawAd)
    {
        string eventName = sawAd ? "AdSeen" : "AdNotSeen";
        FirebaseAnalytics.LogEvent(eventName, "AdType", adType);
        Debug.Log("Firebase event sent for " + adType);
    }

    private void RepeatProcess()
    {
        currentButtonIndex = -1;
        allButtonsTested = false;
        ShowNextButton();
    }

    public void TrackYesButtonPress()
    {
        SetActiveIfNotNull(storeEnquiryPopUp, false);
        FirebaseAnalytics.LogEvent("nav_to_store_response", new Parameter("response", "yes"));
        Debug.Log("Firebase Analytics: Yes button pressed.");
    }

    public void TrackNoButtonPress()
    {
        SetActiveIfNotNull(storeEnquiryPopUp, false);
        FirebaseAnalytics.LogEvent("nav_to_store_response", new Parameter("response", "no"));
        Debug.Log("Firebase Analytics: No button pressed.");
    }

    private void SetActiveIfNotNull(GameObject obj, bool isActive)
    {
        if (obj != null)
        {
            obj.SetActive(isActive);
        }
    }
}
