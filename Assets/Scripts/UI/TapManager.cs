using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject newsPanel;
    public GameObject moreGamesPanel;

    [Header("Buttons")]
    public Button settingsButton;
    public Button newsButton;
    public Button moreGamesButton;

    void Start()
    {
        // Set the OnClick event for each button
        settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel));
        newsButton.onClick.AddListener(() => ShowPanel(newsPanel));
        moreGamesButton.onClick.AddListener(() => ShowPanel(moreGamesPanel));

        // Show settings panel by default
        ShowPanel(settingsPanel);
    }

    void ShowPanel(GameObject panelToShow)
    {
        // Hide all panels
        settingsPanel.SetActive(false);
        newsPanel.SetActive(false);
        moreGamesPanel.SetActive(false);

        // Show the selected panel
        panelToShow.SetActive(true);
    }
}
