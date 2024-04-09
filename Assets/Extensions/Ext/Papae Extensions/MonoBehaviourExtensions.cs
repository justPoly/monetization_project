using System;
using System.Linq.Expressions;
using UnityEngine;

namespace Papae.UnitySDK.Extensions
{
	/// <summary>
	/// http://forum.unity3d.com/threads/change-the-value-of-a-toggle-without-triggering-onvaluechanged.275056/#post-2348336
	///
	/// Problem:
	///     When setting a Unity UI Toggle field isOn, it automatically fires the onchanged event.
	///
	/// This class allows you to set the Toggle, Slider, Scrollbar and Dropdown's value without invoking the onchanged event.
	/// It mostly does this by invoking the private method ('Set(value, sendCallback)') contained in some of the Unity UI elements
	/// </summary>
    public static class MonoBehaviourExtensions
	{
        #region Properties

        private static bool isPaused = false;
        private static float timeScale = 1f;

        #endregion

        public static void CancelInvoke(this MonoBehaviour monoBehaviour, Expression<Action> expr)
        {
            monoBehaviour.CancelInvoke(GetMethodName(expr));
        }

        private static string GetMethodName(Expression<Action> expr)
        {
            return ((MethodCallExpression)expr.Body).Method.Name;
        }

        public static void Invoke(this MonoBehaviour monoBehaviour, Expression<Action> expr, float time)
        {
            monoBehaviour.Invoke(GetMethodName(expr), time);
        }

        public static void InvokeRepeating(this MonoBehaviour monoBehaviour, Expression<Action> expr, float time, float repeatRate)
        {
            monoBehaviour.InvokeRepeating(GetMethodName(expr), time, repeatRate);
        }

        public static void IsNull(this MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour == null)
            {
                Debug.Log(monoBehaviour.name + " is null.");
            }
        }

        public static bool IsInvoking(this MonoBehaviour monoBehaviour, Expression<Action> expr)
        {
            return monoBehaviour.IsInvoking(GetMethodName(expr));
        }

        public static void PauseUnity(this MonoBehaviour monoBehaviour)
        {
            // Pause only if its not in paused state
            if (!isPaused)
            {
                Debug.LogWarning("[MonoBehaviourExtensions] Paused");
                isPaused = true;

                // Cache timescale, later used for resetting
                timeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
        }

        public static void ResumeUnity(this MonoBehaviour monoBehaviour)
        {
            // Resume only if paused
            if (isPaused)
            {
                Debug.LogWarning("[MonoBehaviourExtensions] Resumed");
                isPaused = false;

                // Reset timescale
                Time.timeScale = timeScale;
            }
        }

        public static void SetActive(this MonoBehaviour monoBehaviour, bool flag)
        {
            // Resume only if paused
            if (monoBehaviour != null)
            {
                if (UnityEngine.Application.isPlaying && monoBehaviour.gameObject != null)
                {
                    monoBehaviour.gameObject.SetActive(flag);
                }
            }
        }
	}
}