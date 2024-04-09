using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelToLoad = "MainLevel";

    public GameObject quitMenu;
   // public SceneFader sceneFader;

  //  public CurrencySO playerCoins;
   
    // public SceneFader sceneFader;



    private void Awake()
    {
       // playerCoins.CurrencyInitializer();
       
    }
    public void PlayGame()
    {
       // Time.timeScale = 1;
     //   sceneFader.FadeTo(levelToLoad);
        //  menuSound.clip.
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
    }
    public void QuitMenu()
    {
        quitMenu.gameObject.SetActive(true);
    }
    public void CloseQuitMenu()
    {
        quitMenu.gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game!!");
        Application.Quit();
    }
}
