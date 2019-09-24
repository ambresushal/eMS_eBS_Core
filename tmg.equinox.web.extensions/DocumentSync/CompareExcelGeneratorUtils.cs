using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.documentcomparer
{
    public class CompareExcelGeneratorUtils
    {
        private static string GetNextColumn(string currentColumn)
        {
            //only for column 'A' to column 'AZ' supported
            int charValue = (int)currentColumn[currentColumn.Length - 1];
            string newColumn = currentColumn.Substring(0, currentColumn.Length - 1);
            if ((charValue - 65) < 25)
            {
                charValue = charValue + 1;
                newColumn = newColumn + ((char)charValue).ToString();
            }
            else
            {
                if (currentColumn.Length == 1)
                {
                    newColumn = "AA";
                }
            }
            return newColumn;
        }
    }
}
