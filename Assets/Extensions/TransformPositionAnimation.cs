using System.Collections;
using UnityEngine;

public class TransformPositionAnimation : MonoBehaviour
{
    public bool realtime = false;
    public bool pingPong = false;
    public float pulsePerSecond;
    public Vector3 startPosition;
    public Vector3 endPosition = Vector3.one;

    float currentPulseTime = 0;
    bool started = false;

    Vector3 defaultPosition;
    float lastRealtime;

    void OnEnable()
    {
        if (started)
        {
            currentPulseTime = 0;
            transform.localPosition = startPosition;
            lastRealtime = Time.realtimeSinceStartup;
        }
    }

    void OnDisable()
    {
        if (started)
        {
            transform.localPosition = defaultPosition;
        }
    }

    void Awake()
    {
        defaultPosition = transform.localPosition;
    }

    // Use this for initialization
    void Start()
    {
        started = true;
        transform.localPosition = startPosition;
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

        transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
    }
}
