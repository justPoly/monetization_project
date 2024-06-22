using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class TermsPanel : MonoBehaviour
{
    public GameObject panel; // The panel to hide
    public Button button; // The button to trigger the hide action
    public SceneToLoad sceneToLoad; // Reference to the SceneToLoad script

    private const string PanelStateKey = "PanelState";
    private bool isPanelActive;

    void Start()
    {
        // Load the panel state from PlayerPrefs
        bool isPanelActive = PlayerPrefs.GetInt(PanelStateKey, 0) == 0;
        panel.SetActive(isPanelActive);

        // Add a listener to the button click event
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        Debug.Log("Button Clicked");
        if (isPanelActive = PlayerPrefs.GetInt(PanelStateKey, 0) == 0)
        {
            panel.SetActive(true);
        } else {
            panel.SetActive(false);
            sceneToLoad.MoveToNext();
        }
    }

    public void OnUserAgree()
    {
        // Hide the panel
        panel.SetActive(false);

        // Save the state to PlayerPrefs
        PlayerPrefs.SetInt(PanelStateKey, 1);
        PlayerPrefs.Save();
        // sceneToLoad.MoveToNext();
    }


}
