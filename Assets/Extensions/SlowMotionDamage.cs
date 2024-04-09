using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionDamage : MonoBehaviour
{
    [SerializeField]
    float timeToReset;
    public void SlowMotion() 
    {
        StartCoroutine(SlowDownTime());
    }

    IEnumerator SlowDownTime()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(timeToReset);
        Time.timeScale = 1f;
    }
}
