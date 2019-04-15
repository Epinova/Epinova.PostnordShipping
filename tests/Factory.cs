using System;

namespace Epinova.PostnordShippingTests
{
    internal class Factory
    {
        /// <summary>
        /// Gets a positive integer.
        /// </summary>
        /// <returns></returns>
        public static int GetInteger()
        {
            return Math.Abs(GetString().GetHashCode());
        }

        public static string GetString(int maxLength = 0)
        {
            string value = Guid.NewGuid().ToString("N");

            if (0 >= maxLength)
                return value;

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }

        public static Uri GetUri()
        {
            return new Uri(String.Concat("http://www.", Guid.NewGuid(), ".com/"));
        }
    }
}