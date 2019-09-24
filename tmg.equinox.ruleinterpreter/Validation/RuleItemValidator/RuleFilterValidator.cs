using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.validation
{
    public  class RuleFilterValidator
    {
        Filter _ruleFilter;
        Dictionary<string, List<string>> _validationErrorMessages;

        public RuleFilterValidator(Filter ruleFilter, ref Dictionary<string, List<string>> validationErrorMessages)
        {
            _ruleFilter = ruleFilter;
            _validationErrorMessages = validationErrorMessages;
        }

        public void ValidateFilters()
        {
            ValidateRuleFilterMergeType();
            ValidateFilterMergeAction();
               
            foreach (var filterItem in _ruleFilter.filterlist)
            {
                ValidateRuleFilterType(filterItem.filtertype);
                ValidateFilterExpression(filterItem.filterexpression);
            }
            
        }
        
        private void ValidateRuleFilterMergeType()
        {
            // Validate input filtermerge type againts FiterType Enum.
        }

        private void ValidateFilterMergeAction()
        {
            //validate filter merge actions syntactically.
        }
        
        private void ValidateRuleFilterType(string filterType)
        {
            //validate filtertype againts FilterType Enum
        }

        private void ValidateFilterExpression(string filterExpression)
        {
           // validate filter expression againsts its type and inputs 
        }
    }
}
