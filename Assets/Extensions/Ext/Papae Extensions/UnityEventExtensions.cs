using System;
using System.Reflection;
using UnityEngine.Events;

namespace Papae.UnitySDK.Extensions
{
    public static class UnityEventExtensions
    {
        public static int GetListenerNumber(this UnityEventBase unityEvent)
        {
            var field = typeof(UnityEventBase).GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            var invokeCallList = field.GetValue(unityEvent);
            var property = invokeCallList.GetType().GetProperty("Count");
            return (int)property.GetValue(invokeCallList);
        }
        /// <summary>
        /// Adds a listner to a UnityEvent which is removed as soon as the event is invoked
        /// </summary>
        /// <param name="unityEvent">the event to listen for</param>
        /// <param name="action">the callback to call</param>
        /// <returns></returns>
        public static UnityEvent Add(this UnityEvent unityEvent, Action action)
        {
            UnityAction unityAction = null;
            unityAction = () =>
            {
                if (action != null)
                {
                    action.Invoke();
                }
            };

            unityEvent.AddListener(unityAction);
            return unityEvent;
        }

        /// <summary>
        /// Adds a listner to a UnityEvent which is removed as soon as the event is invoked
        /// </summary>
        /// <param name="unityEvent">the event to listen for</param>
        /// <param name="action">the callback to call</param>
        /// <returns></returns>
        public static UnityEvent<T> Add<T>(this UnityEvent<T> unityEvent, Action<T> action)
        {
            UnityAction<T> unityAction = null;
            unityAction = obj =>
            {
                if (action != null)
                {
                    action(obj);
                }
            };

            unityEvent.AddListener(unityAction);
            return unityEvent;
        }

        /// <summary>
        /// Adds a listner to a UnityEvent which is removed as soon as the event is invoked
        /// </summary>
        /// <param name="unityEvent">the event to listen for</param>
        /// <param name="action">the callback to call</param>
        /// <returns></returns>
        public static UnityEvent<T> Add<T>(this UnityEvent<T> unityEvent, Action<T> action, T value)
        {
            UnityAction<T> unityAction = null;
            unityAction = obj =>
            {
                if (action != null)
                {
                    action(value);
                }
            };

            unityEvent.AddListener(unityAction);
            return unityEvent;
        }

        /// <summary>
        /// Adds a listner to a UnityEvent which is removed as soon as the event is invoked
        /// </summary>
        /// <param name="unityEvent">the event to listen for</param>
        /// <param name="action">the callback to call</param>
        /// <returns></returns>
        public static UnityEvent AddOnce(this UnityEvent unityEvent, Action action)
        {
            UnityAction unityAction = null;
            unityAction = () =>
            {
                unityEvent.RemoveListener(unityAction);
                if (action != null)
                {
                    action.Invoke();
                }
            };

            unityEvent.AddListener(unityAction);
            return unityEvent;
        }

        /// <summary>
        /// Adds a listner to a UnityEvent which is removed as soon as the event is invoked
        /// </summary>
        /// <param name="unityEvent">the event to listen for</param>
        /// <param name="action">the callback to call</param>
        /// <returns></returns>
        public static UnityEvent<T> AddOnce<T>(this UnityEvent<T> unityEvent, Action<T> action)
        {
            UnityAction<T> unityAction = null;
            unityAction = obj =>
            {
                unityEvent.RemoveListener(unityAction);
                if (action != null)
                {
                    action(obj);
                }
            };

            unityEvent.AddListener(unityAction);
            return unityEvent;
        }

        /// <summary>
        /// Adds a listner to a UnityEvent which is removed as soon as the event is invoked
        /// </summary>
        /// <param name="unityEvent">the event to listen for</param>
        /// <param name="action">the callback to call</param>
        /// <returns></returns>
        public static UnityEvent<T> AddOnce<T>(this UnityEvent<T> unityEvent, Action<T> action, T value)
        {
            UnityAction<T> unityAction = null;
            unityAction = obj =>
            {
                unityEvent.RemoveListener(unityAction);
                if (action != null)
                {
                    action(value);
                }
            };

            unityEvent.AddListener(unityAction);
            return unityEvent;
        }
    }
}
