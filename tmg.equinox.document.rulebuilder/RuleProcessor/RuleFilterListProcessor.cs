using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.ruleprocessor
{
   public class RuleFilterListProcessor
    {
       RuleFilter _sourceRuleFilter;
       Dictionary<string, JToken> _source;

       public RuleFilterListProcessor(RuleFilter sourceRuleFilter, Dictionary<string, JToken> source)
       {
           _sourceRuleFilter = sourceRuleFilter;
           _source = source;
       }

       public Dictionary<string, JToken> ProcessFilterItemList()
       {
           Dictionary<string, JToken> ruleFilterDictionary = new Dictionary<string, JToken>();
           foreach (RuleFilterItem item in _sourceRuleFilter.Filters)
           {
               ruleFilterDictionary.Add(item.FilterName, ProcessFilterItem(item));
           }
          return ruleFilterDictionary;
       }

       private JToken ProcessFilterItem(RuleFilterItem filterItem)
       {
           RuleFilterProcessor filterItemProcessor = new RuleFilterProcessor(filterItem, _source);
           return  filterItemProcessor.ProcessFilterItem();
       }
    }
}
