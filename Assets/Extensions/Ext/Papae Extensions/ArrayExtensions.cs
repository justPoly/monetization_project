using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Papae.UnitySDK.Extensions
{
    public static class ArrayExtensions
    {
        public static Random RandomSeed
        {
            get { return new Random((int)DateTime.Now.Ticks); }
        }

        public static T[] ConvertTo<T>(this Array array)
        {
            T[] result = new T[array.Length];

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter.CanConvertFrom(array.GetValue(0).GetType()))
            {
                for (int i = 0; i < array.Length; i++)
                {
                    result[i] = (T)converter.ConvertFrom(array.GetValue(i));
                }
            }
            else
            {
                converter = TypeDescriptor.GetConverter(array.GetValue(0).GetType());
                if (converter.CanConvertTo(typeof(T)))
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        result[i] = (T)converter.ConvertTo(array.GetValue(i), typeof(T));
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            return result;
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <returns>The range.</returns>
        /// <param name="src">Source.</param>
        /// <param name="array">Array.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] AddRange<T>(this T[] src, T[] array)
        {
            List<T> list = src.ToList<T>();
            list.AddRange(array);
            return list.ToArray();
        }

        /// <summary>
        /// Contains the specified src and itm.
        /// </summary>
        /// <returns>The contains.</returns>
        /// <param name="src">Source.</param>
        /// <param name="itm">Itm.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool Contains<T>(this T[] src, T itm)
        {
            if (src != null)
            {
                for (int i = 0; i < src.Length; i++)
                {
                    if (src[i].Equals(itm))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Insert the specified src and val.
        /// </summary>
        /// <returns>The insert.</returns>
        /// <param name="src">Source.</param>
        /// <param name="val">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] Insert<T>(this T[] src, T val)
        {
            int index = src.Length - 1;
            //return src.InsertAfter(index, val);
            return src.Insert(index, val);
        }

        /// <summary>
        /// Insert the specified src, index and val.
        /// </summary>
        /// <returns>The insert.</returns>
        /// <param name="src">Source.</param>
        /// <param name="index">Index.</param>
        /// <param name="val">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] Insert<T>(this T[] src, int index, T val)
        {
            List<T> list = src.ToList();

            list.Insert(index, val);

            return list.ToArray();
        }

        /// <summary>
        /// Inserts the after.
        /// </summary>
        /// <returns>The after.</returns>
        /// <param name="src">Source.</param>
        /// <param name="index">Index.</param>
        /// <param name="val">Value.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] InsertAfter<T>(this T[] src, int index, T val)
        {
            if (index == src.Length - 1)
            {
                Array.Resize(ref src, src.Length + 1);
                src[src.Length - 1] = val;
            }
            else
            {
                T[] dest = new T[src.Length + 1];

                Array.Copy(src, dest, index + 1);

                dest[index + 1] = val;

                Array.Copy(src, index + 1, dest, index + 2, src.Length - (index + 1));

                src = dest;
            }

            return src;
        }

        /// <summary>
        /// Ises the null or empty.
        /// </summary>
        /// <returns><c>true</c>, if null or empty was ised, <c>false</c> otherwise.</returns>
        /// <param name="src">Source.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool IsNullOrEmpty<T>(this T[] src)
        {
            return src == null || src.Length == 0;
        }

        /// <summary>
        /// Sets all values.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array that will be modified.</typeparam>
        /// <param name="src">The one-dimensional, zero-based array</param>
        /// <param name="value">The value.</param>
        /// <returns>A reference to the changed array.</returns>
        public static T[] Populate<T>(this T[] src, T value, int amount)
        {
            if (src == null)
            {
                return null;
            }

            List<T> list = src.ToList();
            amount = Math.Abs(amount);
            int i = 0;
            if (amount <= src.Length)
            {
                for (i = 0; i < amount; i++)
                {
                    list[i] = value;
                }
            }
            else
            {
                for (i = 0; i < amount; i++)
                {
                    if (i < src.Length)
                        list[i] = value;
                    else
                        list.Add(value);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Sets all values.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array that will be modified.</typeparam>
        /// <param name="src">The one-dimensional, zero-based array</param>
        /// <param name="value">The value.</param>
        /// <returns>A reference to the changed array.</returns>
        public static T[] PopulateIfEmpty<T>(this T[] src, T value, int amount)
        {
            if (src == null)
            {
                return null;
            }

            List<T> list = src.ToList();
            amount = Math.Abs(amount);
            int i = 0;
            if (amount > src.Length)
            {
                for (i = 0; i < amount; i++)
                {
                    if (i < src.Length) continue;

                    list.Add(value);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Randoms the value.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="array">Array.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T RandomValue<T>(this T[] src)
        {
            if (src == null)
            {
                return default(T);
            }

            return src.RandomRangeValue(0, src.Length);
        }

        /// <summary>
        /// Randoms the value exclusive.
        /// </summary>
        /// <returns>The value exclusive.</returns>
        /// <param name="src">Source.</param>
        /// <param name="excl">Excl.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T RandomValueExclusive<T>(this T[] src, T[] excl)
        {
            if (src == null)
            {
                return default(T);
            }

            T item;
            do
            {
                item = src.RandomRangeValue(0, src.Length);
            } while (excl.Contains(item));

            return item;
        }

        /// <summary>
        /// Randoms the range value.
        /// </summary>
        /// <returns>The range value.</returns>
        /// <param name="src">Array.</param>
        /// <param name="min_index">Minimum index.</param>
        /// <param name="length">Length.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T RandomRangeValue<T>(this T[] src, int start_index, int length)
        {
            if (src == null || start_index + length >= src.Length)
            {
                new ArgumentOutOfRangeException(src.ToString(), "Random range value supplied is out of range or Array is null!");
            }

            //Random random = new Random((int)DateTime.Now.Ticks);
            //int randomIndex = UnityEngine.Random.Range(0, array.Length);
            int randomIndex = RandomSeed.Next(0, src.Length - 1);
            return src[randomIndex];
        }

        /// <summary>
        /// Removes at src and index.
        /// </summary>
        /// <returns>The <see cref=""/>.</returns>
        /// <param name="src">Source.</param>
        /// <param name="index">Index.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] RemoveAt<T>(this T[] src, int index)
        {
            T[] dest = new T[src.Length - 1];
            if (index > 0)
            {
                Array.Copy(src, 0, dest, 0, index);
            }

            if (index < src.Length - 1)
            {
                Array.Copy(src, index + 1, dest, index, src.Length - index - 1);
            }

            return dest;
        }

        /// <summary>
        /// Sets all values.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array that will be modified.</typeparam>
        /// <param name="src">The one-dimensional, zero-based array</param>
        /// <param name="value">The value.</param>
        /// <returns>A reference to the changed array.</returns>
        public static T[] SetValuesInRange<T>(this T[] src, int startIndex, int length, T repeatValue)
        {
            if (src == null || startIndex < 0 || startIndex + length >= src.Length)
            {
                new ArgumentOutOfRangeException(src.ToString(), "Random range value supplied is out of range or Array is null!");
            }

            List<T> values = new List<T>();
            for (int i = startIndex; i < length; i++)
            {
                values.Add(repeatValue);
            }

            return src.SetValuesInRange(startIndex, length, values.ToArray());
        }

        /// <summary>
        /// Sets all values.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the array that will be modified.</typeparam>
        /// <param name="src">The one-dimensional, zero-based array</param>
        /// <param name="value">The value.</param>
        /// <returns>A reference to the changed array.</returns>
        public static T[] SetValuesInRange<T>(this T[] src, int startIndex, int length, T[] values)
        {
            if (src == null || startIndex < 0 || startIndex + length >= src.Length || values.Length != length - startIndex)
            {
                new ArgumentOutOfRangeException(src.ToString(), "Random range value supplied is out of range or Array is null!");
            }

            for (int i = startIndex; i < startIndex + length; i++)
            {
                src[i] = values[i - startIndex];
            }

            return src;
        }

        /// <summary>
        /// Shuffle the specified array.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                //int index = UnityEngine.Random.Range(0, n);
                int index = RandomSeed.Next(0, n - 1);
                T value = array[index];
                array[index] = array[n];
                array[n] = value;
            }

            return array;
        }

        /// <summary>
        /// Tos the list.
        /// </summary>
        /// <returns>The list.</returns>
        /// <param name="src">Array.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> ToList<T>(this T[] src)
        {
            List<T> list = new List<T>();

            if (src != null)
            {
                for (int i = 0; i < src.Length; i++)
                {
                    list.Add(src[i]);
                }
            }

            return list;
        }

        /// <summary>
        /// Tos the string full.
        /// </summary>
        /// <returns>The string full.</returns>
        /// <param name="src">Array.</param>
        /// <param name="seperator">Seperator.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string ToStringFull<T>(this T[] src, char seperator = ',')
        {
            string result = string.Empty;
            for (int i = 0; i < src.Length; i++)
            {
                result += src[i].ToString();

                result += i == src.Length - 1 ? '\0' : seperator;
            }

            return result;
        }
        public static void MoveItemAtIndexToFront<T>(this List<T> list, int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            list.Insert(0, item);
        }
        /// <summary>
        /// Tos the string full.
        /// </summary>
        /// <returns>The string full.</returns>
        /// <param name="src">Source.</param>
        /// <param name="seperator">Seperator.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string ToStringFull<T>(this T[][] src, char seperator = ',')
        {
            string result = "{";

            if (src != null)
            {
                for (int i = 0; i < src.Length; i++)
                {
                    for (int j = 0; j < src[i].Length; j++)
                    {
                        result += j == 0 ? "[" : "";
                        result += src[i][j].ToString();
                        result += j == src[i].Length - 1 ? '\0' : seperator;
                        result += j == src[i].Length - 1 ? "]" : "";
                    }
                    result += "\n";
                }
            }

            return result += "}";
        }
    }
}

