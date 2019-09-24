using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace tmg.equinox.integration.infrastructure.Util
{
    public class Util
    {
        public static string GetKeyValue(string keyName)
        {
            string keyValue = "";
            if (keyName != "")
            {
                keyValue = ConfigurationManager.AppSettings[keyName];
            }
            return keyValue;
        }

        public static string ConvertToBase36 (int keyVal)
        {
            int baseVal = 36;

            if ((keyVal < 0))
                return null;

            string allDigits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            string finalVal = string.Empty;

            while (keyVal-- > 0)
            {
                finalVal += allDigits.Substring((keyVal % baseVal + 1), 1);
                keyVal /= baseVal;
            }
            finalVal = finalVal.PadLeft(4, '0');
            return finalVal; ;
        }

        public static int ConvertBase10(string keyVal)
        {
            int baseVal = 10;
            int returnVal = 0;
            keyVal = keyVal.TrimStart('0').ToUpper();
            int length = keyVal.Length;
            int position = 0;
            while (true)
            {
                position++;
                if (position > length)
                    break;
                char charVal = Convert.ToChar(keyVal.Substring((length - (position)), 1));
                int asciiVal = (int)charVal;
                if (asciiVal >= 48 && asciiVal <= 57) asciiVal -= 48;
                else if (asciiVal >= 65 && asciiVal <= 90) asciiVal -= 65 + 10;
                else asciiVal = 0;
                returnVal += ((int)Math.Pow(Convert.ToDouble(baseVal), Convert.ToDouble(position - 1)) * Math.Abs(asciiVal));
            }
            return returnVal;
        }

    }
}
