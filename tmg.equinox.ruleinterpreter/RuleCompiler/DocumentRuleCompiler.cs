using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.UIElement;
using tmg.equinox.ruleinterpreter.globalUtility;
using tmg.equinox.ruleinterpreter.model;

namespace tmg.equinox.ruleinterpreter.rulecompiler
{
    /// <summary>
    /// Compiles the Document Rule
    /// </summary>
 public class DocumentRuleCompiler
    {
        Documentrule _rule;
        int _documentRuleTypeID;
        public DocumentRuleCompiler(int documentRuleTypeID, Documentrule rule)
        {
            _documentRuleTypeID = documentRuleTypeID;
            _rule = rule;
        }

        public CompiledDocumentRule CompileRule()
        {
            CompiledDocumentRule compiledRule = new CompiledDocumentRule();
            compiledRule.DocumentRuleTypeID = _documentRuleTypeID;
            compiledRule.Target = CompileRuleTarget();
            compiledRule.SourceContainer = CompileRuleSources();
            return compiledRule;
        }

        private RuleSourcesContainer CompileRuleSources()
        {
            RuleConditionCompiler compiler = new RuleConditionCompiler(_rule.ruleconditions);
            return compiler.Compile();
        }

        private RuleTarget CompileRuleTarget()
        {
            RuleTarget target = new RuleTarget();
            target.TargetPath = _rule.targetelement;
            return target;
        }


        private RuleTrigger CompileRuleTrigger(Trigger trigger)
        {
            RuleTrigger ruleTrigger = new RuleTrigger();
            ruleTrigger.Event = trigger.@event;
            ruleTrigger.Source = trigger.source;
            return ruleTrigger;
        }

        private RuleAction CompileRuleActions(tmg.equinox.ruleinterpreter.model.Action action)
        {
            RuleAction ruleAction = new RuleAction();
            ruleAction.Action = action.action;
            return ruleAction;
        }
    }
}
