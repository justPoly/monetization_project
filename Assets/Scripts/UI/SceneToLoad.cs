using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneToLoad : MonoBehaviour
{
    public string sceneName;
    public int countDownTime;
    public SceneFader sceneFader;
    
    private void Start()
    {
        // Subscribe to the OnAdCompleted event
        if (AdsManager.Instance != null && AdsManager.Instance.interstitialAds != null)
        {
            AdsManager.Instance.interstitialAds.OnAdCompleted.AddListener(OnAdCompleted);
        }
    }

    public void MoveToLoading()
    {
        if (AdsManager.Instance != null)
        {
            StartCoroutine(ShowAdAndTransition());
        }
        else
        {
            StartSceneTransition();
        }
    }

    private IEnumerator ShowAdAndTransition()
    {
        // Show the interstitial ad
        AdsManager.Instance.interstitialAds.ShowInterstitialAd();
        // Wait until the ad is completed
        while (!AdsManager.Instance.interstitialAds.AdCompleted)
        {
            yield return null;
        }
        StartSceneTransition();
    }

    private void StartSceneTransition()
    {
        GameStateManager.ApplicationManager.PlayGame();
        GameStateManager.ApplicationManager.OnSceneLoad.Raise();
        StartCoroutine(CountDown());
    }



    private void OnAdCompleted()
    {
        // Perform scene transition when the ad is completed
        sceneFader.FadeTo(sceneName);
    }

    public void RestartScene()
    {
        GameStateManager.ApplicationManager.OnSceneLoad.Raise();
        StartCoroutine(CountDownToSame());
    }

    IEnumerator CountDown()
    {
        while (countDownTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countDownTime--;
        }

        sceneFader.FadeTo(sceneName);
    }

    IEnumerator CountDownToSame()
    {
        while (countDownTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countDownTime--;
        }

        sceneFader.FadeToSame();


    }

}
