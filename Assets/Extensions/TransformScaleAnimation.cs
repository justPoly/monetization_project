using System.Collections;
using UnityEngine;

public class TransformScaleAnimation : MonoBehaviour
{
    public bool realtime = false;
    public bool pingPong = false;
    public float pulsePerSecond;
    public Vector3 startScale;
    public Vector3 endScale = Vector3.one;

    float currentPulseTime = 0;
    bool started = false;

    Vector3 defaultScale;
    float lastRealtime;

    void OnEnable()
    {
        if (started)
        {
            currentPulseTime = 0;
            transform.localScale = startScale;
            lastRealtime = Time.realtimeSinceStartup;
        }
    }

    void OnDisable()
    {
        if (started)
        {
            transform.localScale = defaultScale;
        }
    }

    void Awake()
    {
        defaultScale = transform.localScale;
    }

    // Use this for initialization
    void Start()
    {
        started = true;
        transform.localScale = startScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (realtime)
        {
            currentPulseTime += Time.realtimeSinceStartup - lastRealtime;
            lastRealtime = Time.realtimeSinceStartup;
        }
        else
        {
            currentPulseTime += Time.deltaTime;
        }

        float t = currentPulseTime * pulsePerSecond;
        if (pingPong)
        {
            t = Mathf.Sin(Mathf.PI * t);
            t *= t;
        }
        else
        {
            t = Mathf.Repeat(t, 1f);
        }

        transform.localScale = Vector3.Lerp(startScale, endScale, t);
    }
}
