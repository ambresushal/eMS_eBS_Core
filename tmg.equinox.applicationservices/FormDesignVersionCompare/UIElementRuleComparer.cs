using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tmg.equinox.applicationservices.viewmodels.FormDesignVersion;
using tmg.equinox.domain.entities.Models;
using tmg.equinox.repository.interfaces;
using tmg.equinox.repository.extensions;

namespace tmg.equinox.applicationservices.FormDesignVersionCompare
{
    internal class UIElementRuleComparer : VersionComparer
    {
        int uiElementId;
        int previousUIElementId;
        IUnitOfWorkAsync _unitOfWork;

        internal UIElementRuleComparer(int uiElementId, int previousUIElementId, IUnitOfWorkAsync unitOfWork)
        {
            this.uiElementId = uiElementId;
            this.previousUIElementId = previousUIElementId;
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Compare the 2 Elements for Rules changes - if Rules have changed, it is a Major Change
        /// </summary>
        /// <returns></returns>
        internal override bool IsMajorVersion()
        {
            bool isMajor = false;
            List<PropertyRuleMap> elementRules = (from rule in this._unitOfWork.Repository<PropertyRuleMap>()
                                  .Query()
                                  .Include(d => d.Rule)
                                  .Include(e => e.Rule.Expressions)
                                  .Get()
                                    where rule.UIElementID == uiElementId
                                    select rule).ToList();

            List<PropertyRuleMap> previousElementRules = (from rule in this._unitOfWork.RepositoryAsync<PropertyRuleMap>()
                                  .Query()
                                  .Include(d => d.Rule)
                                  .Include(e => e.Rule.Expressions)
                                  .Get()
                                    where rule.UIElementID == previousUIElementId
                                    select rule).ToList();
            isMajor = IsMajorVersion(elementRules, previousElementRules);
            return isMajor;
        }

        private bool IsMajorVersion(List<PropertyRuleMap> elementRules, List<PropertyRuleMap> previousElementRules) 
        {
            bool isMajor = false;
            //initialize to empty list if null
            if (elementRules == null)
            {
                elementRules = new List<PropertyRuleMap>();
            }
            if (previousElementRules == null)
            {
                previousElementRules = new List<PropertyRuleMap>();
            }

            //start comparing rules, check for same number of rules first
            if (elementRules.Count != previousElementRules.Count)
            {
                isMajor = true;
            }
            //no need to compare Rules is count is mismatch, which means that Rules have been altered
            if (isMajor == false) 
            {
                if (elementRules.Count > 0) 
                {
                    //now compare the rules for both the elements in detail
                    for (int index = 0; index < elementRules.Count; index++) 
                    {
                        //TODO: assumption is that rules will be fetched in the same sequence, need to make this more robust
                        PropertyRuleMapCompare elementRule = new PropertyRuleMapCompare(elementRules[index]);
                        PropertyRuleMapCompare previousElementRule = new PropertyRuleMapCompare(previousElementRules[index]);
                        isMajor = !elementRule.Equals(previousElementRule);
                        if (isMajor == true)
                        {
                            break;
                        }
                    }
                }
            }
            return isMajor;
        }


        private class PropertyRuleMapCompare 
        {
            internal int TargetPropertyID;
            internal RuleCompare Rule;

            internal PropertyRuleMapCompare(PropertyRuleMap rule)
            {
                this.TargetPropertyID = rule.TargetPropertyID;
                this.Rule = new RuleCompare(rule.Rule);
            }

            public override bool Equals(object obj)
            {
                bool isEqual = false;
                if (obj != null && obj is PropertyRuleMapCompare) 
                { 
                    PropertyRuleMapCompare compare = (PropertyRuleMapCompare)obj;
                    if (this.TargetPropertyID == compare.TargetPropertyID)
                    {
                        if (this.Rule.Equals(compare.Rule) == true) 
                        {
                            isEqual = true;
                        }
                    }
                }
                return isEqual;
            }
        }

        private class RuleCompare
        {
            internal int RuleTargetTypeID;
            internal string ResultSuccess;
            internal string ResultFailure;
            internal List<ExpressionCompare> Expressions;

            internal RuleCompare(Rule rule)
            {
                this.ResultFailure = rule.ResultFailure;
                this.ResultSuccess = rule.ResultFailure;
                this.RuleTargetTypeID = rule.RuleTargetTypeID;
                if (rule.Expressions != null && rule.Expressions.Count() > 0) 
                {
                    this.Expressions = new List<ExpressionCompare>();
                    foreach (var exp in rule.Expressions)
                    {
                        this.Expressions.Add(new ExpressionCompare(exp));
                    }
                }
            }

            public override bool Equals(object obj)
            {
                bool isEqual = false;
                if (obj != null && obj is RuleCompare)
                {
                    RuleCompare compare = (RuleCompare)obj;
                    if (this.ResultFailure == compare.ResultFailure && this.ResultSuccess == compare.ResultSuccess 
                        && this.RuleTargetTypeID == compare.RuleTargetTypeID)
                    {
                        if ((this.Expressions.Count() == compare.Expressions.Count()) && this.Expressions.Count() > 0) 
                        {
                            isEqual = true;
                            for (int index = 0; index < this.Expressions.Count; index++) 
                            {
                                isEqual = this.Expressions[index].Equals(compare.Expressions[index]);
                                if (isEqual == false) 
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                return isEqual;
            }
        }

        private class ExpressionCompare
        {
            internal string LeftOperand;
            internal string RightOperand;
            internal int OperatorTypeID;
            internal int LogicalOperatorTypeID;

            internal ExpressionCompare(Expression expression) 
            {
                this.LeftOperand = expression.LeftOperand;
                this.RightOperand = expression.RightOperand;
                this.OperatorTypeID = expression.OperatorTypeID;
                this.LogicalOperatorTypeID = expression.LogicalOperatorTypeID;
            }

            public override bool Equals(object obj)
            {
                bool isEqual = false;
                if (obj != null && obj is ExpressionCompare)
                {
                    ExpressionCompare compare = (ExpressionCompare)obj;
                    if (this.LeftOperand == compare.LeftOperand && this.LogicalOperatorTypeID == compare.LogicalOperatorTypeID
                        && this.OperatorTypeID == compare.OperatorTypeID && this.RightOperand == compare.RightOperand)
                    {
                        isEqual = true;
                    }
                }
                return isEqual;
            }
        }
    }
}
