using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.caching
{
    internal static class CachingHelper
    {
        public static string GetEncryptedCacheKey(string unEncryptedKey)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(unEncryptedKey);
            string hashString = Convert
              .ToBase64String(MD5.Create().ComputeHash(bytes));
            return hashString;
        }
    }
}
