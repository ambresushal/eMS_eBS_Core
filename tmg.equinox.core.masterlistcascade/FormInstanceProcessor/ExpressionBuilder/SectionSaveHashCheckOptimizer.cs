using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using tmg.equinox.framework;

namespace tmg.equinox.expressionbuilder
{
    /// <summary>
    /// Only to be used for Rule Processing Optimization on Section Save
    /// Use Hash for comparison
    /// </summary>
    public class SectionSaveHashCheckOptimizer : ISectionSaveRuleOptimizer
    {
        private string _previousData;
        private string _currentData;
        public SectionSaveHashCheckOptimizer(string previousData, string currentData)
        {
            this._previousData = previousData;
            this._currentData = currentData;
        }

        /// <summary>
        /// Check if the section has any changes, by comparing the hashes
        /// </summary>
        /// <returns></returns>
        public bool hasSectionChanged()
        {
            bool hasSectionChanged = false;

            string previousDataHash = HashGenerator.ToMD5(_previousData);
            string currentDataHash = HashGenerator.ToMD5(_currentData);

            if (previousDataHash != currentDataHash)
            {
                hasSectionChanged = true;
            }

            return hasSectionChanged;
        }

        /// <summary>
        /// Check if the element at the path specified has changed, by comparing the hashes
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        public bool hasPartChanged(string jsonPath)
        {
            bool hasPartChanged = false;

            FormInstanceParser formInstanceParser = new FormInstanceParser();
            JObject previousDataJObject = JObject.Parse(_previousData);
            JObject currentDataJObject = JObject.Parse(_currentData);

            string previousPartData = JsonConvert.SerializeObject(previousDataJObject.SelectToken(jsonPath));
            string currentPartData = JsonConvert.SerializeObject(currentDataJObject.SelectToken(jsonPath));

            string previousDataHash = HashGenerator.ToMD5(previousPartData.ToString());
            string currentDataHash = HashGenerator.ToMD5(currentPartData.ToString());

            if (previousDataHash != currentDataHash)
            {
                hasPartChanged = true;
            }
            return hasPartChanged;
        }        
    }
}