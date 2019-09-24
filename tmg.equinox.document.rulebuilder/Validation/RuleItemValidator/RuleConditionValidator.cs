using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.validation
{
    public class RuleConditionValidator
    {
        RuleConditions _ruleCondition;
        Dictionary<string, List<string>> _validationErrorMessages;

        public RuleConditionValidator(RuleConditions ruleCondition, ref Dictionary<string, List<string>> validationErrorMessages)
        {
            _ruleCondition = ruleCondition;
            _validationErrorMessages = validationErrorMessages;
        }

        public void ValidateRuleSourceContainer()
        {
            ValidateSourceFilterType();
            ValidateSourceMergeActions();

            //foreach (Source ruleSource in _ruleCondition.sources)
            //{
            //    RuleFilterValidator filterValidator = new RuleFilterValidator(ruleSource.filter,ref _validationErrorMessages);
            //  //  ValidateSourceElementType(ruleSource.sourceelementtype);
            //    //ValidateSourceType(ruleSource.sourcetype);
            //    //ValidateSourceMappings(ruleSource.mappings);
            //    filterValidator.ValidateFilters();
            //}
        }

        private void ValidateSourceElementType(string sourceElementType)
        {
            // validate sourceelement type againts SourceElementType enum.
        }

        private void ValidateSourceType(string sourceType)
        {
            //validate SourceType enum
        }

        private void ValidateSourceMappings(Mappings sourceMappings)
        {
            // number of sourcefield should be equal to number of targetfields.
        }

        private void ValidateSourceFilterType()
        {
            //Validate RuleCondition source filtertype againts FilterType Enum
        }

        private void ValidateSourceMergeActions()
        {
            //validate RuleCondition source merge type
        }
    }
}
