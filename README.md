## Getting Started
1. In your editor, **sign in** to unity with your account
2. In your unity editor: Got to **Edit** -> **Project Settings** -> **Services**
3. In **Services General Settings** select the **_organization_** and click on the **_create project ID_** button. If you get an ATTENTION warning, click on **_refresh access_**. Your screen should look like so:
   
![Screenshot (842)](https://github.com/justPoly/monetization_project/assets/29443625/9580a52e-ebd1-404c-be8c-7ccb5439c5be)

**Note:** Be sure to select **"N0"** as your response to the question: **Will this app be primarily targeted to children under age 13?**. Again, take another look at the screenshot above to identify this question.

4. Go to **windows** -> **package manager** -> select packages: **unity registry** [from top left corner]
     
   ![Screenshot (845)](https://github.com/justPoly/monetization_project/assets/29443625/19200e57-517a-4000-8c5e-1056543f1c01)

6. Search for **Advertisement Legacy** select and install.

## Unity Ads Packgage
6. Download and import the unity ads package to your project.
   click here to download
   
## Integrating Unity Ads into your Game
7. Go to your prefab folder and drag the UnityAdsManager prefab to the hierarchy section in unity editor.
8. If you are attaching it to a button, you can drag the UnityAdsManager prefab from the hierarchy to the "On Click" event and interact with the methods e.g  InterstitialAds.ShowInterstitialAds.
   
![Screenshot (846)](https://github.com/justPoly/monetization_project/assets/29443625/6fc77a76-80f1-41e0-b206-ab42d5f682ee)

9. If you choose to display the ads through a method in your code base, you can use AdManager to assess any of the ads in your custom methods. E.g AdManager.Instance.BannerAds.HideBannerAds();


