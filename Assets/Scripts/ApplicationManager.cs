using UnityEngine;

[CreateAssetMenu(fileName = "Application", menuName = "Managers/Application Manager")]

public class ApplicationManager : ScriptableObject
{

    [FancyHeader("APPLICATION MANAGER", 3f, "#D4AF37", 8.5f, order = 0)]
    [Space]
    public GameEvent OnSceneLoad;


    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void PlayGame()
    {
        Time.timeScale = 1;
    }


}