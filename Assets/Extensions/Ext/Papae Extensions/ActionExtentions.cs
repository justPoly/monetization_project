using System;

namespace Papae.UnitySDK.Extensions
{
    public static class ActionExtentions
    {
        /// <summary>
        /// Methods the name.
        /// </summary>
        /// <returns>The name.</returns>
        /// <param name="action">Action.</param>
        public static string MethodName(this Action action)
        {
            if (action != null)
            {
                return action.Method.Name;
            }

            return "null";
        }

        /// <summary>
        /// Tries the invoke.
        /// </summary>
        /// <param name="action">Action.</param>
        public static void TryInvoke(this Action action)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Tries the invoke.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="parameter">Parameter.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void TryInvoke<T>(this Action<T> action, T parameter)
        {
            if (action != null)
            {
                action.Invoke(parameter);
            }
        }

        /// <summary>
        /// Tries the invoke.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="param1">Param1.</param>
        /// <param name="param2">Param2.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <typeparam name="K">The 2nd type parameter.</typeparam>
        public static void TryInvoke<T, K>(this Action<T, K> action, T param1, K param2)
        {
            if (action != null)
            {
                action.Invoke(param1, param2);
            }
        }
    }
}

