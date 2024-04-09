using UnityEngine;
using UnityEngine.Events;

public class PlayerPrefsEvents : MonoBehaviour
{
    public string key = "";
    public UnityEvent<int> intEvent;
    public void CheckIntKey()
    {
        intEvent?.Invoke(PlayerPrefs.GetInt(key));
    }
}
