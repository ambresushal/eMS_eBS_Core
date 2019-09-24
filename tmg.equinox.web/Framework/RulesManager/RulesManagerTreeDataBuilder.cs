using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tmg.equinox.applicationservices.viewmodels.RulesManager;

namespace tmg.equinox.web.Framework.RulesManager
{
    public class RulesManagerTreeDataBuilder
    {
        private List<RuleHierarchyViewModel> _rules;
        private int _ruleId;
        public RulesManagerTreeDataBuilder(List<RuleHierarchyViewModel> rules, int ruleId)
        {
            _rules = rules;
            _ruleId = ruleId;
        }
        public JArray GenerateTreeData()
        {
            JArray result = new JArray();
            Dictionary<string, string> parentDict = new Dictionary<string, string>();
            int ctr = 65;
            foreach (var rule in _rules)
            {
                string elementName = rule.SourceElementID.ToString() + "|" + rule.TargetUIElementID.ToString() + "|" + rule.RuleID.ToString();
                string elemID = elementName;
                if (parentDict.Keys.Contains(elementName) == false)
                {
                    parentDict.Add(elementName, Convert.ToChar(ctr).ToString());
                    elementName = Convert.ToChar(ctr).ToString();
                    ctr++;
                }
                else
                {
                    elementName = parentDict[elementName];
                }
                JObject obj  = new JObject();
                result.Add(obj);
                foreach(var childRule in _rules)
                {
                    if(childRule.SourceElementID == rule.TargetUIElementID)
                    {
                        childRule.ParentKey = elementName;
                    }
                }

                obj.Add("text", new JObject());
                JObject text = (JObject)obj["text"];
                obj.Add("source", rule.SourceElementPath);
                text.Add("rule", rule.TargetElementName + " - " + rule.TargetProperty);
                obj.Add("htmlid", elemID);
                obj.Add("target", rule.TargetSectionPath);
                obj.Add("targetprop", rule.TargetProperty);
                obj.Add("ruledesc", rule.RuleDescription);
                if(rule.RuleID == _ruleId)
                {
                    obj.Add("isselectedrule", true);
                }
                else
                {
                    obj.Add("isselectedrule", false);
                }
                if (rule.Level > 0)
                {
                    obj.Add("parent", rule.ParentKey);
                }
                obj.Add("key", elementName); 
            }
            foreach (var obj in result)
            {
                var res = from resl in result where resl["parent"] != null && resl["parent"].ToString() == obj["key"].ToString() select resl;
                if(!(res != null && res.Count() > 1))
                {
                    ((JObject)obj).Add("collapse", 0 );
                }
                else
                {
                    ((JObject)obj).Add("collapse", 1);
                }
            }
            return result;
        }

        public JArray GenerateTreeDataV2()
        {
            JArray result = new JArray();
            Dictionary<string, string> parentDict = new Dictionary<string, string>();
            //find the rule id in the list
            //from that ruleid, track till the entire chain of rules before and after
            var ruleObj = from rul in _rules where rul.RuleID ==_ruleId select rul;
            List<RuleHierarchyViewModel> ruleObjectsForTree = new List<RuleHierarchyViewModel>();
            if(ruleObj != null && ruleObj.Count() > 0)
            {
                foreach(var rl in ruleObj)
                {
                    ruleObjectsForTree.Add(rl);
                    GetParentNodes(rl, ruleObjectsForTree);
                    GetChildNodes(rl, ruleObjectsForTree);
                    //each one is in a different group
                    //for each group, get the single 
                }
            }

            return GenerateTreeData(ruleObjectsForTree);
        }

        private void GetParentNodes(RuleHierarchyViewModel ruleNode,List<RuleHierarchyViewModel> ruleObjectsForTree)
        {
            //find nodes where targetelementid is sourceelementid
            int sourceElementId = ruleNode.SourceElementID;
            RuleHierarchyViewModel model = GetParentNode(sourceElementId);
            while(model != null)
            {
                ruleObjectsForTree.Add(model);
                model = GetParentNode(model.SourceElementID);
            }
        }

        private void GetChildNodes(RuleHierarchyViewModel ruleNode, List<RuleHierarchyViewModel> ruleObjectsForTree)
        {
            //find nodes where targetelementid is sourceelementid- tree
            int targetElementId = ruleNode.TargetUIElementID;
            List<RuleHierarchyViewModel> models = GetChildNodes(targetElementId);
            if(models != null && models.Count() > 0)
            {
                ruleObjectsForTree.AddRange(models);
                foreach (var model in models)
                {
                    GetChildNodes(model, ruleObjectsForTree);
                }
            }
        }

        private RuleHierarchyViewModel GetParentNode(int elementID)
        {
            var r = from rl in _rules where rl.TargetUIElementID == elementID select rl;
            if(r != null && r.Count() > 0)
            {
                return r.First();
            }
            else
            {
                return null;
            }
        }
        private List<RuleHierarchyViewModel> GetChildNodes(int elementID)
        {
            List<RuleHierarchyViewModel> rulesToReturn = null;
            var r = from rl in _rules where rl.SourceElementID == elementID select rl;
            if (r != null && r.Count() > 0)
            {
                rulesToReturn = new List<RuleHierarchyViewModel>();
                var groups = r.GroupBy(a => a.RuleID);
                foreach(var grp in groups)
                {
                    rulesToReturn.Add(grp.ToList().First());
                }
            }
            return rulesToReturn;
        }

        public JArray GenerateTreeData(List<RuleHierarchyViewModel> rules)
        {
            JArray result = new JArray();
            rules = rules.OrderBy(a => a.Level).ToList();
            Dictionary<string, string> parentDict = new Dictionary<string, string>();
            int ctr = 65;
            foreach (var rule in rules)
            {
                string elementName = rule.SourceElementID.ToString() + "|" + rule.TargetUIElementID.ToString() + "|" + rule.RuleID.ToString();
                string elemID = elementName;
                if (parentDict.Keys.Contains(elementName) == false)
                {
                    parentDict.Add(elementName, Convert.ToChar(ctr).ToString());
                    elementName = Convert.ToChar(ctr).ToString();
                    ctr++;
                }
                else
                {
                    elementName = parentDict[elementName];
                }
                foreach (var childRule in rules)
                {
                    if (childRule.SourceElementID == rule.TargetUIElementID)
                    {
                        childRule.ParentKey = elementName;
                    }
                }
                if (!(rule.Level > 0 && String.IsNullOrEmpty(rule.ParentKey)))
                {
                    JObject obj = new JObject();
                    result.Add(obj);
                    obj.Add("text", new JObject());
                    JObject text = (JObject)obj["text"];
                    obj.Add("source", rule.SourceElementPath);
                    text.Add("rule", rule.TargetElementName + " - " + rule.TargetProperty);
                    obj.Add("htmlid", elemID);
                    obj.Add("target", rule.TargetSectionPath);
                    obj.Add("targetprop", rule.TargetProperty);
                    obj.Add("ruledesc", rule.RuleDescription);
                    if (rule.RuleID == _ruleId)
                    {
                        obj.Add("isselectedrule", true);
                    }
                    else
                    {
                        obj.Add("isselectedrule", false);
                    }
                    if (rule.Level > 0)
                    {
                        obj.Add("parent", rule.ParentKey);
                    }
                    obj.Add("key", elementName);
                }
            }
            foreach (var obj in result)
            {
                var res = from resl in result where resl["parent"] != null && resl["parent"].ToString() == obj["key"].ToString() select resl;
                if (!(res != null && res.Count() > 1))
                {
                    ((JObject)obj).Add("collapse", 0);
                }
                else
                {
                    ((JObject)obj).Add("collapse", 1);
                }
            }
            return result;
        }
    }
}