using System;
using System.ComponentModel;

namespace RhoTools
{
    public static class CEnumUtils
    {
        public static T[] GetValues<T>()
        {
            Array tArray = Enum.GetValues(typeof(T));
            T[] tList = new T[tArray.Length];
        
            for (int i = 0; i < tArray.Length; i++)
                tList[i] = (T)tArray.GetValue(i);
            return tList;
        }

        public static int GetLength<T>()
        {
            return Enum.GetNames(typeof(T)).Length;
        }

        public static T Parse<T>(string aVal)
        {
            return (T)Enum.Parse(typeof(T), aVal, true);
        }

        public static T Parse<T>(string aVal, T aDefault)
        {
            if (string.IsNullOrEmpty(aVal))
                return aDefault;

            T tReturn;
            TryParse<T>(aVal, out tReturn, aDefault);
            return tReturn;
        }

        public static bool TryParse<T>(string aVal, out T aReturn, T aDefault)
        {
            aReturn = aDefault;
            if (Enum.IsDefined(typeof(T), aVal))
            {
                aReturn = Parse<T>(aVal);
                return true;
            }
            return false;
        }

        public static bool TryParse<T>(string aVal, out T aReturn)
        {
            return TryParse<T>(aVal, out aReturn, default(T));
        }
    }
}
