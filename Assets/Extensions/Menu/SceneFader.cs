using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{

    public Image img;
    public AnimationCurve curve;
    public  bool isSceneFaded;

    public static SceneFader instance;

    private void Awake()
    {
        instance = this;
        

    }
    void Start()
    {
        
        Debug.Log(isSceneFaded);
        StartCoroutine(FadeIn());
        
    }

    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }
    public void FadeToSame()
    {
        StartCoroutine(FadeOutSame());
    }

    IEnumerator FadeIn()
    {
        
        
        float t = 1f;
        
        while (t > 0f)
        {
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            isSceneFaded = false;
            yield return 0;

        }
    }

    IEnumerator FadeOut(string scene)
    {
        isSceneFaded = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            isSceneFaded = true;
            isSceneFaded = false;
            yield return 0;

        }

        SceneManager.LoadScene(scene);
    }


    IEnumerator FadeOutSame()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return 0;

        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
