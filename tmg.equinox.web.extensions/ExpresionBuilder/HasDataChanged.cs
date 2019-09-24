using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.infrastructure.util;
using tmg.equinox.web.Framework;

namespace tmg.equinox.web.ExpresionBuilder
{
    public static class HasDataChanged
    {

        /// <summary>
        /// Check if the section has any changes, by comparing the hashes
        /// </summary>
        /// <returns></returns>
        public static bool hasDataChanged(string currentData,string oldData)
        {
            bool hasDataChanged = false;

            string currentDataHash = HashGenerator.ToSHA1(currentData);
            string previousDataHash = HashGenerator.ToSHA1(oldData);
            
            if (previousDataHash != currentDataHash)
            {
                hasDataChanged = true;
            }

            return hasDataChanged;
        }     
    }
}