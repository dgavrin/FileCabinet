using System;
using System.Globalization;

namespace FileCabinetApp
{
    /// <summary>
    /// Provides methods for converting input.
    /// </summary>
    public static class InputConverters
    {
        private static readonly CultureInfo CultureEnUS = new CultureInfo("en-US");

        /// <summary>
        /// Convert string to int.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>True if it is possible to convert and resulting int.</returns>
        public static Tuple<bool, string, int> IntConverter(string value)
        {
            var conversionIsPossible = false;
            if (int.TryParse(value, out int result))
            {
                conversionIsPossible = true;
            }

            return new Tuple<bool, string, int>(conversionIsPossible, "short", result);
        }

        /// <summary>
        /// Convert string to string.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>True if it is possible to convert and the resulting string.</returns>
        public static Tuple<bool, string, string> StringConverter(string value)
        {
            return new Tuple<bool, string, string>(!string.IsNullOrEmpty(value), "string", value);
        }

        /// <summary>
        /// Convert string to DateTime.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>True if it is possible to convert and resulting DateTime.</returns>
        public static Tuple<bool, string, DateTime> DateConverter(string value)
        {
            DateTime result;
            var conversionIsPossible = false;
            if (DateTime.TryParse(value, CultureEnUS, DateTimeStyles.None, out result))
            {
                conversionIsPossible = true;
            }

            return new Tuple<bool, string, DateTime>(conversionIsPossible, "DateTime", result);
        }

        /// <summary>
        /// Convert string to decimal.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>True if it is possible to convert and resulting decimal.</returns>
        public static Tuple<bool, string, decimal> WalletConverter(string value)
        {
            decimal result;
            var conversionIsPossible = false;
            if (decimal.TryParse(value, out result))
            {
                conversionIsPossible = true;
            }

            return new Tuple<bool, string, decimal>(conversionIsPossible, "decimal", result);
        }

        /// <summary>
        /// Convert string to char.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>True if it is possible to convert and resulting char.</returns>
        public static Tuple<bool, string, char> MaritalStatusConverter(string value)
        {
            char result;
            var conversionIsPossible = false;
            if (char.TryParse(value, out result))
            {
                conversionIsPossible = true;
            }

            return new Tuple<bool, string, char>(conversionIsPossible, "char", result);
        }

        /// <summary>
        /// Convert string to short.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>True if it is possible to convert and resulting short.</returns>
        public static Tuple<bool, string, short> HeightConverter(string value)
        {
            short result;
            var conversionIsPossible = false;
            if (short.TryParse(value, out result))
            {
                conversionIsPossible = true;
            }

            return new Tuple<bool, string, short>(conversionIsPossible, "short", result);
        }
    }
}
