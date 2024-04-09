using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenceAfterTime : MonoBehaviour
{
    [SerializeField]
    public float delayTime = 6f;
    public SceneFader sceneFader;
 
    private float timeElapsed;
   

    [SerializeField]
    private string nextSceneName;

    private void Awake()
    {
      
    }

    void Update()
    {
          timeElapsed += Time.deltaTime;
    }

    public void Load(string scene)
    {
            sceneFader.FadeTo(scene);
    }


  
}
