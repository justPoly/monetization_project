using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Papae.UnitySDK.Extensions
{
    public static class StringExtensions
    {

        /// <summary>
        /// Set of Unicode Characters currently supported in the application for email, etc.
        /// </summary>
        public static readonly string UnicodeCharacters = "À-ÿ{L}{M}ÀàÂâÆæÇçÈèÉéÊêËëÎîÏïÔôŒœÙùÛûÜü«»€₣äÄöÖüÜß"; // German and French

        /// <summary>
        /// Set of Symbol Characters currently supported in the application for email, etc.
        /// Needed if a client side validator is being used.
        /// Not needed if validation is done server side.
        /// The difference is due to subtle differences in Regex engines.
        /// </summary>
        public static readonly string SymbolCharacters = @"!#%&'""=`{}~\.\-\+\*\?\^\|\/\$";

        /// <summary>
        /// Regular Expression string pattern used to match an email address.
        /// The following characters will be supported anywhere in the email address:
        /// ÀàÂâÆæÇçÈèÉéÊêËëÎîÏïÔôŒœÙùÛûÜü«»€₣äÄöÖüÜß[a - z][A - Z][0 - 9] _
        /// The following symbols will be supported in the first part of the email address(before the @ symbol):
        /// !#%&'"=`{}~.-+*?^|\/$
        /// Emails cannot start or end with periods,dashes or @.
        /// Emails cannot have two @ symbols.
        /// Emails must have an @ symbol followed later by a period.
        /// Emails cannot have a period before or after the @ symbol.
        /// </summary>
        public static readonly string EmailPattern = string.Format(
            @"^([\w{0}{2}])+@{1}[\w{0}]+([-.][\w{0}]+)*\.[\w{0}]+([-.][\w{0}]+)*$",                     //  @"^[{0}\w]+([-+.'][{0}\w]+)*@[{0}\w]+([-.][{0}\w]+)*\.[{0}\w]+([-.][{0}\w]+)*$",
            UnicodeCharacters,
            "{1}",
            SymbolCharacters
        );

        /// <summary>
        /// Check's if a given string can be parsed to a specified enum type
        /// </summary>
        /// <param name="value">String enum value</param>
        public static bool CanBeParsedToEnum<T>(string value)
        {
            try
            {
                Enum.Parse(typeof(T), value, true);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return false;
            }
        }

        public static string CombinePath(this string self, string path)
        {
            return System.IO.Path.Combine(self, path);
        }

        //This will compare ignoring the case in both strings
        public static bool Contains(this string self, string stringToCheck, bool ignoreCase)
        {
            if (!ignoreCase)
            {
                return self.Contains(stringToCheck);
            }
            else
            {
                return self.ToLower().Contains(stringToCheck.ToLower());
            }
        }

        public static bool ContainsAny(this string self, string[] checkList, out int index)
        {
            for (int i = 0; i < checkList.Length; i++)
            {
                var c = checkList[i];
                if (self.Contains(c))
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        // Encrypts a string using a key and the RC4 algorithm
        // http://entitycrisis.blogspot.com/2011/04/encryption-between-python-and-c.html
        // Test: http://rc4.online-domain-tools.com/
        public static byte[] EncodeRC4(this string data, string skey)
        {
            byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(data);
            var key = System.Text.ASCIIEncoding.ASCII.GetBytes(skey);
            byte[] s = new byte[256];
            byte[] k = new byte[256];
            byte temp;
            int i, j;

            for (i = 0; i < 256; i++)
            {
                s[i] = (byte)i;
                k[i] = key[i % key.GetLength(0)];
            }

            j = 0;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + k[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }

            i = j = 0;
            for (int x = 0; x < bytes.GetLength(0); x++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
                int t = (s[i] + s[j]) % 256;
                bytes[x] ^= s[t];
            }

            return bytes;
        }

        /// <summary>
        /// More performant version of string.EndsWith method.
        /// https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html
        /// </summary>
        public static bool EndsWithFast(this string content, string match)
        {
            int ap = content.Length - 1;
            int bp = match.Length - 1;

            while (ap >= 0 && bp >= 0 && content[ap] == match[bp])
            {
                ap--;
                bp--;
            }

            return (bp < 0 && content.Length >= match.Length) || (ap < 0 && match.Length >= content.Length);
        }

        public static string FirstCharToUpper(this string source)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }
            // Return char and concat substring.
            return char.ToUpper(source[0]) + source.Substring(1);
        }

        public static string FromBase64(this string self)
        {
            byte[] bytesEncodedInBase64 = Convert.FromBase64String(self);
            return Encoding.UTF8.GetString(bytesEncodedInBase64, 0, bytesEncodedInBase64.Length);
        }

        /// <summary>
        /// Convert a string hex value to a Color32 value
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="hexadecimal">Hex value.</param>
        public static Color FromHexToColor(this string hexadecimal)
        {
            int integerValue = Convert.ToInt32(hexadecimal, 16);

            byte R = (byte)((integerValue >> 24) & 0xFF);
            byte G = (byte)((integerValue >> 16) & 0xFF);
            byte B = (byte)((integerValue >> 8) & 0xFF);

            return new Color(R, G, B, 255);
        }

        /// <summary>
        /// Attempts to extract content between the specified matches (on first occurence).
        /// </summary>
        public static string GetBetween(this string content, string startMatchString, string endMatchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(startMatchString) && content.Contains(endMatchString))
            {
                var startIndex = content.IndexOf(startMatchString) + startMatchString.Length;
                var endIndex = content.IndexOf(endMatchString, startIndex);
                return content.Substring(startIndex, endIndex - startIndex);
            }
            else return null;
        }

        /// <summary>
        /// Attempts to extract content before the specified match (on first occurence).
        /// </summary>
        public static string GetBefore(this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var endIndex = content.IndexOf(matchString);
                return content.Substring(0, endIndex);
            }
            else return null;
        }

        /// <summary>
        /// Attempts to extract content before the specified match (on last occurence).
        /// </summary>
        public static string GetBeforeLast(this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var endIndex = content.LastIndexOf(matchString);
                return content.Substring(0, endIndex);
            }
            else return null;
        }

        /// <summary>
        /// Attempts to extract content after the specified match (on last occurence).
        /// </summary>
        public static string GetAfter(this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var startIndex = content.LastIndexOf(matchString) + matchString.Length;
                if (content.Length <= startIndex) return string.Empty;
                return content.Substring(startIndex);
            }
            else return null;
        }

        /// <summary>
        /// Attempts to extract content after the specified match (on first occurence).
        /// </summary>
        public static string GetAfterFirst(this string content, string matchString)
        {
            Debug.Assert(content != null);
            if (content.Contains(matchString))
            {
                var startIndex = content.IndexOf(matchString) + matchString.Length;
                if (content.Length <= startIndex) return string.Empty;
                return content.Substring(startIndex);
            }
            else return null;
        }

        /// <summary>
        /// Convert an array of bytes to a string of hex digits
        /// </summary>
        /// <param name="bytes">array of bytes</param>
        /// <returns>String of hex digits</returns>
        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Checks whether string is null, empty or consists of whitespace chars.
        /// </summary>
        public static bool IsNullEmptyOrWhiteSpace(string self)
        {
            if (String.IsNullOrEmpty(self))
                return true;

            return String.IsNullOrEmpty(self.TrimFull());
        }


        /// <summary>
        /// Validates the string is an Email Address...
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>bool</returns>
        public static bool IsValidEmail(this string self)
        {
            var valid = true;
            var isnotblank = false;

            var email = self.Trim();
            if (email.Length > 0)
            {
                // Email Address Cannot start with period.
                // Name portion must be at least one character
                // In the Name, valid characters are:  a-z 0-9 ! # _ % & ' " = ` { } ~ - + * ? ^ | / $
                // Cannot have period immediately before @ sign.
                // Cannot have two @ symbols
                // In the domain, valid characters are: a-z 0-9 - .
                // Domain cannot start with a period or dash
                // Domain name must be 2 characters.. not more than 256 characters
                // Domain cannot end with a period or dash.
                // Domain must contain a period
                isnotblank = true;
                valid = Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase) &&
                    !email.StartsWith("-") &&
                    !email.StartsWith(".") &&
                    !email.EndsWith(".") &&
                    !email.Contains("..") &&
                    !email.Contains(".@") &&
                    !email.Contains("@.");
            }

            return (valid && isnotblank);
        }

        /// <summary>
        /// Validates the string is an Email Address and outputs an error message if not.
        /// </summary>
        /// <returns><c>true</c> if is valid email the specified self error; otherwise, <c>false</c>.</returns>
        /// <param name="self">Self.</param>
        /// <param name="error">Error.</param>
        public static bool IsValidEmail(this string self, out string error)
        {
            error = string.Empty;

            bool firstMatch = Regex.IsMatch(self, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z");
            bool secondMath = Regex.IsMatch(self, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");

            if (firstMatch && secondMath)
            {
                return true;
            }

            error = "Email is not a valid email address.";
            return false;
        }

        /// <summary>
        /// Validates the string is an Email Address or a delimited string of email addresses...
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns>bool</returns>
        public static bool IsValidEmailDelimitedList(this string self, char delimiter = ';')
        {
            var valid = true;
            var isnotblank = false;

            string[] emails = self.Split(delimiter);

            foreach (string e in emails)
            {
                var email = e.Trim();
                if (email.Length > 0 && valid) // if valid == false, no reason to continue checking
                {
                    isnotblank = true;
                    if (!email.IsValidEmail())
                    {
                        valid = false;
                    }
                }
            }
            return (valid && isnotblank);
        }

        /// <summary>
        /// Compares a version string (in format #.#.###) with another of the same format,
        /// and return TRUE if this one is minor. Boths trings must have the same number of dot separators.
        /// </summary>
        public static bool IsVersionIsMinor(this string s, string version)
        {
            string[] thisV = s.Split('.');
            string[] otherV = version.Split('.');
            if (thisV.Length != otherV.Length) throw new ArgumentException("Invalid");
            for (int i = 0; i < thisV.Length; ++i)
            {
                int thisInt = Convert.ToInt32(thisV[i]);
                int otherInt = Convert.ToInt32(otherV[i]);
                if (i == thisV.Length - 1) return thisInt < otherInt;
                else if (thisInt == otherInt) continue;
                else if (thisInt < otherInt) return true;
                else if (thisInt > otherInt) return false;
            }
            throw new ArgumentException("Invalid");
        }

        static IEnumerable<char> InsertSpacesBeforeCaps(IEnumerable<char> input)
        {
            for (int i = 0; i < input.Count(); i++)
            {
                char currentChar = input.ElementAt(i);
                char nextChar = i + 1 < input.Count() ? input.ElementAt(i + 1) : currentChar;

                if (i != 0 && char.IsUpper(currentChar) && char.IsLower(nextChar))
                {
                    yield return ' ';
                }

                yield return currentChar;
            }
        }

        public static bool IsValidLength(this string self, int length, out string error)
        {
            error = string.Empty;
            //var hasMinimum4Chars = new Regex(@".{"+length+", }");

            //if (!hasMinimum4Chars.IsMatch(self)) 
            if (self.Length < length)
            {
                error = string.Format("field must have at least {0} characters", length);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if is valid password the specified password error.
        /// </summary>
        /// <returns><c>true</c> if is valid password the specified password error; otherwise, <c>false</c>.</returns>
        /// <param name="password">Password.</param>
        /// <param name="error">Error.</param>
        public static bool IsValidPassword(this string self, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrEmpty(self.Trim()))
            {
                error = "Password should not be empty";
                return false;
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{6,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(self))
            {
                error = "Password should contain At least one lower case letter";
                return false;
            }
            else if (!hasUpperChar.IsMatch(self))
            {
                error = "Password should contain At least one upper case letter";
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(self))
            {
                error = "Password should not be less than 6 or greater than 15 characters";
                return false;
            }
            else if (!hasNumber.IsMatch(self))
            {
                error = "Password should contain At least one numeric value";
                return false;
            }
            else if (!hasSymbols.IsMatch(self))
            {
                error = "Password should contain At least one special case characters";
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsValidUsername(this string self, out string error)
        {
            error = string.Empty;

            //var hasNumber = new Regex(@"[0-9]+");
            //var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum4Chars = new Regex(@".{4,12}");

            if (!hasMinimum4Chars.IsMatch(self))
            {
                error = "Username should not be less than 4 or greater than 12 characters";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Whether compared string is literally-equal (independent of case).
        /// </summary>
        public static bool LiterallyEquals(this string content, string comparedString)
        {
            Debug.Assert(content != null);
            return content.Equals(comparedString, StringComparison.OrdinalIgnoreCase);
        }

        // http://wiki.unity3d.com/index.php?title=MD5
        public static string MD5(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        /// <summary>
        /// Md5s the sum.
        /// </summary>
        /// <returns>The sum.</returns>
        /// <param name="strToEncrypt">String to encrypt.</param>
        public static string Md5Sum(this string value)
        {
            UTF8Encoding ue = new UTF8Encoding();
            byte[] bytes = ue.GetBytes(value);
            // encrypt bytes
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);
            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        /// <summary>
        /// Tries to parse string value to a specified enum type.
        /// Will print a warning in case of falture, and returen default value for a given Enum type
        /// </summary>
        /// <param name="value">String enum value</param>
        public static T ParseEnum<T>(string value)
        {
            try
            {
                T val = (T)Enum.Parse(typeof(T), value, true);
                return val;
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Enum Parsing failed: " + ex.Message);
                return default(T);
            }
        }

        /// <summary>
        /// Surround string with "color" tag
        /// </summary>
        public static string RichTextColor(this string message, RichTextColor color)
        {
            return string.Format("<color={0}>{1}</color>", color, message);
        }

        /// <summary>
        /// Surround string with "color" tag
        /// </summary>
        public static string RichTextColor(this string message, string colorCode)
        {
            return string.Format("<color={0}>{1}</color>", colorCode, message);
        }

        /// <summary>
        /// Surround string with "size" tag
        /// </summary>
        public static string RichTextSize(this string message, int size)
        {
            return string.Format("<size={0}>{1}</size>", size, message);
        }

        /// <summary>
        /// Surround string with "b" tag
        /// </summary>
        public static string RichTextBold(this string message)
        {
            return string.Format("<b>{0}</b>", message);
        }

        /// <summary>
        /// Surround string with "i" tag
        /// </summary>
        public static string RichTextItalics(this string message)
        {
            return string.Format("<i>{0}</i>", message);
        }

        /// <summary>
        /// Compute hash for string encoded as UTF8
        /// </summary>
        /// <param name="s">String to be hashed</param>
        /// <returns>40-character hex string</returns>
        public static string SHA1HashStringForUTF8String(this string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        public static string SpaceCamelCase(this string str)
        {
            return new string(InsertSpacesBeforeCaps(str).ToArray());
        }

        public static string InsertSpaceBeforeUpperCase(this string str)
        {
            var sb = new StringBuilder();
            var previousChar = char.MinValue; // Unicode '\0'

            foreach (var c in str)
            {
                if (char.IsUpper(c))
                {
                    if (sb.Length != 0 && previousChar != ' ')
                    {
                        sb.Append(' ');
                    }
                }

                sb.Append(c);
                previousChar = c;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Takes a string in camel case, split it into separate words, and 
        /// capitalizes each word.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SplitCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            string camelCase = Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            string firstLetter = camelCase.Substring(0, 1).ToUpper();

            if (str.Length > 1)
            {
                string rest = camelCase.Substring(1);

                return firstLetter + rest;
            }
            else
            {
                return firstLetter;
            }
        }

        /// <summary>
        /// Splits the string using new line symbol as a separator.
        /// Will split by all type of new lines, independant of environment.
        /// </summary>
        public static string[] SplitByNewLine(this string content)
        {
            if (string.IsNullOrEmpty(content)) return null;

            // "\r\n"   (\u000D\u000A)  Windows
            // "\n"     (\u000A)        Unix
            // "\r"     (\u000D)        Mac
            // Not using Environment.NewLine here, as content could've been produced 
            // in not the same environment we running the program in.
            return content.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// More performant version of string.StartsWith method.
        /// https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html
        /// </summary>
        public static bool StartsWithFast(this string content, string match)
        {
            int aLen = content.Length;
            int bLen = match.Length;
            int ap = 0, bp = 0;

            while (ap < aLen && bp < bLen && content[ap] == match[bp])
            {
                ap++;
                bp++;
            }

            return (bp == bLen && aLen >= bLen) || (ap == aLen && bLen >= aLen);
        }

        //Returns string between _startString and _endString from _string.
        public static string StringBetween(this string value, string startString, string endString, bool ignoreCase)
        {
            StringComparison _comparision = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            int startStringLength = startString != null ? startString.Length : 0;
            int startStringOccuranceIndex = value.IndexOf(startString, _comparision);

            //Check the end string next to the occurance of above start string.
            int endStringOccuranceIndex = value.IndexOf(endString, startStringOccuranceIndex + startStringLength, _comparision);

            string subString;

            if (startStringOccuranceIndex == -1 || endStringOccuranceIndex == -1)
            {
                subString = "";
            }
            else
            {
                int _lengthRequired = endStringOccuranceIndex - (startStringOccuranceIndex + startStringLength);//Shouldn't include the strings use for matching
                subString = value.Substring(startStringOccuranceIndex + startStringLength, _lengthRequired);
            }

            return subString;
        }

        /// <summary>
        /// Converts to array.
        /// </summary>
        /// <returns>The to array.</returns>
        /// <param name="arrayString">Array string.</param>
        /// <param name="seperator">Seperator.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] ToArray<T>(this string arrayString, char seperator = ',')
        {
            List<T> indexes = new List<T>();

            if (!string.IsNullOrEmpty(arrayString.Trim()))
            {
                string[] ids = arrayString.Split(seperator);

                foreach (string id in ids)
                {
                    T value = default(T);
                    value = (T)Convert.ChangeType(id, typeof(T));
                    indexes.Add(value);
                }
            }

            return indexes.ToArray();
        }

        public static string ToBase64(this string value)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(value);
            return System.Convert.ToBase64String(bytesToEncode);
        }

        /// <summary>
        /// Converts the string to a color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="colorString">A string.</param>
        public static Color ToColor(this string colorString)
        {
            Color clr = new Color(0, 0, 0);
            if (colorString != null && colorString.Length > 0)
            {
                try
                {
                    int commaCount = 0;
                    foreach (char c in colorString)
                    {
                        if (c == ',') commaCount++;
                    }

                    if (colorString.Substring(0, 1) == "#")
                    {  // #FFFFFF format
                        string str = colorString.Substring(1, colorString.Length - 1);
                        clr.r = int.Parse(str.Substring(0, 2),
                            NumberStyles.AllowHexSpecifier) / 255.0f;
                        clr.g = int.Parse(str.Substring(2, 2),
                            NumberStyles.AllowHexSpecifier) / 255.0f;
                        clr.b = int.Parse(str.Substring(4, 2),
                            NumberStyles.AllowHexSpecifier) / 255.0f;
                        if (str.Length == 8)
                        {
                            clr.a = int.Parse(str.Substring(6, 2),
                               NumberStyles.AllowHexSpecifier) / 255.0f;
                        }
                        else clr.a = 1.0f;
                    }
                    else if (commaCount >= 2 && commaCount <= 3 && colorString.Length >= 2 * commaCount + 1)
                    {  // 0.3, 1.0, 0.2 format
                        int p0 = 0;
                        int p1 = 0;
                        int c = 0;
                        p1 = colorString.IndexOf(",", p0);

                        if (p1 <= 0)
                            throw new FormatException();

                        while (p1 > p0 && c < 4)
                        {
                            float value = float.Parse(colorString.Substring(p0, p1 - p0));
                            value = Mathf.Clamp(value, 0, 1);
                            clr[c++] = value;
                            p0 = p1 + 1;
                            if (p0 < colorString.Length) p1 = colorString.IndexOf(",", p0);
                            if (p1 < 0) p1 = colorString.Length;
                        }
                        if (c < 4) clr.a = 1.0f;
                    }
                    else
                        throw new FormatException();
                }
                catch (Exception e)
                {
                    Debug.LogError("ToColor could not convert " + colorString + " to Color. " + e);
                    return Color.magenta;
                }
            }
            else
                return Color.magenta;

            return clr;
        }

        /// <summary>
        /// Converts string to enum of type T. T must be an enum
        /// otherwise a 'NotSupportedException' exception is thrown.
        /// </summary>
        public static T ToEnum<T>(this string value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new NotSupportedException("Must be an Enum! type: " + typeof(T).Name);

            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Converts a comma delimited string to a UnityEngine Vector3
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this string vector3)
        {
            var split = vector3.Split(',').Select(Convert.ToSingle).ToArray();
            return split.Length == 3 ? new Vector3(split[0], split[1], split[2]) : Vector3.zero;
        }

        /// <summary>
        /// Tos the vertical string.
        /// </summary>
        /// <returns>The vertical string.</returns>
        /// <param name="value">Input.</param>
        public static string ToVerticalString(this string value)
        {
            StringBuilder sb = new StringBuilder(value.Length * 2);
            foreach (char chr in value)
            {
                sb.Append(chr).Append("\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Removes mathing trailing string.
        /// </summary>
        public static string TrimEnd(this string source, string value)
        {
            if (!source.EndsWithFast(value))
                return source;

            return source.Remove(source.LastIndexOf(value));
        }

        /// <summary>
        /// Performes <see cref="string.Trim"/> additionally removing any BOM and other service symbols.
        /// </summary>
        public static string TrimFull(this string source)
        {
#if UNITY_WEBGL // WebGL build under .NET 4.6 fails when using Trim with UTF-8 chars. (should be fixed in Unity 2018.1)
            var whitespaceChars = new System.Collections.Generic.List<char> {
                '\u0009','\u000A','\u000B','\u000C','\u000D','\u0020','\u0085','\u00A0',
                '\u1680','\u2000','\u2001','\u2002','\u2003','\u2004','\u2005','\u2006',
                '\u2007','\u2008','\u2009','\u200A','\u2028','\u2029','\u202F','\u205F',
                '\u3000','\uFEFF','\u200B',
            };

            // Trim start.
            if (string.IsNullOrEmpty(source)) return source;
            var c = source[0];
            while (whitespaceChars.Contains(c))
            {
                if (source.Length <= 1) return string.Empty;
                source = source.Substring(1);
                c = source[0];
            }

            // Trim end.
            if (string.IsNullOrEmpty(source)) return source;
            c = source[source.Length - 1];
            while (whitespaceChars.Contains(c))
            {
                if (source.Length <= 1) return string.Empty;
                source = source.Substring(0, source.Length - 1);
                c = source[source.Length - 1];
            }

            return source;
#else
            return source.Trim().Trim(new char[] { '\uFEFF', '\u200B' });
#endif
        }

#pragma warning disable 168


        /// <summary>
        /// Tries to parse string value to a specified enum type
        /// </summary>
        /// <param name="value">String enum value</param>
        /// <param name="result">Enum result</param>
        public static bool TryParseEnum<T>(string value, out T result)
        {
            try
            {
                result = (T)Enum.Parse(typeof(T), value, true);
                return true;
            }
            catch (Exception ex)
            {
                result = default(T);
                return false;
            }
        }

#pragma warning restore 168
    }

    public enum RichTextColor
    {
        aqua,
        black,
        blue,
        brown,
        cyan,
        darkblue,
        fuchsia,
        green,
        grey,
        lightblue,
        lime,
        magenta,
        maroon,
        navy,
        olive,
        purple,
        red,
        silver,
        teal,
        white,
        yellow
    }
}