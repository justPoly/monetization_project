using System;
using System.Collections;
using UnityEngine;

public class SceneToLoad : MonoBehaviour
{
    public string sceneName;
    public int countDownTime;
    public SceneFader sceneFader;

    private void OnEnable()
    {
        // Subscribe to the event when this script is enabled
        InterstitialAds.OnInterstitialAdCompleted += AdCompletedHandler;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when this script is disabled or destroyed
        InterstitialAds.OnInterstitialAdCompleted -= AdCompletedHandler;
    }

    public void AdCompletedHandler()
    {
        MoveToLoading();
    }

    public void MoveToLoading()
    {
        GameStateManager.ApplicationManager.PlayGame();
        GameStateManager.ApplicationManager.OnSceneLoad.Raise();
        StartCoroutine(CountDown());
    }

    public void RestartScene()
    {
        GameStateManager.ApplicationManager.OnSceneLoad.Raise();
        StartCoroutine(CountDownToSame());
    }

    private IEnumerator CountDown()
    {
        while (countDownTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countDownTime--;
        }

        sceneFader.FadeTo(sceneName);
    }

    private IEnumerator CountDownToSame()
    {
        while (countDownTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            countDownTime--;
        }

        sceneFader.FadeToSame();
    }
}
