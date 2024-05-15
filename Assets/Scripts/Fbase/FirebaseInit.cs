using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase;

public class FirebaseInit : MonoBehaviour
{
    private FirebaseApp app;

    // Use this for initialization
    void Start () {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                app = FirebaseApp.DefaultInstance;

                // Set a flag here for indicating that your project is ready to use Firebase.
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}",dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
    
    #region 
    public void ViewRewardedAdButton()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("View_RewardedAd_Button");
    }

    public void OptionButton(int number)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("Option_Button_Pressed", new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter("ButtonNumber", number),
            new Firebase.Analytics.Parameter("ButtonNumber", number),
        });
    }
    #endregion
}
