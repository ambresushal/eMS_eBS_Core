using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.validation
{
   public class RuleInputValidator
    {
       Documentrule _rule;
       Dictionary<string, List<string>> validationErrorMessages;

       public RuleInputValidator( Documentrule rule)
       {
           _rule = rule;
           validationErrorMessages = new Dictionary<string, List<string>>();
       }

       public void ValidateRuleActions()
       {
           //// Validate actions items for ActionType (need to add) enum 
           List<string> actionsErrorMessages = new List<string>();
           validationErrorMessages.Add("Action", actionsErrorMessages);
       }

       public void ValidateTriggerEvents()
       {
           //Validate Trigger Events againsts Triggers (Need to Add) enum types 
           List<string> triggerErrorMessages = new List<string>();
           validationErrorMessages.Add("Trigger", triggerErrorMessages);
       }

       public void ValidateRuleConditions()
       {
           RuleConditionValidator sourceValidator = new RuleConditionValidator(_rule.ruleconditions,ref validationErrorMessages);
           sourceValidator.ValidateRuleSourceContainer();
       }
    }
}
