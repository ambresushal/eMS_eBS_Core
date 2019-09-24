using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.sbccalculator.Model;

namespace tmg.equinox.sbccalculator.Helper
{
    public static class ProcessingRuleManager
    {
        #region Public

        public static List<KeyValuePair<string, int>> GetRuleExecutionSequence(string processingRule)
        {
            Dictionary<string, int> dict = SetProcessingRuleSequence().Where(s => s.ProcessingRule == processingRule)
                                            .Select(s => s.SequenceDict)
                                            .FirstOrDefault();
            List<KeyValuePair<string, int>> seq = new List<KeyValuePair<string, int>>();
            if (dict != null)
            {
                seq = dict.ToList<KeyValuePair<string, int>>()
                                                     .OrderBy(s => s.Value)
                                                     .ToList();
            }
            return seq;
        }

        public static string GetProcessRule(CoverageExample example)
        {
            string processingRule = SBCConstant.NoRule;

            if (example.Copay.Contains('$') && !example.Coinsurance.Contains('%') && !example.Deductible.Contains('$'))
            {
                processingRule = SBCConstant.AllCopay;
            }
            else if (!example.Copay.Contains('$') && example.Coinsurance.Contains('%') && !example.Deductible.Contains('$'))
            {
                processingRule = SBCConstant.AllCoins;
            }
            else if (!example.Copay.Contains('$') && !example.Coinsurance.Contains('%') && example.Deductible.Contains('$'))
            {
                processingRule = SBCConstant.AllDeductible;
            }
            else if (!example.Copay.Contains('$') && example.Coinsurance.Contains('%') && example.Deductible.Contains('$'))
            {
                processingRule = SBCConstant.FirstDeductibleThenCoinsurance;
            }
            else if (example.Copay.Contains('$') && !example.Coinsurance.Contains('%') && example.Deductible.Contains('$'))
            {
                processingRule = SBCConstant.FirstCopayThenDeductible;
            }
            else if (example.Copay.Contains('$') && example.Coinsurance.Contains('%') && example.Deductible.Contains('$'))
            {
                processingRule = SBCConstant.FirstCopayDeductibleThenCoinsurance;
            }
            else if (example.Copay.Contains('$') && example.Coinsurance.Contains('%'))
            {
                processingRule = SBCConstant.CopayThenCoinsurance;
            }
            return processingRule;
        }

        #endregion Public

        #region  Private

        private static List<ProcessingRuleSequence> SetProcessingRuleSequence()
        {
            string ruleJson = "[{'SequenceDict':{'Copay':1},'ProcessingRule':'All Copay'},{'SequenceDict':{'Coinsurance':1},'ProcessingRule':'All Coinsurance'},{'SequenceDict':{'Deductible':1},'ProcessingRule':'All Deductible'},{'SequenceDict':{'Copay':1,'Deductible':2},'ProcessingRule':'First Copay Then Deductible'},{'SequenceDict':{'Coinsurance':1,'Deductible':2},'ProcessingRule':'First Coinsurance Then Deductible'},{'SequenceDict':{'Deductible':1,'Copay':2},'ProcessingRule':'First Deductible Then Copay'},{'SequenceDict':{'Deductible':1,'Coinsurance':2},'ProcessingRule':'First Deductible Then Coinsurance'},{'SequenceDict':{'Copay':1,'Deductible':2,'Coinsurance':3},'ProcessingRule':'First Copay, Deductible Then Coinsurance'},{'SequenceDict':{'Copay':1,'Coinsurance':2},'ProcessingRule':'Copay Then Coinsurance'}]";
            List<ProcessingRuleSequence> ProcessingRuleSequencelist = JsonConvert.DeserializeObject<List<ProcessingRuleSequence>>(ruleJson);
            return ProcessingRuleSequencelist;
        }

        #endregion Private
    }

}
