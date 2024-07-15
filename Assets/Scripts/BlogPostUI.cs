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

        // Ensure the title is updated immediately
        titleText.ForceMeshUpdate();

        // Start the coroutine to load the image if the GameObject is active
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(LoadImage(imageUrl));
        }
    }

    void OnEnable()
    {
        // Ensure the image is loaded when the GameObject becomes active
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

        // Adjust the aspect ratio of the image if needed
        RectTransform rt = image.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.x * texture.height / texture.width);
    }

    public void OnClick()
    {
        Debug.Log("Opening URL: " + url);

        // Use a coroutine to handle opening the URL and then restoring the UI
        StartCoroutine(OpenURLAndRestore());
    }

    IEnumerator OpenURLAndRestore()
    {
        // Open URL
        Application.OpenURL(url);

        // Wait for a short delay to ensure the URL is opened
        yield return new WaitForSeconds(0.1f);

        // Reload the image to ensure it is still visible
        if (!string.IsNullOrEmpty(imageUrl))
        {
            StartCoroutine(LoadImage(imageUrl));
        }
    }
}
