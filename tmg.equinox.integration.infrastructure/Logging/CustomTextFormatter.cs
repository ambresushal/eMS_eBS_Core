using System;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace tmg.equinox.integration.infrastructure.logging
{
    [ConfigurationElementType(typeof(CustomFormatterData))]
    public class CustomTextFormatter : LogFormatter
    {
        private const string Error = "Error";
        public CustomTextFormatter(NameValueCollection nvc)
        {
            //not used
        }

        public static void LogError(Exception ex, string additionalMessage)
        {

        }

        public override string Format(LogEntry log)
        {
            string[] customAttributeArray = log.Message.Split('&').ToArray();
            return "Time UTC :" + (customAttributeArray.Length > 5 ? customAttributeArray[5] : "") + "\r\n"
                   + "Application :" + (customAttributeArray.Length > 0 ? customAttributeArray[0] : "") + "\r\n"
                   + "Host :" + (customAttributeArray.Length > 1 ? customAttributeArray[1] : "") + "\r\n"
                   + "Type :" + (customAttributeArray.Length > 2 ? customAttributeArray[2] : "") + "\r\n"
                   + "Source :" + (customAttributeArray.Length > 3 ? customAttributeArray[3] : "") + "\r\n"
                   + "Message :" + (customAttributeArray.Length > 4 ? customAttributeArray[4] : "") + "\r\n"
                   + "Category :" + Error;
        }
    }
}
