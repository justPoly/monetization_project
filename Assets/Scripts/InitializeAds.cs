using UnityEngine;
using UnityEngine.Advertisements;


public class InitializeAds : MonoBehaviour ,IUnityAdsInitializationListener
{
    [SerializeField] private string androidGameId;
    [SerializeField] private string iosGameId;
    [SerializeField] private bool isTesting;

    private string gameId;


    void Awake()
    {
        InitializeUnityAds();
    }

    public void InitializeUnityAds()
    {
    #if UNITY_IOS
            gameId = iosGameId;
    #elif UNITY_ANDROID
            gameId = androidGameId;
    #elif UNITY_EDITOR
            gameId = androidGameId; //Only for testing the functionality in the Editor
    #endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, isTesting, this);
        }
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Ads Initialized...");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)    
    {    
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
