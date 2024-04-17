using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text moneyText;
    public TMP_Text gemsText;

    private static GameManager instance;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        GameStateManager.EconomyManager.InitializeValues();
        UpdateUI();
    }

    private void UpdateUI()
    {
        moneyText.text = "Money: " + GameStateManager.EconomyManager.Money.ToString("0.00"); // Format as currency
        gemsText.text = "Gems: " + GameStateManager.EconomyManager.Gems.ToString();
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

    public void TestAddGems(float amount)
    {
        GameStateManager.EconomyManager.AddGems(amount);
        UpdateUI();
    }

    public void TestReduceMoney()
    {
        GameStateManager.EconomyManager.ReduceMoney(20);
        UpdateUI();
    }
}
