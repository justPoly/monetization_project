using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text moneyText;
    public TMP_Text gemsText;

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

    private void Awake()
    {
    
    }

    public void DisplayIntertitialAds()
    { 
       AdsManager.Instance.interstitialAds.ShowInterstitialAd();
    }

    public void DisplayRewardedAds()
    {
        AdsManager.Instance.rewardedAds.ShowRewardedAd();
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
