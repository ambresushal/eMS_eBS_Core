using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.ruleprocessor;

namespace tmg.equinox.document.rulebuilder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
        static void Main()
        {
            string ruleJSON = "";
            if (string.IsNullOrWhiteSpace(ruleJSON) || string.IsNullOrEmpty(ruleJSON))
                Console.WriteLine("Please Enter JSON Formatted Custom Rule.");

            else
            {
                List<string> rules = new List<string>();
                rules.Add(ruleJSON);
                //DocumentRulesProcessor ruleProcessor = new DocumentRulesProcessor(10, 10, rules); // TODO : Remove Hard coded Value

                //JToken ruleOutput = ruleProcessor.ProcessDocumentRules();
                //string ruleResult= ruleOutput.ToString();
            }
        }
    }
}
