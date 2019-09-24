
using tmg.equinox.infrastructure.util;

namespace tmg.equinox.expressionbuilder
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

            string currentDataHash = HashGenerator.ToMD5(currentData);
            string previousDataHash = HashGenerator.ToMD5(oldData);
            
            if (previousDataHash != currentDataHash)
            {
                hasDataChanged = true;
            }

            return hasDataChanged;
        }     
    }
}