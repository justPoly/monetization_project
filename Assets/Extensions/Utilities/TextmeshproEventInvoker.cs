using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextmeshproEventInvoker : MonoBehaviour
{
    public bool m_notObject = false;

    [ShowIf("m_notObject")]
    public TextMeshProUGUI m_text;
    public string m_prefix;
    public string m_suffix;

    [Header("PlayerPrefs")]
    [ShowIf("isPlayerPrefs")] public string m_prefKey;
    [ShowIf("isPlayerPrefs")] public PlayerPrefsDataType playerPrefsDataType;
    private void Awake()
    {
        if (!m_notObject)
            m_text = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        OnEnable();
    }
    bool isPlayerPrefs() { return true; }
    private void OnEnable()
    {

        switch (playerPrefsDataType)
        {
            case PlayerPrefsDataType.FLOAT:
                Invoke(PlayerPrefs.GetFloat(m_prefKey));
                break;

            case PlayerPrefsDataType.STRING:
                InvokeStr(PlayerPrefs.GetString(m_prefKey));
                break;

            case PlayerPrefsDataType.INT:
                Invoke(PlayerPrefs.GetInt(m_prefKey));
                break;

        }


    }
    public void Invoke(int _i)
    {
        CheckTxtObj();
        m_text.text = m_prefix + _i.ToString() + m_suffix;
    }
    public void InvokeCountingUp(int _i)
    {
        CheckTxtObj();
        //m_text.text = m_prefix + _i.ToString() + m_suffix;
        UpdateText(_i);
        _previousValue = _i;
    }

    private Coroutine CountingCoroutine;
    [Header("Counting Animation")]
    public int CountFPS = 30;
    public float Duration = 1f;
    public string NumberFormat = "N0";
    private int _previousValue = 0;
    private void UpdateText(int newValue)
    {
        if (CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
        }

        CountingCoroutine = StartCoroutine(CountText(newValue));
    }
    private IEnumerator CountText(int newValue)
    {
        WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
        int previousValue = _previousValue;
        int stepAmount;

        if (newValue - previousValue < 0)
        {
            stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration)); // newValue = -20, previousValue = 0. CountFPS = 30, and Duration = 1; (-20- 0) / (30*1) // -0.66667 (ceiltoint)-> 0
        }
        else
        {
            stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration)); // newValue = 20, previousValue = 0. CountFPS = 30, and Duration = 1; (20- 0) / (30*1) // 0.66667 (floortoint)-> 0
        }

        if (previousValue < newValue)
        {
            while (previousValue < newValue)
            {
                previousValue += stepAmount;
                if (previousValue > newValue)
                {
                    previousValue = newValue;
                }

                m_text.SetText(m_prefix + previousValue.ToString(NumberFormat) + m_suffix);

                yield return Wait;
            }
        }
        else
        {
            while (previousValue > newValue)
            {
                previousValue += stepAmount; // (-20 - 0) / (30 * 1) = -0.66667 -> -1              0 + -1 = -1
                if (previousValue < newValue)
                {
                    previousValue = newValue;
                }

                m_text.SetText(m_prefix + previousValue.ToString(NumberFormat) + m_suffix);

                yield return Wait;
            }
        }
    }
    public void InvokeStr(string _str)
    {
        CheckTxtObj();
        m_text.text = m_prefix + _str.ToString() + m_suffix;
    }
    public void Invoke(float _f)
    {
        CheckTxtObj();
        m_text.text = m_prefix + _f.ToString() + m_suffix;
    }
    public void CheckTxtObj()
    {
        if (!m_text)
            m_text = GetComponent<TextMeshProUGUI>();
    }
    public enum PlayerPrefsDataType
    {
        FLOAT, STRING, INT, None
    }
    public enum Parameters
    {
        // Dont mind the number tags, I was trying out some weird experiments
        NONE = 0,
        MONEY = 1 << 0,
        LEVEL = 2 << 1,
        DAY = 3 << 2,
        SEASON = 4 << 3,
        PLAYERPREFS = 5 << 4,
        NEXT_LEVEL_MONEY = 6 << 5,
        NEXT_LEVEL = 7 << 6,
        LEVEL_PROGRESSION = 8 << 7,
        TIME = 9 << 8,
        TOTAL_EARNING = 10 << 9
        //OTHER = 5 << 4
    }
}
