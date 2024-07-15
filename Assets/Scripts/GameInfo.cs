using UnityEngine;

[CreateAssetMenu(fileName = "GameInfo", menuName = "ScriptableObjects/GameInfo", order = 1)]
public class GameInfo : ScriptableObject
{
    public Sprite gameImage;
    // public string deepLinkURL;        // Deep link URL to open the game
    public string androidStoreLink;   // Google Play Store link
    public string iOSStoreLink;       // App Store link
}
