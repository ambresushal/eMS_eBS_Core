using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.document.rulebuilder.globalUtility;
using tmg.equinox.document.rulebuilder.model;

namespace tmg.equinox.document.rulebuilder.rulecompiler
{
    /// <summary>
    /// Compiles the Document Rule
    /// </summary>
 public class DocumentRuleCompiler
    {
        Documentrule _rule;
        public DocumentRuleCompiler(Documentrule rule)
        {
            _rule = rule;
        }

        public CompiledDocumentRule CompileRule()
        {
            CompiledDocumentRule compiledRule = new CompiledDocumentRule();

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

        private RuleAction CompileRuleActions(tmg.equinox.document.rulebuilder.model.Action action)
        {
            RuleAction ruleAction = new RuleAction();
            ruleAction.Action = action.action;
            return ruleAction;
        }
    }
}
