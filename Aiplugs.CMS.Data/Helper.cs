using System;

namespace Aiplugs.CMS.Data
{
    internal class Helper
    {
        private static Random random = new Random();

        /// <summary>
        /// Create descending key
        /// </summary>
        /// <returns>Time-based key</returns>
        public static string CreateKey()
            => $"{(DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks)}{random.Next(1000000000):000000000}";
    }
}
