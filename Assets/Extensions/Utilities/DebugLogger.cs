using NaughtyAttributes;
using UnityEngine;

namespace PAGG
{

    public class DebugLogger : MonoBehaviour
    {
        [Label("Sentence Format")] public string m_sentence;
        public void DebugLogWithFormat(string _s)
        {
            Debug.Log(string.Format(m_sentence, _s));
        }

        public void DebugLog(string _s)
        {
            Debug.Log(_s);
        }
        public void DebugLogWithFormat(int _value)
        {
            Debug.Log(string.Format(m_sentence, _value.ToString()));
        }
    }
}