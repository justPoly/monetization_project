using UnityEngine;
using UnityEngine.UI;

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
        // panel.SetActive(isPanelActive);

        // Add a listener to the button click event
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        if (isPanelActive = PlayerPrefs.GetInt(PanelStateKey, 0) == 0)
        {
        // Hide the panel
            panel.SetActive(true);
        } else {
                panel.SetActive(false);
                sceneToLoad.MoveToLoading();
        }
    }

    public void OnUserAgree()
    {
        // Hide the panel
        panel.SetActive(false);

        // Save the state to PlayerPrefs
        PlayerPrefs.SetInt(PanelStateKey, 1);
        PlayerPrefs.Save();
        sceneToLoad.MoveToLoading();
    }


}
