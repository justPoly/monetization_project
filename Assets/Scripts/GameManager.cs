using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     private void Awake()
    {
        // if (Instance != null && Instance != this)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        // Instance = this;
        // DontDestroyOnLoad(gameObject);

        // StartCoroutine(DisplayBannerWithDelay());
    }

    public void DisplayIntertitialAds()
    { 
       AdsManager.Instance.interstitialAds.ShowInterstitialAd();
    }
}
