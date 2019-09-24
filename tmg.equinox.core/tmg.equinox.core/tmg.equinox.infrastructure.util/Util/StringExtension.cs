using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tmg.equinox.infrastructure.util.extensions
{
    public static class StringExtension
    {
        #region Private Memebers

        #endregion Private Members

        #region Public Properties

        #endregion Public Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods
        public static string ToCamelCase(this string inputString)
        {
            // If there are 0 or 1 characters, just return the string.
            if (inputString == null || inputString.Length < 2) return inputString;

            // Split the string into words.
            string[] words = inputString.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }

            return result;
        }

        public static string ToUpperFirstLetter(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            if (letters.Length > 1)
            {
                for (int i = 1; i < letters.Length; i++)
                {
                    letters[i] = char.ToLower(letters[i]);
                }
            }
            // return the array made of the new char array
            return new string(letters);
        }

        private static string GetGeneratedName(string label)
        {
            string generatedName = "";
            if (!String.IsNullOrEmpty(label))
            {
                Regex regex = new Regex("[^a-zA-Z0-9]");
                generatedName = regex.Replace(label, String.Empty);
                if (generatedName.Length > 70)
                {
                    generatedName = generatedName.Substring(0, 70);
                }
            }
            return generatedName;
        }

        public static string RemoveSpecialCharacters(this string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in inputString)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_' || c == '[' || c == ']')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods
    }
}
