using System;
using System.Collections.Generic;

namespace Papae.UnitySDK.Extensions
{
    /// <summary>
    /// Extentions for enums.
    /// </summary>
    public static class EnumExtentions
    {
        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        public static int GetEnumCount(this Enum enumValue)
        {
            return Enum.GetNames(enumValue.GetType()).Length;
        }

        public static int GetValue(this Enum enumValue)
        {
            int finalValue = 0;
            Type enumType = enumValue.GetType();

            foreach (int value in Enum.GetValues(enumType))
            {
                if (((int)(object)enumValue & value) != 0)
                    finalValue |= value;
            }

            return finalValue;
        }

        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// A FX 3.5 way to mimic the FX4 "HasFlag" method. Also worked well with "None".
        /// </summary>
        /// <param name="a">The tested enum.</param>
        /// <param name="b">The value to test.</param>
        /// <returns>True if the flag is set. Otherwise false.</returns>
        public static bool HasFlag(this Enum a, Enum b)
        {
            // check if from the same type.
            if (a.GetType() != b.GetType())
            {
                throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
            }

            TypeCode typeCodeA = a.GetTypeCode();
            TypeCode typeCodeB = b.GetTypeCode();

            bool hasConverted;
            long i = DoConvert<long>(GetTypeValue(a, typeCodeA), out hasConverted);
            long j = DoConvert<long>(GetTypeValue(b, typeCodeB), out hasConverted);
            long k = (i & j);

            return k == j;
        }

        public static bool HasAllFlags(this Enum a)
        {
            TypeCode typeCodeA = a.GetTypeCode();

            bool hasConverted;
            long i = DoConvert<long>(GetTypeValue(a, typeCodeA), out hasConverted);

            return i == -1;
        }

        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not remove value from enumerated type '{0}'.",
                        typeof(T).Name
                        ), ex);
            }
        }

        public static Enum SetFlag(this Enum a, Enum b)
        {
            // check if from the same type.
            if (a.GetType() != b.GetType())
            {
                throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
            }

            long x = Convert.ToInt64(a);
            long y = Convert.ToInt64(b);

            return (Enum)Enum.ToObject(a.GetType(), x | y);
        }

        public static Enum ToogleFlag(this Enum a, Enum b)
        {
            // check if from the same type.
            if (a.GetType() != b.GetType())
            {
                throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
            }

            long x = Convert.ToInt64(a);
            long y = Convert.ToInt64(b);

            return (Enum)Enum.ToObject(a.GetType(), x ^ y);
        }

        public static Enum UnsetFlag(this Enum a, Enum b)
        {
            // check if from the same type.
            if (a.GetType() != b.GetType())
            {
                throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
            }

            long x = Convert.ToInt64(a);
            long y = Convert.ToInt64(b);

            return (Enum)Enum.ToObject(a.GetType(), x & (~y));
        }

        static object GetTypeValue(Enum value, TypeCode code)
        {
            switch (code)
            { 
                case TypeCode.Byte: return Convert.ToByte(value);
                case TypeCode.SByte: return Convert.ToSByte(value);

                case TypeCode.Int16: return Convert.ToInt16(value);
                case TypeCode.Int32: return Convert.ToInt32(value);
                case TypeCode.Int64: return Convert.ToInt64(value);
                    
                case TypeCode.UInt16: return Convert.ToUInt16(value);
                case TypeCode.UInt32: return Convert.ToUInt32(value);
                case TypeCode.UInt64: return Convert.ToUInt64(value);
                    
                default: return null;
            }
        }

        public static TConvertType DoConvert<TConvertType>(object convertValue, out bool hasConverted)
        {
            hasConverted = false;
            var converted = default(TConvertType);
            try
            {
                converted = (TConvertType)
                    Convert.ChangeType(convertValue, typeof(TConvertType));
                hasConverted = true;
            }
            catch (InvalidCastException)
            {
            }
            catch (ArgumentNullException)
            {
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
            {
            }

            return converted;
        }

        public static string[] TotStringArray(Enum[] enums)
        {
            List<string> stringList = new List<string>();

            if (enums != null && enums.Length > 0)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    stringList.Add(enums[i].ToString());
                }
            }

            return stringList.ToArray();
        }

        public static string[] ToStringArray(this Enum[] enums)
        {
            List<string> stringList = new List<string>();

            if (enums != null && enums.Length > 0)
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    stringList.Add(enums[i].ToString());
                }
            }

            return stringList.ToArray();
        }
    }
}