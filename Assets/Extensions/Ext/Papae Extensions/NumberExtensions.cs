using System;
using UnityEngine;

namespace Papae.UnitySDK.Extensions
{
    public static class NumberExtensions
    {
        public static bool DoubleEquals(this double num1, double num2, double threshold = .0001f)
        {
            return Math.Abs(num1 - num2) < threshold;
        }

        public static bool FloatEquals(this float num1, float num2, float threshold = .0001f)
        {
            return Math.Abs(num1 - num2) < threshold;
        }

        public static string FormatNumber(this float number, string thousandsSeparator = ",", string decimalSeparator = ".", float decimalPlaces = float.NaN) 
        {
            var nInt = Math.Floor(number);
            var nDec = number - nInt;

            var sInt = nInt.ToString("F");
            string sDec;

            if (!float.IsNaN(decimalPlaces)) 
            {
                sDec = (Math.Round(nDec* Math.Pow(10, decimalPlaces)) / Math.Pow(10, decimalPlaces)).ToString("F").Substring(2);
            } 
            else 
            {
                sDec = nDec == 0 ? "" : nDec.ToString("F").Substring(2);
            }

            string fInt = "";
            int i;
            for (i = 0; i<sInt.Length; i++)
            {
                fInt += sInt.Substring(i, 1);
                if ((sInt.Length - i - 1) % 3 == 0 && i != sInt.Length - 1) fInt += thousandsSeparator;
            }

            return fInt + (sDec.Length > 0 ? decimalSeparator + sDec : "");
        }

        public static bool IsEven(this int number)
        {
            return number % 2 == 0;
        }

        public static bool IsNotNegative(this int value)
        {
            return value >= 0;
        }

        /// <summary>
        /// Determines whether the specified value is a valid number.
        /// </summary>
        /// <param name="number">The number to check.</param>
        /// <returns>A flag indicating whether the specified value is a number.</returns>
        public static bool IsNumber(this double number)
        {
            return !double.IsInfinity(number) && !double.IsNaN(number);
        }

        /// <summary>
        /// Determines whether the specified value is a valid number.
        /// </summary>
        /// <param name="number">The number to check.</param>
        /// <returns>A flag indicating whether the specified value is a number.</returns>
        public static bool IsNumber(this float number)
        {
            return !float.IsInfinity(number) && !float.IsNaN(number);
        }

        public static bool IsOdd(this int number)
        {
            return number % 2 != 0;
        }

        public static bool IsPrime(this int number)
        {
            if (number % 2 == 0)
            {
                return number == 2;
            }

            int sqrt = (int)Math.Sqrt(number);

            for (int divisor = 3; divisor <= sqrt; divisor += 2)
            {
                if (number % divisor == 0)
                {
                    return false;
                }
            }

            return number != 1;
        }

        /// <summary>Returns TRUE if the int is within the given range.</summary>
        /// <param name="min">Min</param>
        /// <param name="max">Max</param>
        /// <param name="inclusive">If TRUE min/max range values will be valid, otherwise not</param>
        /// <returns></returns>
        public static bool IsWithinRange(this int number, int min, int max, bool inclusive = true)
        {
            return inclusive ? number >= min && number <= max : number > min && number < max;
        }

        /// <summary>
        /// Convert 32-bit int to a Color32 value
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="number">Int value.</param>
        public static Color ToColor(this int number)
        {
            byte R = (byte)((number >> 24) & 0xFF);
            byte G = (byte)((number >> 16) & 0xFF);
            byte B = (byte)((number >> 8) & 0xFF);

            return new Color(R, G, B, 255);
        }

        public static string ToOccurrence(this int number)
        {
            switch (number)
            {
                case 1: return "once";
                case 2: return "twice";
                case 3: return "thrice";
                default: return string.Format("{0} times", number);
            }
        }

        public static string ToPosition(this int number)
        {
            string result = string.Empty;
            if (number <= 0)
            {
                if (number < 0)
                {
                    return result;
                }

                return "None";
            }

            result = number.ToString();
            int remainder = number % 10;

            switch (remainder)
            {
                case 1: result += "st"; break;
                case 2: result += "nd"; break;
                case 3: result += "rd"; break;
                default: result += "th"; break;
            }

            return result;
        }

        public static string ToTimeFormat(this long timeInMilliseconds)
        {
            return string.Format("{0:0.000}", timeInMilliseconds / 1000.0);
        }

        public static string ToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + ToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += ToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += ToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }


    }
}
