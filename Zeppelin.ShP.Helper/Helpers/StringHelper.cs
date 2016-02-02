using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zeppelin.ShP.Helper.Helpers
{
    /// <summary>
    /// Класс содержит методы по работе со строками
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Проверяет, одинаковые ли строки (без учета регистра)
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns>True - если строки равны</returns>
        public static bool IsEqual(string strA, string strB)
        {
            return String.Compare(strA, strB, true, CultureInfo.InvariantCulture) == 0;
        }

        /// <summary>
        /// Проверяет, одинаковые ли строки (без учета регистра, убирает пробелы в начале и в конце)
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns>True - если строки равны</returns>
        public static bool IsEqualWithTrim(string strA, string strB)
        {
            return IsEqual(strA.Trim(), strB.Trim());
        }

        /// <summary>
        /// Проверяет, что строки не равны
        /// </summary>
        /// <param name="strA"></param>
        /// <param name="strB"></param>
        /// <returns>True - если строки не равны<</returns>
        public static bool IsNotEqual(string strA, string strB)
        {
            return !IsEqual(strA, strB);
        }

        /// <summary>
        /// Обрезвет строку до maxLenght символов и добавляет ...
        /// </summary>
        /// <param name="value">Строка</param>
        /// <param name="maxLenght">Максимальное число символов</param>
        /// <returns></returns>
        public static string Short(this string value, int maxLenght)
        {
            if (value.Length > maxLenght)
            {
                value = value.Substring(0, maxLenght - 3) + "...";
            }
            return value;
        }

        /// <summary>
        /// Преобразовывает строку, чтобы ее можно было использовать в виде URL на странице
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Encode(this string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return url;
            }
            return SPHttpUtility.UrlPathEncode(url, true);
        }

        /// <summary>
        /// Safe method to convert string to uint
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static uint? ConvertToUint(this string integerValue)
        {
            uint? countToSet = null;
            int count = 0;
            if (int.TryParse(integerValue, out count))
            {
                countToSet = (uint)count;
            }

            return countToSet;
        }

        public static int? ConvertToInt(string integerValue)
        {
            int result;
            if (int.TryParse(integerValue, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static int ConvertToInt(string integerValue, int defaultValue)
        {
            int? result = ConvertToInt(integerValue);
            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        public static decimal? ConvertToDecimal(string decimalValue)
        {
            decimal result;
            if (decimal.TryParse(decimalValue, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static decimal ConvertToInt(string decimalValue, decimal defaultValue)
        {
            decimal? result = ConvertToDecimal(decimalValue);
            if (result.HasValue)
            {
                return result.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Объединяет строки из списка в одну строку, разделяя разделителем
        /// </summary>
        /// <param name="list">список строк</param>
        /// <param name="delimiter">разделитель</param>
        /// <returns></returns>
        public static string ToStringFromStringList(this List<string> list, string delimiter)
        {
            return string.Join(delimiter, list.ToArray());
        }

        /// <summary>
        /// Преобразвать объект в GUID
        /// </summary>
        /// <param name="s">строка для преобразрвания</param>
        /// <param name="value">выходной параметр Guid</param>
        /// <returns>True, если получилось преобразование</returns>
        public static bool TryConvertToGuid(this string s, out Guid value)
        {
            try
            {
                value = new Guid(s);
                return true;
            }
            catch (FormatException)
            {
                value = Guid.Empty;
                return false;
            }
        }

        /// <summary>
        /// Преобразовывет объект в строку. Если передан null или метод объекта возвращает пусто, но используется defaultValue  
        /// </summary>
        /// <param name="value">Объект</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns></returns>
        public static string ToStringOrDefault(this object value, string defaultValue)
        {
            if (defaultValue == null)
            {
                defaultValue = string.Empty;
            }

            if (value == null)
            {
                return defaultValue;
            }

            var converted = value.ToString();
            if (string.IsNullOrEmpty(converted))
            {
                return defaultValue;
            }

            return converted;
        }

        /// <summary>
        /// Преобразовывет объект в строку. Если передан null, метод возвращает пусто
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetStringOrDefault(object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            else
            {
                return o.ToString();
            }
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}
