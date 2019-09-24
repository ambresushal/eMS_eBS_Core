using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.document.rulebuilder.model
{
    public class CompiledDocumentRule
    {
        public string RuleAlias { get; set; }
        public RuleTarget Target { get; set; }
        public RuleSourcesContainer SourceContainer { get; set; }
    }

    public class RuleTarget
    {
        public string TargetPath { get; set; }
    }

    public class RuleTrigger
    {
        public string Source { get; set; }
        public string Event { get; set; }
    }

    public class RuleAction
    {
        public string Action { get; set; }
    }

    public class SourceMereList
    {
        public OutputProperties OutputColumns { get; set; }
        public List<SourceMergeAction> SourceMergeActions { get; set; }
    }

    public class SourceMergeAction
    {
        public FilterType SourceMergeType { get; set; }
        public Dictionary<string, RuleFilterExpression> MergeExpression { get; set; }
        public string CodeBlock { get; set; }
    }

    public class OutputProperties
    {
        public Dictionary<string, List<KeyValuePair<string, string>>> Columns { get; set; }
        public JObject Children { get; set; }
    }

    public class RuleSourcesContainer
    {
        public List<RuleSourceItem> RuleSources { get; set; }
        public SourceMereList SourceMergeList { get; set; }
    }

    public class RuleSourceItem
    {
        public string SourceName { get; set; }
        public int SequenceNumber { get; set; }
        public string SourcePath { get; set; }
    }

    public class RuleFilter
    {
        public List<RuleFilterItem> Filters { get; set; }
        public FilterType FilterMergeType { get; set; }
        public Dictionary<string, RuleFilterExpression> MergeAction { get; set; }
        public Dictionary<string, string> KeyColumn { get; set; }
    }

    public class RuleFilterItem
    {
        public FilterType FilterType { get; set; }
        public string FilterName { get; set; }
        public Dictionary<string, RuleFilterExpression> Expression { get; set; }
        public Dictionary<string, string> DistinctKeyColumn { get; set; }
    }

    public class RuleFilterExpression
    {
        public RuleOperand LeftOperand { get; set; }
        public RuleOperand RightOperand { get; set; }
        public RuleOperatorType Operator { get; set; }
        public int SequenceNumber { get; set; }
        public ExecutionType ExecutionType { get; set; }
    }

    public class RuleOperand
    {
        public string OperandValue { get; set; }
        public OperandType OperandType { get; set; }
        public string OperandPath { get; set; }
    }

    public enum RuleExpressionType
    {
        Simple, //A and B
        Nested // (A and B) or (C or D)
    }

    public enum RuleOperatorType
    {
        intersect,
        union,
        coljoin,
        except,
        distinct,
        crossjoin,

        equalto,
        lessthan,
        lessthanequalto,
        greaterthan,
        greaterthanequalto,
        contains,
        notequalto
    }

    public enum OperandType
    {
        parent,
        source,
        target,
        child,
        self,
        property,
        derived,
    }

    public enum FilterType
    {
        distinct,
        expr,
        self,
        none,
        script
    }

    public enum ExecutionType
    {
        crossjoin,
        collectioncomparer,
        collectionvaluecomparer,
        self,
        none
    }

    public enum ElementType
    {
        Repeater,
        Section,
        Field
    }
}