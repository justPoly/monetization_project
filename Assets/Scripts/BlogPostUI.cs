using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class BlogPostUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private RawImage image;

    private string url;
    private string imageUrl;

    public void SetPost(string title, string imageUrl, string postUrl)
    {
        titleText.text = title;
        this.url = postUrl;
        this.imageUrl = imageUrl;
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(LoadImage(imageUrl));
        }
    }

    void OnEnable()
    {
        if (!string.IsNullOrEmpty(imageUrl))
        {
            StartCoroutine(LoadImage(imageUrl));
        }
    }

    IEnumerator LoadImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load image: " + www.error);
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(www);
        image.texture = texture;
    }

    public void OnClick()
    {
        Application.OpenURL(url);
    }
}
