using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class CheckForInternet : MonoBehaviour
{
   [SerializeField] TextMeshProUGUI loadingText;
   [SerializeField] TextMeshProUGUI connectionErrorText;
   [SerializeField] Button tryAgainButton;
   [SerializeField] Button continueButton;

    void Start()
    {
        StartCoroutine(CheckInternetConnection());
    }

    IEnumerator CheckInternetConnection()
    {
        UnityWebRequest request = new UnityWebRequest("https://google.com");
        yield return request.SendWebRequest();

        if(request.error != null)
        {
            loadingText.gameObject.SetActive(false);
            connectionErrorText.gameObject.SetActive(true);
            tryAgainButton.gameObject.SetActive(true);
        }
        else 
        {
            loadingText.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);
        }
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Continue(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}
