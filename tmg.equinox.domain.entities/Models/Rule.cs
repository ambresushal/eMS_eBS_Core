using System;
using System.Collections.Generic;

namespace tmg.equinox.domain.entities.Models
{
    public partial class Rule : Entity
    {
        public Rule()
        {
            this.Expressions = new List<Expression>();
            this.PropertyRuleMaps = new List<PropertyRuleMap>();
        }

        public int RuleID { get; set; }
        public string RuleName { get; set; }
        public string RuleDescription { get; set; }
        public int RuleTargetTypeID { get; set; }
        public string ResultSuccess { get; set; }
        public string ResultFailure { get; set; }
        public string AddedBy { get; set; }
        public System.DateTime AddedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public virtual ICollection<Expression> Expressions { get; set; }
        public virtual ICollection<PropertyRuleMap> PropertyRuleMaps { get; set; }
        public virtual RuleTargetType RuleTargetType { get; set; }
        public bool IsResultSuccessElement { get; set; }
        public bool IsResultFailureElement { get; set; }
        public string Message { get; set; }
        public bool RunOnLoad { get; set; }
        public string RuleCode { get; set; }
        public bool IsStandard { get; set; }
        public int? TargetTypeID { get; set; }
        public Rule Clone(string username, DateTime addedDate)
        {
            Rule item = new Rule();
            item.RuleID = this.RuleID;
            item.RuleName = this.RuleName;
            item.RuleDescription = this.RuleDescription;
            item.RuleTargetTypeID = this.RuleTargetTypeID;
            item.ResultSuccess = this.ResultSuccess;
            item.ResultFailure = this.ResultFailure;
            item.IsResultSuccessElement = this.IsResultSuccessElement;
            item.IsResultFailureElement = this.IsResultFailureElement;
            item.Message = this.Message;
            item.AddedBy = username;
            item.AddedDate = addedDate;
            item.Expressions = new List<Expression>();
            item.RunOnLoad = this.RunOnLoad;
            item.RuleCode = RuleCode;
            item.IsStandard = IsStandard;
            item.TargetTypeID = TargetTypeID;

            foreach (var exp in this.Expressions)
            {
                item.Expressions.Add(exp.Clone(username, addedDate));
            }
            return item;
        }
    }
}
