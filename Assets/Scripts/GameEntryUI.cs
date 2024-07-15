using UnityEngine;
using UnityEngine.UI;

public class GameEntryUI : MonoBehaviour
{
    public Image gameImage;
    public Button playButton;

    private string androidStoreLink;
    private string iOSStoreLink;

    public void Setup(GameInfo gameInfo)
    {
        gameImage.sprite = gameInfo.gameImage;
        androidStoreLink = gameInfo.androidStoreLink;
        iOSStoreLink = gameInfo.iOSStoreLink;

        playButton.onClick.AddListener(OpenStore);
    }

    private void OpenStore()
    {
#if UNITY_ANDROID
        Application.OpenURL(androidStoreLink);
#elif UNITY_IOS
        Application.OpenURL(iOSStoreLink);
#endif
    }
}
