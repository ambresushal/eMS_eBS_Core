using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.infrastructure.util;

namespace tmg.equinox.documentcomparer
{
    public class DocumentJSONReaderHelper
    {
        public static string GetSectionFieldValue(JToken parentSectionToken, string fieldName, CompareDocumentSource compareSource)
        {
            string fieldValue = null;
            JToken fieldToken = parentSectionToken.SelectToken(fieldName);
            if (fieldToken != null)
            {
                fieldValue = fieldToken.ToString();
            }

            if (!string.IsNullOrEmpty(fieldValue) && HtmlContentHelper.IsHTML(fieldValue) && compareSource == CompareDocumentSource.GENERATEREPORT)
                fieldValue= HtmlContentHelper.GetFreeFromHtmlText(fieldValue);

            return fieldValue;
        }

        public static JToken GetSectionToken(JToken parent, JToken pathChild)
        {
            JToken sectionToken = parent.SelectToken(pathChild.Parent.Parent.Path);
            return sectionToken;
        }
    }
}
