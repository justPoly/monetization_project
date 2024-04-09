using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Papae.UnitySDK.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T value)
        {
            foreach (T t in enumerable)
            {
                yield return t;
            }

            yield return value;
        }

        public static IEnumerable<T> AppendIfObsolete<T>(this IEnumerable<T> enumerable, T value)
        {
            if (!enumerable.Contains(value))
            {
                enumerable.Append(value);
            }

            return enumerable;
        }

        /// <summary>
        ///   Checks whether a sequence contains all elements of another one.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the sequence to check.</typeparam>
        /// <param name="first">Containing sequence.</param>
        /// <param name="second">Contained sequence.</param>
        /// <returns>
        ///   <c>true</c>, if the sequence contains all elements of the other one, and
        ///   <c>false</c> otherwise.
        /// </returns>
        public static bool ContainsAll<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            return second.All(first.Contains);
        }

        // LINQ already has a method called "Contains" that does the same thing as this
        // BUT it fails to work with Mono 3.5 in some cases.
        // For example the following prints False, True in Mono 3.5 instead of True, True like it should:
        //
        // IEnumerable<string> args = new string[]
        // {
        //     "",
        //     null,
        // };
        // Log.Info(args.ContainsItem(null));
        // Log.Info(args.Where(x => x == null).Any());
        public static bool ContainsItem<T>(this IEnumerable<T> list, T value)
        {
            // Use object.Equals to support null values
            return list.Where(x => object.Equals(x, value)).Any();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.DistinctBy(keySelector, null);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (keySelector == null)
                throw new ArgumentNullException("keySelector");
            return DistinctByImpl(source, keySelector, comparer);
        }

        static IEnumerable<TSource> DistinctByImpl<TSource, TKey>(IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var knownKeys = new HashSet<TKey>(comparer);
            foreach (var element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> first, Action<T> action)
        {
            foreach (T t in first)
            {
                action(t);
            }
        }

        public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> list)
        {
            return list.GroupBy(x => x).Where(x => x.Skip(1).Any()).Select(x => x.Key);
        }

        public static string GetKey<T>(this IDictionary dictionary, T value)
        {
            string key = null;

            if (value != null)
            {
                foreach (KeyValuePair<string, T> eachPair in dictionary.Keys)
                {
                    if (eachPair.Value != null && eachPair.Value.Equals(value))
                    {
                        key = eachPair.Key;
                        break;
                    }
                }
            }

            return key;
        }

        public static T GetSingle<T>(this object[] objectArray, bool required)
        {
            if (required)
            {
                return objectArray.Where(x => x is T).Cast<T>().Single();
            }
            else
            {
                return objectArray.Where(x => x is T).Cast<T>().SingleOrDefault();
            }
        }

        /// <summary>
        ///   Tries to get the value with the specified key from the
        ///   dictionary, and returns the passed default value if the key
        ///   could not be found.
        /// </summary>
        /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
        /// <param name="dictionary">
        ///   Dictionary to get the value from.
        /// </param>
        /// <param name="key">
        ///   Key of the value to get.
        /// </param>
        /// <param name="defaultValue">
        ///   Default value to return if the specified key could not be found.
        /// </param>
        /// <returns>
        ///   Value with the specified <paramref name="key" />, if found,
        ///   and <paramref name="defaultValue" /> otherwise.
        /// </returns>
        public static TValue GetValueOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        /// <summary>
        ///   Tries to get the value with the specified key from the
        ///   dictionary, and returns the default value of the key type
        ///   if the key could not be found.
        /// </summary>
        /// <typeparam name="TKey">Type of the dictionary keys.</typeparam>
        /// <typeparam name="TValue">Type of the dictionary values.</typeparam>
        /// <param name="dictionary">
        ///   Dictionary to get the value from.
        /// </param>
        /// <param name="key">
        ///   Key of the value to get.
        /// </param>
        /// <returns>
        ///   Value with the specified <paramref name="key" />, if found,
        ///   and the default value of <typeparamref name="TValue" /> otherwise.
        /// </returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : default(TValue);
        }

        // These are more efficient than Count() in cases where the size of the collection is not known
        public static bool HasAtLeast<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.Take(amount).Count() == amount;
        }

        public static bool HasAtMost<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.Take(amount + 1).Count() <= amount;
        }

        public static bool HasLessThan<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.HasAtMost(amount - 1);
        }

        public static bool HasMoreThan<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.HasAtLeast(amount + 1);
        }

        /// <summary>
        ///   Returns the zero-based index of the first occurrence of the specified item in a sequence.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the sequence.</typeparam>
        /// <param name="items">Sequence to search.</param>
        /// <param name="item">Item to search for.</param>
        /// <returns>
        ///   Index of the specified item, if it could be found,
        ///   and <c>-1</c> otherwise.
        /// </returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            var index = 0;

            foreach (var i in items)
            {
                if (Equals(i, item))
                {
                    return index;
                }

                ++index;
            }

            return -1;
        }

        /// <summary>
        ///   Returns the zero-based index of the first  item in a sequence that satisfies a condition.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the sequence.</typeparam>
        /// <param name="items">Sequence to search.</param>
        /// <param name="predicate">Function to test each element for a condition..</param>
        /// <returns>
        ///   Index of the first item satisfying the condition, if any could be found,
        ///   and <c>-1</c> otherwise.
        /// </returns>
        public static int IndexOf<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            var index = 0;

            foreach (var i in items)
            {
                if (predicate(i))
                {
                    return index;
                }

                ++index;
            }

            return -1;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        // This is more efficient than just Count() < x because it will end early
        // rather than iterating over the entire collection
        public static bool IsLength<T>(this IEnumerable<T> enumerable, int amount)
        {
            return enumerable.Take(amount + 1).Count() == amount;
        }

        /// <summary>
        ///   Determines whether a sequence is null or doesn't contain any elements.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the sequence to check.</typeparam>
        /// <param name="sequence">Sequence to check. </param>
        /// <returns>
        ///   <c>true</c> if the sequence is null or empty, and
        ///   <c>false</c> otherwise.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                return true;
            }

            return !sequence.Any();
        }

        // Return the first item when the list is of length one and otherwise returns default
        public static TSource OnlyOrDefault<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var results = source.Take(2).ToArray();
            return results.Length == 1 ? results[0] : default(TSource);
        }


        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> enumerable, T value)
        {
            yield return value;

            foreach (T t in enumerable)
            {
                yield return t;
            }
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            foreach (T t in second)
            {
                yield return t;
            }

            foreach (T t in first)
            {
                yield return t;
            }
        }
        /// <summary>
        /// 0-9 A-Z a-z (Length of roomName === 6; up from 4). 
        /// https://answers.unity.com/questions/241219/random-code-generation.html
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomAlphaNumericStr(int desiredLength)
        {
            StringBuilder codeSB = new StringBuilder(""); // Requires @ top: using System.Text;
            //abcdefghijklmnopqrstuvwxyz
            char[] chars = "ABCDEFGHJKLMNPQRSTUVWXYZ123456789".ToCharArray();
            char singleChar;

            while (codeSB.Length < desiredLength)
            {
                singleChar = chars[UnityEngine.Random.Range(0, chars.Length)];
                codeSB.Append(singleChar);
            }

            Debug.Log("GenerateRandomAlphaNumericStr: " + codeSB.ToString());


            return codeSB.ToString();
        }

        /// <summary>
        /// Randomly selects one elements from the enumerable
        /// </summary>
        /// <typeparam name="T">The type of the item</typeparam>
        /// <param name="items">The items to random from</param>
        /// <returns></returns>
        public static T RandomElement<T>(this IEnumerable<T> items)
        {
            return items.ElementAt(items.RandomIndex());
        }

        /// <summary>
        /// Randoms selects one index from the enumerable
        /// </summary>
        /// <returns>The index.</returns>
        /// <param name="items">Items.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static int RandomIndex<T>(this IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentException("Cannot randomly pick an item from the list, the list is null!");
            if (items.Count() == 0) throw new ArgumentException("Cannot randomly pick an item from the list, there are no items in the list!");
            return UnityEngine.Random.Range(0, items.Count());
        }

        /*
        /// <summary>
        /// Return sub-list of random items from origin list without repeating.
        /// </summary>
        public static int RandomIndexNoRepeats<T>(this IEnumerable<T> items)
        {
            int index = -1;
            List<int> remainedIndexes = Enumerable.Range(0, items.Count()).ToList();
            Dictionary<int, T> dictionary = new Dictionary<int, T>();

            //while (remainedIndexes.Count < items.Count())
            { 
                //int selectedIndex = remainedIndexes.RandomIndex();

            }

            while (dictionary.Count < items.Count())
            {
                int selectedIndex = items.RandomIndex();

                if (dictionary.ContainsKey(selectedIndex))
                {

                }
                else
                { 
                
                }

                remainedIndexes.Remove(selectedIndex);
                items.Add(list[selectedIndex]);
            }

            return index;
        }
        */
        public static void Randomise<T>(this IEnumerable<T> list)
        {
            Random random = new Random();

            for (int i = 0; i < list.Count(); i++)
            {
                int index = random.Next(0, list.Count());
                list.Swap(list.ElementAt(i), list.ElementAt(index));
            }
        }

        /// <summary>
        /// Return sub-list of random items from origin list without repeating.
        /// </summary>
        public static List<T> RandomiseNoRepeats<T>(List<T> list, int count)
        {
            List<T> items = new List<T>();
            List<int> remainedIndexes = Enumerable.Range(0, list.Count).ToList();

            for (int i = 0; i < count; i++)
            {
                int selectedIndex = remainedIndexes.RandomIndex();
                remainedIndexes.Remove(selectedIndex);
                items.Add(list[selectedIndex]);
            }

            return items;
        }

        public static void RandomiseUsingFisherYates<T>(this IEnumerable<T> list)
        {
            Random random = new Random();

            // Using Fisher Yates Shuffle Algorithm
            int randomIndex;
            int n = list.Count();

            while (n > 1)
            {
                n--;
                randomIndex = random.Next(0, n);
                list.Swap(list.ElementAt(randomIndex), list.ElementAt(n));
            }
        }

        /// <summary>
        /// Borrowed from http://stackoverflow.com/questions/56692/random-weighted-choice
        /// Randomly selects one element from the enumerable, taking into account a weight
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="weightSelector"></param>
        /// <returns></returns>
        public static T RandomWeightedElement<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = UnityEngine.Random.value * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;

            }

            return default(T);
        }

        public static int RemoveAll<T>(this LinkedList<T> list, Func<T, bool> predicate)
        {
            int numRemoved = 0;

            var currentNode = list.First;
            while (currentNode != null)
            {
                if (predicate(currentNode.Value))
                {
                    var toRemove = currentNode;
                    currentNode = currentNode.Next;
                    list.Remove(toRemove);
                    numRemoved++;
                }
                else
                {
                    currentNode = currentNode.Next;
                }
            }

            return numRemoved;
        }

        /// <summary>
        /// Removes the first element in the collection / list.
        /// </summary>
        /// <returns><c>true</c>, if last was removed, <c>false</c> otherwise.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool RemoveFirst<T>(this IList<T> list)
        {
            return list.Remove(list.First());
        }

        /// <summary>
        /// Removes the last element in the collection / list.
        /// </summary>
        /// <returns><c>true</c>, if last was removed, <c>false</c> otherwise.</returns>
        /// <param name="list">List.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool RemoveLast<T>(this IList<T> list)
        {
            return list.Remove(list.Last());
        }

        public static IEnumerable<T> ReplaceOrAppend<T>(
            this IEnumerable<T> enumerable, Predicate<T> match, T replacement)
        {
            bool replaced = false;

            foreach (T t in enumerable)
            {
                if (match(t))
                {
                    replaced = true;
                    yield return replacement;
                }
                else
                {
                    yield return t;
                }
            }

            if (!replaced)
            {
                yield return replacement;
            }
        }

        // Another name for IEnumerable.Reverse()
        // This is useful to distinguish betweeh List.Reverse() when dealing with a list
        public static IEnumerable<T> Reversed<T>(this IEnumerable<T> list)
        {
            return list.Reverse();
        }

        public static T Second<T>(this IEnumerable<T> list)
        {
            return list.Skip(1).First();
        }

        public static T SecondOrDefault<T>(this IEnumerable<T> list)
        {
            return list.Skip(1).FirstOrDefault();
        }

        /// <summary>
        /// Shifts an item from an index to another, without modifying the list except than by moving elements around
        /// </summary>
        public static void Shift<T>(this IList<T> list, int fromIndex, int toIndex)
        {
            if (toIndex == fromIndex) return;
            int index = fromIndex;
            T shifted = list[fromIndex];
            while (index > toIndex)
            {
                index--;
                list[index + 1] = list[index];
                list[index] = shifted;
            }
            while (index < toIndex)
            {
                index++;
                list[index - 1] = list[index];
                list[index] = shifted;
            }
        }

        /// <summary>
        ///   Returns a comma-separated string that represents a sequence.
        /// </summary>
        /// <param name="sequence">Sequence to get a textual representation of.</param>
        /// <returns>Comma-separated string that represents the sequence.</returns>
        public static string SequenceToString(this IEnumerable sequence)
        {
            // Check empty sequence.
            if (sequence == null)
            {
                return "null";
            }

            var stringBuilder = new StringBuilder();

            // Add opening bracket.
            stringBuilder.Append("[");

            foreach (var element in sequence)
            {
                var elementString = element as string;
                if (elementString == null)
                {
                    // Handle nested enumerables.
                    var elementEnumerable = element as IEnumerable;
                    elementString = elementEnumerable != null
                        ? elementEnumerable.SequenceToString()
                        : element.ToString();
                }

                // Add comma.
                stringBuilder.AppendFormat("{0},", elementString);
            }

            // Empty sequence.
            if (stringBuilder.Length <= 1)
            {
                return "[]";
            }

            // Add closing bracket.
            stringBuilder[stringBuilder.Length - 1] = ']';
            return stringBuilder.ToString();
        }

        // Inclusive because it includes the item that meets the predicate
        public static IEnumerable<TSource> TakeUntilInclusive<TSource>(
            this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (var item in source)
            {
                yield return item;
                if (predicate(item))
                {
                    yield break;
                }
            }
        }

        public static object[] ToArray(this ICollection _collection)
        {
            if (_collection == null)
                return null;

            IEnumerator _enumerator = _collection.GetEnumerator();
            int _count = _collection.Count;
            object[] _objectArray = new object[_count];
            int _iter = 0;

            while (_enumerator.MoveNext())
                _objectArray[_iter++] = _enumerator.Current;

            return _objectArray;
        }

        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return (T)enumerator.Current;
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        public static IEnumerable<T> Swap<T>(this IEnumerable<T> list, T item1, T item2)
        {
            if (list.Contains(item1) && list.Contains(item2))
            {
                int index1 = list.IndexOf(item1);
                int index2 = list.IndexOf(item2);

                return list.Swap(index1, index2);
            }

            return list;
        }

        public static IEnumerable<T> Swap<T>(this IEnumerable<T> list, int index1, int index2)
        {
            if (index1 != index2 && index1 < list.Count() && index2 < list.Count())
            {
                //IEnumerable<T> copy = list.Where(x => false);

                for (int index = 0; index < list.Count(); index++)
                {
                    if (index == index1)
                    {
                        yield return list.ElementAt(index2);
                        //copy.Append(list.ElementAt(index2));
                    }
                    else if (index == index2)
                    {
                        yield return list.ElementAt(index1);
                        //copy.Append(list.ElementAt(index1));
                    }

                    yield return list.ElementAt(index);
                    //copy.Append(list.ElementAt(index));
                }
            }
        }
    }
}
